using CityBuilder.Domain.Entities;

namespace CityBuilder.Application.Interfaces
{
    public interface IGridService
    {
        bool IsCellFree(GridPosition pos);
        (float worldX, float worldY) CellToWorld(GridPosition pos);
    }
}
