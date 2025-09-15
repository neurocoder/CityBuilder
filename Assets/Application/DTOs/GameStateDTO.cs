using System.Collections.Generic;
using System;

namespace CityBuilder.Application.DTOs
{
    [Serializable]
    public class GameStateDTO
    {
        public List<BuildingDTO> Buildings = new();
        public int Gold;
    }
}