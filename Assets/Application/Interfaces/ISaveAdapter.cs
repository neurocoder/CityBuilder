namespace CityBuilder.Application.Interfaces
{
    using System.Collections.Generic;
    using CityBuilder.Application.DTOs;

    public interface ISaveAdapter
    {
        void Save(List<BuildingDTO> buildings);
        List<BuildingDTO> Load();
    }
}
