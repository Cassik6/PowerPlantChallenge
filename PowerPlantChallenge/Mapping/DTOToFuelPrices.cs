using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Mapping
{
    public class DTOToFuelPrices
    {
        public static FuelPrices Map(FuelPricesDTO dto)
        {
            return new FuelPrices
            {
                CO2 = dto.CO2,
                Gas = dto.Gas,
                Kerosine = dto.Kerosine,
                Wind = dto.Wind
            };
        }
    }
}
