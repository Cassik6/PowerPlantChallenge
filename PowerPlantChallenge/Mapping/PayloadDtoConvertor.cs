using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Mapping
{
    public class PayloadDtoConvertor
    {
        public static Payload Map(PayloadDto dto)
        {
            var payload = new Payload
            {
                FuelPrices = FuelPricesDtoConvertor.Map(dto.Fuels),
                NeededLoad = dto.Load,
                PowerPlants = dto.PowerPlants.Select(pow => PowerPlantDtoConvertor.Map(pow)).ToList()
            };      

            payload.UpdatePowerPlantsData();

            return payload;
        }
    }
}
