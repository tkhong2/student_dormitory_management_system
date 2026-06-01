using MediatR;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.Application.Buildings.Commands
{
    public record CreateBuildingCommand(
        string Name,
        string Gender,
        int TotalFloors,
        int TotalRooms,
        string? Description
    ) : IRequest<int>;

    public class CreateBuildingHandler : IRequestHandler<CreateBuildingCommand, int>
    {
        private readonly IBuildingRepository _repository;
        public CreateBuildingHandler(IBuildingRepository repository) => _repository = repository;

        public async Task<int> Handle(CreateBuildingCommand request, CancellationToken cancellationToken)
        {
            var building = new Building
            {
                Name = request.Name,
                Gender = request.Gender,
                TotalFloors = request.TotalFloors,
                TotalRooms = request.TotalRooms,
                Description = request.Description
            };

            await _repository.AddAsync(building);
            return building.Id;
        }
    }
}
