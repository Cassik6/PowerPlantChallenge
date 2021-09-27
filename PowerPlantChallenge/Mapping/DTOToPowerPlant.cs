using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Mapping
{
    public class DTOToPowerPlant
    {
        public static Powerplant Map(PowerplantDTO dto)
        {
            return new Powerplant
            {
                Efficiency = dto.Efficiency,
                Name = dto.Name,
                PMax = dto.PMax,
                PMin = dto.PMin,
                Type = (PowerplantType) Enum.Parse(typeof(PowerplantType), dto.Type)
            };
        }
    }
}
