# Technical Design Document

## ✅ Implementation Status (Updated: 2026-06-06)

### Completed Components

#### Backend (100% Complete)
- ✅ RoomApplication entity with assigned room fields
- ✅ Contract entity with auto-generation logic
- ✅ BuildingAssignment entity for staff authorization
- ✅ Repository methods: `GetActiveApplicationsByStudentAsync()`, `GetActiveContractsByStudentAsync()`, `GetNextSequenceForYearAsync()`
- ✅ API Endpoints:
  - `POST /api/roomapplications` - Create application with room selection
  - `PUT /api/roomapplications/{id}/approve` - Simplified approval (no room input)
  - `PUT /api/roomapplications/{id}/reject` - Reject application
  - `POST /api/roomapplications/{id}/accept` - Student acceptance → auto-create contract

#### Frontend (100% Complete)
- ✅ StudentRoomListView - Browse and select specific rooms
- ✅ StudentRoomRegistrationView - Submit application with selected room
- ✅ RoomApplicationListView (Admin) - Approve/reject with room already selected
- ✅ MyContractView (Student) - View approved applications, preview contract, accept to create contract
- ✅ Contract display with status indicators

### Workflow Now Implemented

```
Student selects room 101 → Submits application → Admin approves (no re-entry) 
→ Student sees preview → Student accepts → System auto-creates contract HD{year}{seq}
→ Admin sees contract in contracts list
```

---

## Introduction

This document provides the technical design for the Room Registration Workflow feature in the Dormitory Management System. The feature implements a comprehensive workflow that allows students to browse and apply for dormitory rooms, enables staff members to review and approve applications with building-level authorization, and automatically creates rental contracts when students accept approved applications.

The system is built on a microservices architecture with three main services:
- **RoomBuildingService**: Manages building and room data
- **ContractStudentService**: Manages students, applications, and contracts
- **BillingMaintenanceService**: Manages users, authentication, and authorization

## System Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      Frontend (Vue.js)                      │
│  - PublicLayout (guest users)                               │
│  - StudentLayout (student portal)                           │
│  - DefaultLayoutAnt (admin/staff portal)                    │
└────────────────────────┬────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────┐
│                API Gateway (Ocelot) - Port 5052             │
└────────────┬──────────────────┬─────────────────┬───────────┘
             │                  │                 │
             ▼                  ▼                 ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│ RoomBuilding     │  │ ContractStudent  │  │ BillingMaint     │
│ Service          │  │ Service          │  │ Service          │
│ Port 5001        │  │ Port 5059        │  │ Port 5002        │
└────────┬─────────┘  └────────┬─────────┘  └────────┬─────────┘
         │                     │                      │
         ▼                     ▼                      ▼
┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐
│ RoomBuildingDb   │  │ ContractStudentDb│  │ BillingMaintenDb │
└──────────────────┘  └──────────────────┘  └──────────────────┘
```

### Service Responsibilities

#### RoomBuildingService
- Manages Buildings, Floors, Rooms, and RoomTypes
- Provides room availability information
- Handles room status changes
- Provides room search and filtering capabilities

#### ContractStudentService
- Manages Student profiles
- Manages RoomApplication lifecycle (create, review, approve, reject)
- Manages Contract lifecycle (create, activate, terminate)
- Stores application and contract snapshot data
- Enforces business rules for applications and contracts

#### BillingMaintenanceService
- Manages User accounts and authentication
- Provides JWT-based authentication
- Manages BuildingAssignment for staff members
- Provides authorization services for role-based and building-level access control

### Data Flow: Student Room Registration

```
Student → Frontend → API Gateway → ContractStudentService
                                           ↓
                                    RoomApplication Created
                                    (Status: Pending)
                                           ↓
                                    Notification sent
```

### Data Flow: Application Approval

```
Admin/Staff → Frontend → API Gateway → BillingMaintenanceService
                                              ↓
                                    Authorization Check
                                    (Role + Building Assignment)
                                              ↓
                                    ← Authorized/Denied
                                              ↓
                            (If Authorized) ContractStudentService
                                              ↓
                                    Application Updated
                                    (Status: Approved/Rejected)
                                              ↓
                                    Notification sent
```

### Data Flow: Contract Creation on Acceptance

```
Student → Frontend → API Gateway → ContractStudentService
                                           ↓
                                    Validate Application Status = Approved
                                           ↓
                                    Create Contract Entity
                                    - Copy data from Application
                                    - Generate ContractCode
                                    - Set default rates
                                    - Set Status: Pending
                                           ↓
                                    Link Contract ↔ Application
                                           ↓
                                    Notification sent
