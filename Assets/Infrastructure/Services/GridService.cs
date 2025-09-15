using CityBuilder.Application.Interfaces;
using CityBuilder.Domain.Entities;

namespace CityBuilder.Infrastructure.Services
{
    public class GridService : IGridService
    {
        private readonly IBuildingRepository _repo;

        public GridService(IBuildingRepository repo) { _repo = repo; }
        public bool IsCellFree(GridPosition pos) => _repo.FindByPosition(pos) == null;
    }
}
