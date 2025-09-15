using CityBuilder.Domain.Entities;
using System;

namespace CityBuilder.Application.DTOs
{
    [Serializable]
    public class BuildingDTO
    {
        public string Id = string.Empty;
        public BuildingType Type;
        public int X;
        public int Y;
        public int Level;
    }
}