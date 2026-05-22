using MediatR;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;
using RoomBuildingService.Domain.Enums;

namespace RoomBuildingService.Application.Buildings.Commands
{
    public record CreateBuildingCommand(string Name, string Description, int NumberOfFloors, BuildingType Type) : IRequest<Guid>;

    public class CreateBuildingHandler : IRequestHandler<CreateBuildingCommand, Guid>
    {
        private readonly IBuildingRepository _repository;
        public CreateBuildingHandler(IBuildingRepository repository) => _repository = repository;

        public async Task<Guid> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
        {
            var building = new Building
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                NumberOfFloors = request.NumberOfFloors,
                Type = request.Type
            };

            await _repository.AddAsync(building);
            return building.Id;
        }
    }
}