```

## Component Design

### Frontend Components

#### Public Components (Guest Access)
- **RoomRegistrationPage.vue**: Displays available rooms for public browsing
- **HomePage.vue**: Landing page with system overview

#### Student Components (Authenticated Students)
- **StudentRoomListView.vue**: Browse available rooms with filters
- **StudentRoomRegistrationView.vue**: Submit room application form
- **MyRoomView.vue**: View current room assignment
- **MyContractView.vue**: View and accept approved contracts
- **StudentDashboard.vue**: Overview of applications and contracts

#### Admin/Staff Components (Authenticated Admin/Staff)
- **RoomApplicationListView.vue**: Review all applications with filters
- **ContractListView.vue**: View all contracts
- **UserListView.vue**: Manage user accounts and building assignments
- **BuildingListView.vue**: Manage buildings
- **RoomListView.vue**: Manage rooms

### Backend API Endpoints

#### RoomBuildingService (Port 5001)

```csharp
// Buildings
GET    /api/buildings                    // Get all buildings
GET    /api/buildings/{id}                // Get building by ID
POST   /api/buildings                     // Create building
PUT    /api/buildings/{id}                // Update building
DELETE /api/buildings/{id}                // Delete building

// Rooms
GET    /api/rooms                         // Get all rooms
GET    /api/rooms/{id}                    // Get room by ID
GET    /api/rooms/available               // Get available rooms
GET    /api/rooms/building/{buildingId}  // Get rooms by building
POST   /api/rooms                         // Create room
PUT    /api/rooms/{id}                    // Update room
DELETE /api/rooms/{id}                    // Delete room

// Room Types
GET    /api/roomtypes                     // Get all room types
GET    /api/roomtypes/{id}                // Get room type by ID
```

#### ContractStudentService (Port 5059)

```csharp
// Students
GET    /api/students                      // Get all students
GET    /api/students/{id}                 // Get student by ID
GET    /api/students/by-user/{userId}     // Get student by UserId
POST   /api/students                      // Create student profile
PUT    /api/students/{id}                 // Update student profile

// Room Applications
GET    /api/roomapplications               // Get all applications
GET    /api/roomapplications/{id}          // Get application by ID
GET    /api/roomapplications/student/{studentId}  // Get student's applications
POST   /api/roomapplications               // Create new application
PUT    /api/roomapplications/{id}/approve  // Approve application
PUT    /api/roomapplications/{id}/reject   // Reject application
PUT    /api/roomapplications/{id}/cancel   // Cancel application
POST   /api/roomapplications/{id}/accept   // Student accepts approved application

// Contracts
GET    /api/contracts                      // Get all contracts
GET    /api/contracts/{id}                 // Get contract by ID
GET    /api/contracts/student/{studentId}  // Get student's contracts
POST   /api/contracts                      // Create contract (automated)
PUT    /api/contracts/{id}                 // Update contract
PUT    /api/contracts/{id}/terminate       // Terminate contract
```

#### BillingMaintenanceService (Port 5002)

```csharp
// Authentication
POST   /api/auth/login                    // Login and get JWT token
POST   /api/auth/refresh                  // Refresh JWT token
POST   /api/auth/logout                   // Logout

// Users
GET    /api/users                         // Get all users
GET    /api/users/{id}                    // Get user by ID
POST   /api/users                         // Create user account
PUT    /api/users/{id}                    // Update user account
DELETE /api/users/{id}                    // Delete user account

// Building Assignments (for Staff)
GET    /api/buildingassignments           // Get all assignments
GET    /api/buildingassignments/staff/{userId}  // Get staff's assignments
POST   /api/buildingassignments           // Create assignment
DELETE /api/buildingassignments/{id}      // Remove assignment
```

## Data Models

### Core Entities

#### RoomApplication (ContractStudentDb)

```csharp
public class RoomApplication
{
    public int Id { get; set; }
    
    // Student Information
    public int StudentId { get; set; }
    public Student Student { get; set; }
    
    // Preferred Room Information (Snapshot Data)
    public int PreferredBuildingId { get; set; }
    public string PreferredBuildingName { get; set; }    // Snapshot
    public int PreferredRoomTypeId { get; set; }
    public string PreferredRoomTypeName { get; set; }    // Snapshot
    public decimal PreferredRoomPrice { get; set; }      // Snapshot
    
    // Requested Period
    public DateOnly RequestedStartDate { get; set; }
    public DateOnly RequestedEndDate { get; set; }
    
    // Application Details
    public string? SpecialRequirements { get; set; }
    public string? Note { get; set; }
    public bool IsLocalStudent { get; set; }
    public int Priority { get; set; }                    // 0=local, 1=non-local
    public string? AttachedDocumentUrls { get; set; }
    
    // Status Management
    public string Status { get; set; }                   // Pending, UnderReview, Approved, Rejected, Cancelled
    public DateTime CreatedAt { get; set; }
    
    // Review Information
    public int? ReviewedByUserId { get; set; }
    public string? ReviewedByName { get; set; }          // Snapshot
    public DateTime? ReviewedAt { get; set; }
    public string? RejectReason { get; set; }
    
