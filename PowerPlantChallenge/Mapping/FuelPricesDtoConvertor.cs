﻿using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Mapping
{
    public static class FuelPricesDtoConvertor
    {
        public static FuelPrices Map(FuelPricesDto dto)
        {
            return new FuelPrices
            {
                Co2 = dto.Co2,
                Gas = dto.Gas,
                Kerosine = dto.Kerosine,
                Wind = dto.Wind
            };
        }
    }
}
