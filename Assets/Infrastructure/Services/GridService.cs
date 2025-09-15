using CityBuilder.Application.Interfaces;
using CityBuilder.Domain.Entities;

namespace CityBuilder.Infrastructure.Services
{
    public class GridService : IGridService
    {
        private readonly IBuildingRepository _repo;
        private readonly float _cellOffset = 0.5f;

        public GridService(IBuildingRepository repo) { _repo = repo; }
        public bool IsCellFree(GridPosition pos) => _repo.FindByPosition(pos) == null;
        public (float worldX, float worldY) CellToWorld(GridPosition pos) => (pos.X + _cellOffset, pos.Y + _cellOffset);
    }
}
