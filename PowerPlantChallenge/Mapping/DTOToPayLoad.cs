using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Mapping
{
    public class DTOToPayLoad
    {
        public static Payload Map(PayloadDTO dto)
        {
            var payload = new Payload
            {
                FuelPrices = DTOToFuelPrices.Map(dto.Fuels),
                NeededLoad = dto.Load,
                Powerplants = dto.Powerplants.Select(pow => DTOToPowerPlant.Map(pow)).ToList()
            };      

            payload.UpdatePowerplantsData();

            return payload;
        }
    }
}
