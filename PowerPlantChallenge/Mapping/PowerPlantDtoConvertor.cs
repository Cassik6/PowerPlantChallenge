using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Mapping
{
    public static class PowerPlantDtoConvertor
    {
        public static PowerPlant Map(PowerPlantDto dto)
        {
            return new PowerPlant
            {
                Efficiency = dto.Efficiency,
                Name = dto.Name,
                PMax = dto.PMax,
                PMin = dto.PMin,
                Type = (PowerPlantType) Enum.Parse(typeof(PowerPlantType), dto.Type, true)
            };
        }
    }
}
