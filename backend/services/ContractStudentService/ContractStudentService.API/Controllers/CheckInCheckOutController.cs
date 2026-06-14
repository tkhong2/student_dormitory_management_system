using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;
using System.Text.Json;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInCheckOutController : ControllerBase
    {
        private readonly ICheckInRepository _checkInRepository;
        private readonly ICheckOutRepository _checkOutRepository;
        private readonly IContractRepository _contractRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<CheckInCheckOutController> _logger;

        public CheckInCheckOutController(
            ICheckInRepository checkInRepository,
            ICheckOutRepository checkOutRepository,
            IContractRepository contractRepository,
            IStudentRepository studentRepository,
            ILogger<CheckInCheckOutController> logger)
        {
            _checkInRepository = checkInRepository;
            _checkOutRepository = checkOutRepository;
            _contractRepository = contractRepository;
            _studentRepository = studentRepository;
            _logger = logger;
        }

        // GET: api/CheckInCheckOut/pending-checkins
        [HttpGet("pending-checkins")]
        public async Task<ActionResult<IEnumerable<PendingCheckInDto>>> GetPendingCheckIns()
        {
            try
            {
                // Get all Active contracts
                var activeContracts = await _contractRepository.GetByStatusAsync("Active");
                
                // Get all existing check-ins
                var existingCheckIns = await _checkInRepository.GetAllAsync();
                var checkedInContractIds = existingCheckIns.Select(c => c.ContractId).ToHashSet();

                var pendingCheckIns = new List<PendingCheckInDto>();

                foreach (var contract in activeContracts)
                {
                    var student = await _studentRepository.GetByIdAsync(contract.StudentId);
                    if (student == null) continue;

                    bool hasCheckedIn = checkedInContractIds.Contains(contract.Id);

                    // Only show contracts that start within 30 days or already started
                    var daysUntilStart = (contract.StartDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days;
                    if (daysUntilStart <= 30)
                    {
                        pendingCheckIns.Add(new PendingCheckInDto
                        {
                            Id = contract.Id,
                            ContractId = contract.Id,
                            StudentId = contract.StudentId,
                            StudentName = student.FullName,
                            StudentCode = student.StudentCode,
                            RoomId = contract.RoomId,
                            RoomNumber = contract.RoomNumber,
                            BuildingName = contract.BuildingName,
                            StartDate = contract.StartDate,
                            DepositAmount = contract.DepositAmount,
                            CheckedIn = hasCheckedIn
                        });
                    }
                }

                return Ok(pendingCheckIns.OrderBy(p => p.StartDate));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending check-ins");
                return StatusCode(500, new { message = "Error loading pending check-ins", error = ex.Message });
            }
        }

        // GET: api/CheckInCheckOut/pending-checkouts
        [HttpGet("pending-checkouts")]
        public async Task<ActionResult<IEnumerable<PendingCheckOutDto>>> GetPendingCheckOuts()
        {
            try
            {
                // Get all Active contracts
                var activeContracts = await _contractRepository.GetByStatusAsync("Active");
                
                // Get all existing check-outs
                var existingCheckOuts = await _checkOutRepository.GetAllAsync();
                var checkedOutContractIds = existingCheckOuts.Select(c => c.ContractId).ToHashSet();

                var pendingCheckOuts = new List<PendingCheckOutDto>();

                foreach (var contract in activeContracts)
                {
                    var student = await _studentRepository.GetByIdAsync(contract.StudentId);
                    if (student == null) continue;

                    bool hasCheckedOut = checkedOutContractIds.Contains(contract.Id);

                    // Show contracts ending within 60 days or already ended
                    var daysUntilEnd = (contract.EndDate.ToDateTime(TimeOnly.MinValue) - DateTime.Now).Days;
                    if (daysUntilEnd <= 60)
                    {
                        pendingCheckOuts.Add(new PendingCheckOutDto
                        {
                            Id = contract.Id,
                            ContractId = contract.Id,
                            StudentId = contract.StudentId,
                            StudentName = student.FullName,
                            StudentCode = student.StudentCode,
                            RoomId = contract.RoomId,
                            RoomNumber = contract.RoomNumber,
                            BuildingName = contract.BuildingName,
                            EndDate = contract.EndDate,
                            DepositAmount = contract.DepositAmount,
                            CheckedOut = hasCheckedOut
                        });
                    }
                }

                return Ok(pendingCheckOuts.OrderBy(p => p.EndDate));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending check-outs");
                return StatusCode(500, new { message = "Error loading pending check-outs", error = ex.Message });
            }
        }

        // GET: api/CheckInCheckOut/checkins
        [HttpGet("checkins")]
        public async Task<ActionResult<IEnumerable<CheckInDto>>> GetAllCheckIns()
        {
            try
            {
                var checkIns = await _checkInRepository.GetAllAsync();
                var result = checkIns.Select(c => new CheckInDto
                {
                    Id = c.Id,
                    ContractId = c.ContractId,
                    StudentId = c.StudentId,
                    StudentName = c.Student?.FullName,
                    StudentCode = c.Student?.StudentCode,
                    RoomId = c.RoomId,
                    RoomNumber = c.RoomNumber,
                    BuildingName = c.BuildingName,
                    CheckInDate = c.CheckInDate,
                    CheckedInByUserId = c.CheckedInByUserId,
                    CheckedInByName = c.CheckedInByName,
                    IdCardImageUrls = c.IdCardImageUrls,
                    IsDepositPaid = c.IsDepositPaid,
                    DepositAmount = c.DepositAmount,
                    DepositPaidAt = c.DepositPaidAt,
                    RoomConditionChecklist = c.RoomConditionChecklist,
                    RoomImageUrls = c.RoomImageUrls,
                    RoomCondition = c.RoomCondition,
                    Notes = c.Notes,
                    InitialElectricityReading = c.InitialElectricityReading,
                    InitialWaterReading = c.InitialWaterReading,
                    KeysProvided = c.KeysProvided,
                    KeyCount = c.KeyCount,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting check-ins");
                return StatusCode(500, new { message = "Error loading check-ins", error = ex.Message });
            }
        }

        // GET: api/CheckInCheckOut/checkouts
        [HttpGet("checkouts")]
        public async Task<ActionResult<IEnumerable<CheckOutDto>>> GetAllCheckOuts()
        {
            try
            {
                var checkOuts = await _checkOutRepository.GetAllAsync();
                var result = checkOuts.Select(c => new CheckOutDto
                {
                    Id = c.Id,
                    ContractId = c.ContractId,
                    StudentId = c.StudentId,
                    StudentName = c.Student?.FullName,
                    StudentCode = c.Student?.StudentCode,
                    RoomId = c.RoomId,
                    RoomNumber = c.RoomNumber,
                    BuildingName = c.BuildingName,
                    CheckOutDate = c.CheckOutDate,
                    CheckedOutByUserId = c.CheckedOutByUserId,
                    CheckedOutByName = c.CheckedOutByName,
                    CurrentRoomImageUrls = c.CurrentRoomImageUrls,
                    RoomCondition = c.RoomCondition,
                    DamageDescription = c.DamageDescription,
                    DepositAmount = c.DepositAmount,
                    CompensationCost = c.CompensationCost,
                    RefundAmount = c.RefundAmount,
                    CompensationDetails = c.CompensationDetails,
                    FinalElectricityReading = c.FinalElectricityReading,
                    FinalWaterReading = c.FinalWaterReading,
                    IsKeyReturned = c.IsKeyReturned,
                    IsDepositRefunded = c.IsDepositRefunded,
                    DepositRefundedAt = c.DepositRefundedAt,
                    RefundMethod = c.RefundMethod,
                    RefundReference = c.RefundReference,
                    Notes = c.Notes,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting check-outs");
                return StatusCode(500, new { message = "Error loading check-outs", error = ex.Message });
            }
        }

        // POST: api/CheckInCheckOut/checkin
        [HttpPost("checkin")]
        public async Task<ActionResult<CheckInDto>> CreateCheckIn([FromBody] CreateCheckInDto dto)
        {
            try
            {
                // Validate contract exists and is active
                var contract = await _contractRepository.GetByIdAsync(dto.ContractId);
                if (contract == null)
                    return NotFound(new { message = "Contract not found" });

                if (contract.Status != "Active")
                    return BadRequest(new { message = "Contract is not active" });

                // Check if already checked in
                var existingCheckIn = await _checkInRepository.GetByContractIdAsync(dto.ContractId);
                if (existingCheckIn != null)
                    return BadRequest(new { message = "This contract has already been checked in" });

                var checkIn = new CheckIn
                {
                    ContractId = dto.ContractId,
                    StudentId = dto.StudentId,
                    RoomId = dto.RoomId,
                    RoomNumber = dto.RoomNumber,
                    BuildingName = dto.BuildingName,
                    CheckInDate = dto.CheckInDate,
                    CheckedInByUserId = dto.CheckedInByUserId,
                    CheckedInByName = dto.CheckedInByName,
                    IdCardImageUrls = dto.IdCardImageUrls,
                    IsDepositPaid = dto.IsDepositPaid,
                    DepositAmount = dto.DepositAmount,
                    DepositPaidAt = dto.DepositPaidAt,
                    RoomConditionChecklist = dto.RoomConditionChecklist,
                    RoomImageUrls = dto.RoomImageUrls,
                    RoomCondition = dto.RoomCondition,
                    Notes = dto.Notes,
                    InitialElectricityReading = dto.InitialElectricityReading,
                    InitialWaterReading = dto.InitialWaterReading,
                    KeysProvided = dto.KeysProvided,
                    KeyCount = dto.KeyCount,
                    Status = "Completed"
                };

                await _checkInRepository.AddAsync(checkIn);

                // Update contract deposit status if paid
                if (dto.IsDepositPaid && !contract.IsDepositPaid)
                {
                    contract.IsDepositPaid = true;
                    contract.DepositPaidAt = dto.DepositPaidAt;
                    await _contractRepository.UpdateAsync(contract);
                }

                var result = new CheckInDto
                {
                    Id = checkIn.Id,
                    ContractId = checkIn.ContractId,
                    StudentId = checkIn.StudentId,
                    RoomId = checkIn.RoomId,
                    RoomNumber = checkIn.RoomNumber,
                    BuildingName = checkIn.BuildingName,
                    CheckInDate = checkIn.CheckInDate,
                    CheckedInByUserId = checkIn.CheckedInByUserId,
                    CheckedInByName = checkIn.CheckedInByName,
                    IdCardImageUrls = checkIn.IdCardImageUrls,
                    IsDepositPaid = checkIn.IsDepositPaid,
                    DepositAmount = checkIn.DepositAmount,
                    DepositPaidAt = checkIn.DepositPaidAt,
                    RoomConditionChecklist = checkIn.RoomConditionChecklist,
                    RoomImageUrls = checkIn.RoomImageUrls,
                    RoomCondition = checkIn.RoomCondition,
                    Notes = checkIn.Notes,
                    InitialElectricityReading = checkIn.InitialElectricityReading,
                    InitialWaterReading = checkIn.InitialWaterReading,
                    KeysProvided = checkIn.KeysProvided,
                    KeyCount = checkIn.KeyCount,
                    Status = checkIn.Status,
                    CreatedAt = checkIn.CreatedAt
                };

                return CreatedAtAction(nameof(GetAllCheckIns), new { id = checkIn.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating check-in");
                return StatusCode(500, new { message = "Error creating check-in", error = ex.Message });
            }
        }

        // POST: api/CheckInCheckOut/checkout
        [HttpPost("checkout")]
        public async Task<ActionResult<CheckOutDto>> CreateCheckOut([FromBody] CreateCheckOutDto dto)
        {
            try
            {
                // Validate contract exists and is active
                var contract = await _contractRepository.GetByIdAsync(dto.ContractId);
                if (contract == null)
                    return NotFound(new { message = "Contract not found" });

                if (contract.Status != "Active")
                    return BadRequest(new { message = "Contract is not active" });

                // Check if already checked out
                var existingCheckOut = await _checkOutRepository.GetByContractIdAsync(dto.ContractId);
                if (existingCheckOut != null)
                    return BadRequest(new { message = "This contract has already been checked out" });

                var checkOut = new CheckOut
                {
                    ContractId = dto.ContractId,
                    StudentId = dto.StudentId,
                    RoomId = dto.RoomId,
                    RoomNumber = dto.RoomNumber,
                    BuildingName = dto.BuildingName,
                    CheckOutDate = dto.CheckOutDate,
                    CheckedOutByUserId = dto.CheckedOutByUserId,
                    CheckedOutByName = dto.CheckedOutByName,
                    CurrentRoomImageUrls = dto.CurrentRoomImageUrls,
                    RoomCondition = dto.RoomCondition,
                    DamageDescription = dto.DamageDescription,
                    DepositAmount = dto.DepositAmount,
                    CompensationCost = dto.CompensationCost,
                    RefundAmount = dto.RefundAmount,
                    CompensationDetails = dto.CompensationDetails,
                    FinalElectricityReading = dto.FinalElectricityReading,
                    FinalWaterReading = dto.FinalWaterReading,
                    IsKeyReturned = dto.IsKeyReturned,
                    IsDepositRefunded = dto.IsDepositRefunded,
                    DepositRefundedAt = dto.DepositRefundedAt,
                    RefundMethod = dto.RefundMethod,
                    RefundReference = dto.RefundReference,
                    Notes = dto.Notes,
                    Status = "Completed"
                };

                await _checkOutRepository.AddAsync(checkOut);

                // Update contract status and deposit return info
                contract.Status = "Terminated";
                contract.TerminatedAt = dto.CheckOutDate;
                contract.TerminatedReason = "Normal check-out completion";
                contract.DepositReturnedAmount = dto.RefundAmount;
                contract.DepositReturnedAt = dto.DepositRefundedAt;
                if (dto.CompensationCost > 0)
                {
                    contract.DepositDeductionReason = dto.CompensationDetails ?? $"Room damage compensation: {dto.DamageDescription}";
                }
                await _contractRepository.UpdateAsync(contract);

                var result = new CheckOutDto
                {
                    Id = checkOut.Id,
                    ContractId = checkOut.ContractId,
                    StudentId = checkOut.StudentId,
                    RoomId = checkOut.RoomId,
                    RoomNumber = checkOut.RoomNumber,
                    BuildingName = checkOut.BuildingName,
                    CheckOutDate = checkOut.CheckOutDate,
                    CheckedOutByUserId = checkOut.CheckedOutByUserId,
                    CheckedOutByName = checkOut.CheckedOutByName,
                    CurrentRoomImageUrls = checkOut.CurrentRoomImageUrls,
                    RoomCondition = checkOut.RoomCondition,
                    DamageDescription = checkOut.DamageDescription,
                    DepositAmount = checkOut.DepositAmount,
                    CompensationCost = checkOut.CompensationCost,
                    RefundAmount = checkOut.RefundAmount,
                    CompensationDetails = checkOut.CompensationDetails,
                    FinalElectricityReading = checkOut.FinalElectricityReading,
                    FinalWaterReading = checkOut.FinalWaterReading,
                    IsKeyReturned = checkOut.IsKeyReturned,
                    IsDepositRefunded = checkOut.IsDepositRefunded,
                    DepositRefundedAt = checkOut.DepositRefundedAt,
                    RefundMethod = checkOut.RefundMethod,
                    RefundReference = checkOut.RefundReference,
                    Notes = checkOut.Notes,
                    Status = checkOut.Status,
                    CreatedAt = checkOut.CreatedAt
                };

                return CreatedAtAction(nameof(GetAllCheckOuts), new { id = checkOut.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating check-out");
                return StatusCode(500, new { message = "Error creating check-out", error = ex.Message });
            }
        }

        // GET: api/CheckInCheckOut/checkin/contract/{contractId}
        [HttpGet("checkin/contract/{contractId}")]
        public async Task<ActionResult<CheckInDto>> GetCheckInByContractId(int contractId)
        {
            try
            {
                var checkIn = await _checkInRepository.GetByContractIdAsync(contractId);
                if (checkIn == null)
                    return NotFound(new { message = "Check-in record not found for this contract" });

                var result = new CheckInDto
                {
                    Id = checkIn.Id,
                    ContractId = checkIn.ContractId,
                    StudentId = checkIn.StudentId,
                    StudentName = checkIn.Student?.FullName,
                    StudentCode = checkIn.Student?.StudentCode,
                    RoomId = checkIn.RoomId,
                    RoomNumber = checkIn.RoomNumber,
                    BuildingName = checkIn.BuildingName,
                    CheckInDate = checkIn.CheckInDate,
                    CheckedInByUserId = checkIn.CheckedInByUserId,
                    CheckedInByName = checkIn.CheckedInByName,
                    IdCardImageUrls = checkIn.IdCardImageUrls,
                    IsDepositPaid = checkIn.IsDepositPaid,
                    DepositAmount = checkIn.DepositAmount,
                    DepositPaidAt = checkIn.DepositPaidAt,
                    RoomConditionChecklist = checkIn.RoomConditionChecklist,
                    RoomImageUrls = checkIn.RoomImageUrls,
                    RoomCondition = checkIn.RoomCondition,
                    Notes = checkIn.Notes,
                    InitialElectricityReading = checkIn.InitialElectricityReading,
                    InitialWaterReading = checkIn.InitialWaterReading,
                    KeysProvided = checkIn.KeysProvided,
                    KeyCount = checkIn.KeyCount,
                    Status = checkIn.Status,
                    CreatedAt = checkIn.CreatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting check-in by contract ID");
                return StatusCode(500, new { message = "Error loading check-in", error = ex.Message });
            }
        }

        // GET: api/CheckInCheckOut/checkout/contract/{contractId}
        [HttpGet("checkout/contract/{contractId}")]
        public async Task<ActionResult<CheckOutDto>> GetCheckOutByContractId(int contractId)
        {
            try
            {
                var checkOut = await _checkOutRepository.GetByContractIdAsync(contractId);
                if (checkOut == null)
                    return NotFound(new { message = "Check-out record not found for this contract" });

                var result = new CheckOutDto
                {
                    Id = checkOut.Id,
                    ContractId = checkOut.ContractId,
                    StudentId = checkOut.StudentId,
                    StudentName = checkOut.Student?.FullName,
                    StudentCode = checkOut.Student?.StudentCode,
                    RoomId = checkOut.RoomId,
                    RoomNumber = checkOut.RoomNumber,
                    BuildingName = checkOut.BuildingName,
                    CheckOutDate = checkOut.CheckOutDate,
                    CheckedOutByUserId = checkOut.CheckedOutByUserId,
                    CheckedOutByName = checkOut.CheckedOutByName,
                    CurrentRoomImageUrls = checkOut.CurrentRoomImageUrls,
                    RoomCondition = checkOut.RoomCondition,
                    DamageDescription = checkOut.DamageDescription,
                    DepositAmount = checkOut.DepositAmount,
                    CompensationCost = checkOut.CompensationCost,
                    RefundAmount = checkOut.RefundAmount,
                    CompensationDetails = checkOut.CompensationDetails,
                    FinalElectricityReading = checkOut.FinalElectricityReading,
                    FinalWaterReading = checkOut.FinalWaterReading,
                    IsKeyReturned = checkOut.IsKeyReturned,
                    IsDepositRefunded = checkOut.IsDepositRefunded,
                    DepositRefundedAt = checkOut.DepositRefundedAt,
                    RefundMethod = checkOut.RefundMethod,
                    RefundReference = checkOut.RefundReference,
                    Notes = checkOut.Notes,
                    Status = checkOut.Status,
                    CreatedAt = checkOut.CreatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting check-out by contract ID");
                return StatusCode(500, new { message = "Error loading check-out", error = ex.Message });
            }
        }
    }
}
