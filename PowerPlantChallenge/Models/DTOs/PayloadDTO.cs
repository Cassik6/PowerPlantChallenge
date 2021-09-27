using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Models.DTOs
{
    public class PayloadDTO
    {

        
        public double Load { get; set; }
        
        public FuelPricesDTO Fuels { get; set; }
        
        public List<PowerplantDTO> Powerplants { get; set; }

    }
}
