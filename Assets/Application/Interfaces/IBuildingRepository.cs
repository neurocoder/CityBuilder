using System;
using System.Collections.Generic;
using CityBuilder.Domain.Entities;

namespace CityBuilder.Application.Interfaces
{
    public interface IBuildingRepository
    {
        void Add(Building building);
        void Remove(Guid id);
        Building? FindByPosition(GridPosition pos);
        Building? FindById(Guid id);
        IEnumerable<Building> GetAll();
        void Clear();
    }
}