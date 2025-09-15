using System;
using CityBuilder.Domain.Entities;

namespace CityBuilder.Application.DTOs
{
    public class PlaceBuildingDTO
    {
        public BuildingType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class MoveBuildingDTO
    {
        public Guid BuildingId { get; set; }
        public int NewX { get; set; }
        public int NewY { get; set; }
    }

    public class RemoveBuildingDTO
    {
        public Guid BuildingId { get; set; }
    }

    public class UpgradeBuildingDTO
    {
        public Guid BuildingId { get; set; }
    }
}