    // Assigned Room (After Approval)
    public int? AssignedRoomId { get; set; }
    public string? AssignedRoomNumber { get; set; }      // Snapshot
    public string? AssignedBuildingName { get; set; }    // Snapshot
    
    // Cancellation
    public DateTime? CancelledAt { get; set; }
    public string? CancelledReason { get; set; }
    public bool CancelledBySelf { get; set; }
    
    // Navigation
    public Contract? Contract { get; set; }
}
```

#### Contract (ContractStudentDb)

```csharp
public class Contract
{
    public int Id { get; set; }
    
    // References
    public int StudentId { get; set; }
    public Student Student { get; set; }
    public int ApplicationId { get; set; }
    public RoomApplication Application { get; set; }
    
    // Room Information (Snapshot Data)
    public int RoomId { get; set; }
    public string RoomNumber { get; set; }               // Snapshot
    public int BuildingId { get; set; }
    public string BuildingName { get; set; }             // Snapshot
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; }             // Snapshot
    
    // Contract Details
    public string ContractCode { get; set; }             // HD{YEAR}{SEQUENCE}
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public decimal MonthlyRent { get; set; }             // Snapshot
    public decimal DepositAmount { get; set; }
    public bool IsDepositPaid { get; set; }
    public DateTime? DepositPaidAt { get; set; }
    
    // Rates (Snapshot Data)
    public decimal ElectricityRate { get; set; }
    public decimal WaterRate { get; set; }
    public int PaymentDueDay { get; set; }
    
    // Signing Information
    public string? WitnessName { get; set; }
    public string? WitnessTitle { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? SignedImageUrl { get; set; }
    
    // Status Management
    public string Status { get; set; }                   // Pending, Active, Terminated, Expired
    public DateTime? TerminatedAt { get; set; }
    public string? TerminatedReason { get; set; }
    public int? TerminatedByUserId { get; set; }
    
    // Deposit Return
    public decimal? DepositReturnedAmount { get; set; }
    public DateTime? DepositReturnedAt { get; set; }
    public string? DepositDeductionReason { get; set; }
    
    // Audit
    public int CreatedByUserId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### User (BillingMaintenanceDb)

```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Role { get; set; }                     // Admin, Staff, Student
    public string? AvatarUrl { get; set; }
    
    // Student Reference
    public int? StudentId { get; set; }
    public string? StudentCode { get; set; }
    
    // Account Status
    public bool IsActive { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // JWT
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}
```

#### BuildingAssignment (BillingMaintenanceDb)

```csharp
public class BuildingAssignment
{
    public int Id { get; set; }
    public int UserId { get; set; }                      // Staff member
    public int BuildingId { get; set; }                  // Building reference
    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }             // Admin who created
}
```

### Supporting Entities

#### Student (ContractStudentDb)

```csharp
public class Student
{
    public int Id { get; set; }
    public string StudentCode { get; set; }
    public string FullName { get; set; }
    public string Faculty { get; set; }
    public string Major { get; set; }
    public int AcademicYear { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int? UserId { get; set; }                     // Reference to User in BillingMaintenanceDb
    public bool IsActive { get; set; }
    
    // Navigation
    public ICollection<RoomApplication> Applications { get; set; }
    public ICollection<Contract> Contracts { get; set; }
}
```

#### Room (RoomBuildingDb)

```csharp
public class Room
{
    public int Id { get; set; }
    public string RoomNumber { get; set; }
    public int FloorId { get; set; }
    public Floor Floor { get; set; }
    public int RoomTypeId { get; set; }
    public RoomType RoomType { get; set; }
    public string Status { get; set; }                   // Available, Occupied, Maintenance, Locked
    public int CurrentOccupants { get; set; }
    public int MaxOccupants { get; set; }
    public bool IsLocked { get; set; }
    public string? LockReason { get; set; }
    public DateTime? AvailableFrom { get; set; }
}
```

## Business Logic

### Application Creation Logic

```csharp
public async Task<RoomApplicationDto> CreateApplication(CreateRoomApplicationDto dto)
{
    // 1. Validate required fields
    ValidateRequiredFields(dto);
    
    // 2. Validate student exists
    var student = await _studentRepository.GetByIdAsync(dto.StudentId);
    if (student == null)
        throw new NotFoundException("Student not found");
    
    // 3. Check for existing active applications
    var existingActive = await _applicationRepository
        .GetActiveApplicationsByStudentAsync(dto.StudentId);
    if (existingActive.Any())
        throw new BusinessRuleException("Student already has an active application");
    
    // 4. Validate room availability (if specific room selected)
    if (dto.RoomId.HasValue)
    {
        var room = await _roomService.GetRoomAsync(dto.RoomId.Value);
        if (room.Status != "Available" || room.CurrentOccupants >= room.MaxOccupants)
            throw new BusinessRuleException("Selected room is not available");
    }
    
    // 5. Set priority based on IsLocalStudent
    int priority = dto.IsLocalStudent ? 0 : 1;
    
    // 6. Create application entity
    var application = new RoomApplication
    {
        StudentId = dto.StudentId,
        PreferredBuildingId = dto.PreferredBuildingId,
        PreferredBuildingName = dto.PreferredBuildingName,  // Snapshot
        PreferredRoomTypeId = dto.PreferredRoomTypeId,
        PreferredRoomTypeName = dto.PreferredRoomTypeName,  // Snapshot
        PreferredRoomPrice = dto.PreferredRoomPrice,        // Snapshot
        RequestedStartDate = dto.RequestedStartDate,
        RequestedEndDate = dto.RequestedEndDate,
        SpecialRequirements = dto.SpecialRequirements,
        Note = dto.Note,
        IsLocalStudent = dto.IsLocalStudent,
        Priority = priority,
        Status = "Pending",
        CreatedAt = DateTime.UtcNow
    };
    
    // 7. Save to database
    await _applicationRepository.AddAsync(application);
    
    // 8. Send notification
    await _notificationService.SendApplicationConfirmationAsync(application);
    
    return MapToDto(application);
}
```

### Application Approval Logic

```csharp
public async Task<RoomApplicationDto> ApproveApplication(
    int applicationId, 
    int userId, 
    string userName,
    int assignedRoomId)
{
    // 1. Get application
    var application = await _applicationRepository.GetByIdAsync(applicationId);
    if (application == null)
        throw new NotFoundException("Application not found");
    
    // 2. Validate application status
    if (application.Status != "Pending" && application.Status != "UnderReview")
        throw new BusinessRuleException("Application cannot be approved in current status");
    
    // 3. Authorize user (building-level check)
    await _authorizationService.AuthorizeApplicationActionAsync(
        userId, 
        application.PreferredBuildingId);
    
    // 4. Validate assigned room
    var room = await _roomService.GetRoomAsync(assignedRoomId);
    if (room.Status != "Available" || room.CurrentOccupants >= room.MaxOccupants)
        throw new BusinessRuleException("Assigned room is not available");
    
    // 5. Update application
    application.Status = "Approved";
    application.ReviewedByUserId = userId;
    application.ReviewedByName = userName;  // Snapshot
    application.ReviewedAt = DateTime.UtcNow;
    application.AssignedRoomId = assignedRoomId;
    application.AssignedRoomNumber = room.RoomNumber;        // Snapshot
    application.AssignedBuildingName = room.Building.Name;   // Snapshot
    
    // 6. Save changes
    await _applicationRepository.UpdateAsync(application);
    
    // 7. Send notification to student
    await _notificationService.SendApplicationApprovedAsync(application);
    
    return MapToDto(application);
}
```

### Application Rejection Logic

```csharp
public async Task<RoomApplicationDto> RejectApplication(
    int applicationId,
    int userId,
    string userName,
    string rejectReason)
{
    // 1. Validate reject reason provided
    if (string.IsNullOrWhiteSpace(rejectReason))
        throw new ValidationException("Reject reason is required");
    
    // 2. Get application
    var application = await _applicationRepository.GetByIdAsync(applicationId);
    if (application == null)
        throw new NotFoundException("Application not found");
    
    // 3. Validate application status
    if (application.Status != "Pending" && application.Status != "UnderReview")
        throw new BusinessRuleException("Application cannot be rejected in current status");
    
    // 4. Authorize user (building-level check)
    await _authorizationService.AuthorizeApplicationActionAsync(
        userId,
        application.PreferredBuildingId);
    
    // 5. Update application
    application.Status = "Rejected";
    application.ReviewedByUserId = userId;
    application.ReviewedByName = userName;  // Snapshot
    application.ReviewedAt = DateTime.UtcNow;
    application.RejectReason = rejectReason;
    
    // 6. Save changes
    await _applicationRepository.UpdateAsync(application);
    
    // 7. Send notification to student
    await _notificationService.SendApplicationRejectedAsync(application);
    
    return MapToDto(application);
}
```

### Contract Creation Logic (On Student Acceptance)

```csharp
public async Task<ContractDto> AcceptApprovedApplication(int applicationId, int studentId)
{
    // 1. Get application
    var application = await _applicationRepository.GetByIdAsync(applicationId);
    if (application == null)
        throw new NotFoundException("Application not found");
    
    // 2. Validate ownership
    if (application.StudentId != studentId)
        throw new UnauthorizedException("Application does not belong to this student");
    
    // 3. Validate application status
    if (application.Status != "Approved")
        throw new BusinessRuleException("Application must be approved before acceptance");
    
    // 4. Check for existing active contracts
    var existingActive = await _contractRepository
        .GetActiveContractsByStudentAsync(studentId);
    if (existingActive.Any())
        throw new BusinessRuleException("Student already has an active contract");
    
    // 5. Get system settings for default rates
    var settings = await _settingsService.GetSystemSettingsAsync();
    
    // 6. Generate contract code: HD{YEAR}{SEQUENCE}
    var year = DateTime.UtcNow.Year;
    var sequence = await _contractRepository.GetNextSequenceForYearAsync(year);
    var contractCode = $"HD{year}{sequence:D4}";
    
    // 7. Create contract entity
    var contract = new Contract
    {
        StudentId = application.StudentId,
        ApplicationId = application.Id,
        RoomId = application.AssignedRoomId.Value,
        RoomNumber = application.AssignedRoomNumber,        // Snapshot
        BuildingId = application.PreferredBuildingId,
        BuildingName = application.AssignedBuildingName,   // Snapshot
        RoomTypeId = application.PreferredRoomTypeId,
        RoomTypeName = application.PreferredRoomTypeName,  // Snapshot
        ContractCode = contractCode,
        StartDate = application.RequestedStartDate,
        EndDate = application.RequestedEndDate,
        MonthlyRent = application.PreferredRoomPrice,      // Snapshot
        DepositAmount = settings.DefaultDepositAmount,
        ElectricityRate = settings.DefaultElectricityRate,
        WaterRate = settings.DefaultWaterRate,
        PaymentDueDay = settings.DefaultPaymentDueDay,
        Status = "Pending",
        CreatedByUserId = studentId,  // Self-created
        CreatedAt = DateTime.UtcNow
    };
    
    try
    {
        // 8. Save contract
        await _contractRepository.AddAsync(contract);
        
        // 9. Link contract to application
        application.Contract = contract;
        await _applicationRepository.UpdateAsync(application);
        
        // 10. Send notification
        await _notificationService.SendContractCreatedAsync(contract);
        
        return MapToDto(contract);
    }
    catch (Exception ex)
    {
        // 11. On error: log, notify admin, keep application as Approved
        _logger.LogError(ex, "Failed to create contract for application {ApplicationId}", applicationId);
        await _notificationService.NotifyAdminContractCreationFailedAsync(application, ex.Message);
        throw;
    }
}
```

### Authorization Service

```csharp
public class AuthorizationService
{
    public async Task<bool> AuthorizeApplicationActionAsync(int userId, int buildingId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        // Admin has full access
        if (user.Role == "Admin")
            return true;
        
        // Staff must have building assignment
        if (user.Role == "Staff")
        {
            var assignments = await _buildingAssignmentRepository
                .GetByUserIdAsync(userId);
            
            if (assignments.Any(a => a.BuildingId == buildingId))
                return true;
            
            throw new UnauthorizedException(
                "Staff member does not have permission for this building");
        }
        
        throw new UnauthorizedException("User does not have permission");
    }
    
    public async Task<List<int>> GetAuthorizedBuildingIdsAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        // Admin sees all buildings
        if (user.Role == "Admin")
            return await _buildingRepository.GetAllBuildingIdsAsync();
        
        // Staff sees only assigned buildings
        if (user.Role == "Staff")
        {
            var assignments = await _buildingAssignmentRepository
                .GetByUserIdAsync(userId);
            return assignments.Select(a => a.BuildingId).ToList();
        }
        
        return new List<int>();
    }
}
```

## Error Handling

### Exception Types

```csharp
// Domain exceptions
public class NotFoundException : Exception { }
public class ValidationException : Exception { }
public class BusinessRuleException : Exception { }
public class UnauthorizedException : Exception { }

// Application-specific exceptions
public class DuplicateApplicationException : BusinessRuleException { }
public class RoomNotAvailableException : BusinessRuleException { }
public class ContractCreationException : Exception { }
```

### Error Response Format

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": [
      {
        "field": "RequestedStartDate",
        "message": "Start date is required"
      }
    ]
  }
}
```

## Security Considerations

### Authentication
- JWT-based authentication with refresh tokens
- Passwords hashed with BCrypt
- Token expiry: 15 minutes (access), 7 days (refresh)

### Authorization
- Role-based access control (Admin, Staff, Student)
- Building-level permissions for Staff members
- Endpoint-level authorization decorators

### Data Protection
- Snapshot data prevents tampering with historical records
- Soft delete for audit trail
- Sensitive data (passwords) never logged or exposed

### Input Validation
- Required field validation
- Data type and format validation
- Business rule validation
- SQL injection prevention via parameterized queries

## Performance Considerations

### Database Indexing

```sql
-- RoomApplication indexes
CREATE INDEX IX_RoomApplication_StudentId ON RoomApplications(StudentId);
CREATE INDEX IX_RoomApplication_Status ON RoomApplications(Status);
CREATE INDEX IX_RoomApplication_PreferredBuildingId ON RoomApplications(PreferredBuildingId);
CREATE INDEX IX_RoomApplication_CreatedAt ON RoomApplications(CreatedAt DESC);

-- Contract indexes
CREATE INDEX IX_Contract_StudentId ON Contracts(StudentId);
CREATE INDEX IX_Contract_ApplicationId ON Contracts(ApplicationId);
CREATE INDEX IX_Contract_Status ON Contracts(Status);
CREATE INDEX IX_Contract_RoomId ON Contracts(RoomId);

-- BuildingAssignment indexes
CREATE INDEX IX_BuildingAssignment_UserId ON BuildingAssignments(UserId);
CREATE INDEX IX_BuildingAssignment_BuildingId ON BuildingAssignments(BuildingId);

-- Room indexes
CREATE INDEX IX_Room_Status ON Rooms(Status);
CREATE INDEX IX_Room_BuildingId_FloorId ON Rooms(BuildingId, FloorId);
```

### Caching Strategy
- Cache building assignments on user login
- Cache room availability for 1 minute
- Cache system settings for 5 minutes

### Query Optimization
- Use pagination for list endpoints (default 20 items per page)
- Eager loading for related entities
- Projection to DTOs to minimize data transfer

## Testing Strategy

### Unit Tests
- Business logic validation
- Authorization rules
- Data transformation and mapping
- Error handling scenarios

### Property-Based Tests
- Application creation with various valid inputs
- Authorization checks across different user/building combinations
- Contract creation from approved applications
- Data integrity constraints

### Integration Tests
- End-to-end application workflow
- Cross-service communication via API Gateway
- Database transactions and rollback
- Notification delivery

### Example-Based Tests
- Specific UI rendering scenarios
- Concurrent application submissions
- Error recovery scenarios

## Correctness Properties

*A property is a characteristic or behavior that should hold true across all valid executions of a system—essentially, a formal statement about what the system should do. Properties serve as the bridge between human-readable specifications and machine-verifiable correctness guarantees.*

### Property 1: Application Creation Stores Complete Data

*For any* valid room application submission with all required fields (StudentId, PreferredBuildingId, PreferredRoomTypeId, RequestedStartDate, RequestedEndDate), the Room_Application_System SHALL create a RoomApplication entity with status "Pending" and store all provided fields including room details, dates, special requirements, and priority.

**Validates: Requirements 1.3, 1.4, 1.8**

### Property 2: Missing Required Fields Rejected

*For any* room application submission missing one or more required fields (StudentId, PreferredBuildingId, PreferredRoomTypeId, RequestedStartDate, RequestedEndDate), the Room_Application_System SHALL reject the submission and return validation error messages identifying the missing fields.

**Validates: Requirements 1.5**

### Property 3: Unavailable Room Applications Rejected

*For any* room application for a room that is not Available (status other than "Available" or CurrentOccupants >= MaxOccupants), the Room_Application_System SHALL reject the submission and display an error message.

**Validates: Requirements 1.6, 6.6**

### Property 4: Room Display Shows Complete Information

*For any* selected room, the Room_Application_System SHALL display all required details including room number, building name, capacity, current occupants, available slots, amenities, and monthly rent.

**Validates: Requirements 1.2, 6.2**

### Property 5: Staff Authorization Enforces Building Restrictions

*For any* Staff member attempting to approve, reject, or edit a RoomApplication for a building not in their BuildingAssignment, the Authorization_System SHALL deny the action and display an authorization error message.

**Validates: Requirements 2.3, 5.6**

### Property 6: Staff Can View All Applications

*For any* Staff member, the Authorization_System SHALL grant view access to all RoomApplications, Contracts, and maintenance requests across all buildings, regardless of BuildingAssignment.

**Validates: Requirements 2.2, 5.1**

### Property 7: Admin Bypasses Building Restrictions

*For any* RoomApplication and Admin user, the Authorization_System SHALL allow all approve, reject, and edit actions regardless of building.

**Validates: Requirements 2.4**

### Property 8: UI Indicates Manageable Applications

*For any* Staff member viewing the application list, the Room_Application_System SHALL display visual indicators distinguishing between applications they can manage (building in their BuildingAssignment) and applications outside their authorization.

**Validates: Requirements 2.6, 5.5**

### Property 9: Approval Updates Status and Records Reviewer

*For any* authorized user approving a RoomApplication with a valid assigned room, the Room_Application_System SHALL update the status to "Approved", record the ReviewedByUserId, ReviewedByName, ReviewedAt timestamp, and store AssignedRoomId, AssignedRoomNumber, and AssignedBuildingName.

**Validates: Requirements 3.1, 3.2, 3.3**

### Property 10: Rejection Requires Reason

*For any* rejection attempt without a RejectReason, the Room_Application_System SHALL reject the operation. *For any* rejection with a RejectReason, the system SHALL update status to "Rejected", store the reason, and record ReviewedByUserId, ReviewedByName, and ReviewedAt.

**Validates: Requirements 3.4, 3.5, 3.6**

### Property 11: Approval Without Room Assignment Rejected

*For any* approval attempt without a valid AssignedRoomId, the Room_Application_System SHALL reject the approval and display an error message.

**Validates: Requirements 3.8**

### Property 12: Approved Applications Are Immutable Until Student Response

*For any* RoomApplication with status "Approved", the Room_Application_System SHALL prevent status changes by staff until the student accepts or the application is cancelled.

**Validates: Requirements 3.9**

### Property 13: Acceptance Creates Contract With Complete Data

*For any* RoomApplication with status "Approved", when a student accepts it, the Contract_System SHALL automatically create a Contract entity with all fields populated from the application (StudentId, ApplicationId, RoomId, RoomNumber, BuildingId, BuildingName, RoomTypeId, RoomTypeName, StartDate, EndDate, MonthlyRent), generate a unique ContractCode in format "HD{YEAR}{SEQUENCE}", set default rates, set status to "Pending", and establish bidirectional reference between Contract and Application.

**Validates: Requirements 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8, 4.10**

### Property 14: Room Filtering By Building

*For any* building filter selection, the Room_Application_System SHALL display only rooms where the room's BuildingId matches the selected building.

**Validates: Requirements 6.3**

### Property 15: Room Filtering By Room Type

*For any* room type filter selection, the Room_Application_System SHALL display only rooms where the room's RoomTypeId matches the selected room type or the room's capacity/amenities match the filter criteria.

**Validates: Requirements 6.4**

### Property 16: Available Rooms Filter

*For any* room set displayed to students, the Room_Application_System SHALL show only rooms with status "Available".

**Validates: Requirements 6.1**

### Property 17: Application Status Validity

*For any* RoomApplication entity, the status SHALL be one of the valid values: "Pending", "UnderReview", "Approved", "Rejected", or "Cancelled".

**Validates: Requirements 7.1**

### Property 18: Application Display Shows Required Fields

*For any* student viewing their applications, the Room_Application_System SHALL display current status, submission date (CreatedAt), preferred room information, and review information (ReviewedByName, ReviewedAt, RejectReason, AssignedRoomNumber) when available.

**Validates: Requirements 7.2, 7.3, 7.4**

### Property 19: Cancellation Updates All Fields

*For any* pending RoomApplication cancelled by a student, the Room_Application_System SHALL update status to "Cancelled", record CancelledAt timestamp, store CancelledReason, and set CancelledBySelf to true.

**Validates: Requirements 7.6**

### Property 20: Building Assignment Creation and Retrieval

*For any* Admin user creating a BuildingAssignment with a valid Staff UserId and BuildingId, the Authorization_System SHALL record the assignment and allow the Staff member to perform approval actions on applications for that building.

**Validates: Requirements 8.1, 8.2**

### Property 21: Building Assignment Removal Revokes Permissions

*For any* BuildingAssignment removal, the Authorization_System SHALL revoke the Staff member's approval permissions for that building, preventing subsequent approval actions.

**Validates: Requirements 8.3**

### Property 22: Multiple Building Assignments Allowed

*For any* Staff member, the Authorization_System SHALL allow assignment to multiple buildings, and the staff member SHALL be authorized to act on applications for any of their assigned buildings.

**Validates: Requirements 8.4**

### Property 23: Non-Staff Building Assignment Rejected

*For any* user with Role other than "Staff", the Authorization_System SHALL reject building assignment attempts and display an error message.

**Validates: Requirements 8.7**

### Property 24: Application Snapshot Data Immutability

*For any* RoomApplication, the snapshot fields (PreferredBuildingName, PreferredRoomTypeName, PreferredRoomPrice, AssignedRoomNumber, AssignedBuildingName, ReviewedByName) SHALL remain unchanged even when the corresponding master data (Building, RoomType, Room, User) is modified.

**Validates: Requirements 9.1**

### Property 25: Contract Snapshot Data Immutability

*For any* Contract, the snapshot fields (RoomNumber, BuildingName, RoomTypeName, MonthlyRent) SHALL remain unchanged even when the corresponding master data (Room, Building, RoomType) is modified.

**Validates: Requirements 9.2**

### Property 26: Student Reference Validation

*For any* RoomApplication creation attempt with a StudentId that does not reference an existing Student record, the Room_Application_System SHALL reject the creation and return a validation error.

**Validates: Requirements 9.3**

### Property 27: Application Reference Validation

*For any* Contract creation attempt with an ApplicationId that does not reference an existing RoomApplication record, the Contract_System SHALL reject the creation and return a validation error.

**Validates: Requirements 9.4**

### Property 28: Single Active Application Per Student

*For any* student with an existing RoomApplication having status "Pending" or "Approved", the Room_Application_System SHALL reject attempts to create additional applications with these statuses and return an error message.

**Validates: Requirements 9.5**

### Property 29: Single Active Contract Per Student

*For any* student with an existing Contract having status "Active", the Contract_System SHALL reject attempts to create additional contracts with "Active" status and return an error message.

**Validates: Requirements 9.6**

### Property 30: Local Student Priority Assignment

*For any* RoomApplication where IsLocalStudent is true, the Room_Application_System SHALL set Priority to 0.

**Validates: Requirements 10.1**

### Property 31: Non-Local Student Priority Assignment

*For any* RoomApplication where IsLocalStudent is false, the Room_Application_System SHALL set Priority to 1.

**Validates: Requirements 10.2**

### Property 32: Application Sorting By Priority

*For any* list of RoomApplications sorted by Priority, applications with higher priority values SHALL appear first in the sorted list (non-local students with Priority=1 before local students with Priority=0 when sorted descending).

**Validates: Requirements 10.3, 10.5**

### Property 33: Manual Priority Adjustment

*For any* authorized user adjusting the Priority value of a RoomApplication, the Room_Application_System SHALL persist the new priority value and use it for subsequent sorting operations.

**Validates: Requirements 10.4**

## Non-Functional Requirements

### Scalability
- System should support up to 10,000 concurrent students
- Application processing throughput: 100 applications per minute
- Contract creation: 50 contracts per minute

### Availability
- Target uptime: 99.9% (excluding planned maintenance)
- Maximum planned downtime: 4 hours per month

### Response Time
- API response time: < 500ms for 95% of requests
- Page load time: < 2 seconds
- Database query time: < 100ms for indexed queries

### Data Retention
- Active applications: Indefinite
- Rejected/Cancelled applications: 2 years
- Active contracts: Indefinite
- Terminated contracts: 7 years (legal requirement)
- Audit logs: 5 years

## Deployment Architecture

### Development Environment
- Local development with .NET 9.0
- SQL Server Express or Docker SQL Server
- Vue.js dev server with hot reload

### Production Environment
- Azure App Service or IIS
- Azure SQL Database with geo-replication
- Azure CDN for static assets
- Application Insights for monitoring
- Azure Key Vault for secrets management

### CI/CD Pipeline
- GitHub Actions for automated builds
- Automated testing on pull requests
- Staging environment for pre-production validation
- Blue-green deployment for zero-downtime releases

## Future Enhancements

### Phase 2 Features
- Real-time availability updates via SignalR
- Mobile app for students
- Automated waitlist management
- Room inspection integration with applications
- Payment integration with contract creation

### Phase 3 Features
- AI-powered room recommendations
- Roommate matching system
- Virtual room tours
- Automated contract renewal
- Advanced analytics dashboard

## References

### External Documentation
- ASP.NET Core Documentation: https://docs.microsoft.com/aspnet/core
- Entity Framework Core: https://docs.microsoft.com/ef/core
- Vue.js Documentation: https://vuejs.org/guide
- Ant Design Vue: https://antdv.com/components/overview

### Related Documents
- System Requirements Document: requirements.md
- API Documentation: Available via Swagger at /swagger
- Database Schema: Available in EF Core migrations
- User Guide: To be created in Phase 2

## Appendix

### Status Values

#### RoomApplication.Status
- **Pending**: Initial state after submission
- **UnderReview**: Staff is reviewing the application
- **Approved**: Staff approved, awaiting student acceptance
- **Rejected**: Staff rejected the application
- **Cancelled**: Student or staff cancelled

#### Contract.Status
- **Pending**: Created but not yet signed
- **Active**: Signed and currently in effect
- **Terminated**: Ended before EndDate
- **Expired**: Ended naturally at EndDate

#### Room.Status
- **Available**: Room has vacant slots
- **Occupied**: Room is fully occupied
- **Maintenance**: Room is under maintenance
- **Locked**: Room is locked for administrative reasons

### User Roles

#### Admin
- Full system access
- Can approve/reject applications for all buildings
- Can manage users and building assignments
- Can view all data and modify all records

#### Staff
- Can view all applications and contracts
- Can approve/reject/edit only for assigned buildings
- Can manage room data for assigned buildings
- Cannot modify user accounts or building assignments

#### Student
- Can view and apply for available rooms
- Can view their own applications and contracts
- Can accept approved applications
- Can cancel their pending applications
- Cannot view other students' data

### Validation Rules

#### RoomApplication Validation
- StudentId: Must reference existing student
- RequestedStartDate: Must be in the future
- RequestedEndDate: Must be after RequestedStartDate
- RequestedEndDate - RequestedStartDate: Must be at least 30 days
- PreferredBuildingId: Must reference existing building
- PreferredRoomTypeId: Must reference existing room type

#### Contract Validation
- ApplicationId: Must reference existing approved application
- StartDate: Must match application RequestedStartDate
- EndDate: Must match application RequestedEndDate
- MonthlyRent: Must be greater than 0
- DepositAmount: Must be greater than or equal to 0
- ContractCode: Must be unique

#### BuildingAssignment Validation
- UserId: Must reference existing user with Role="Staff"
- BuildingId: Must reference existing building
- Combination (UserId, BuildingId): Must be unique
