using System.Collections.Generic;

namespace PowerPlantChallenge.Models.Dtos
{
    public class PayloadDto
    {
        public double Load { get; set; }

        public FuelPricesDto Fuels { get; set; }

        public List<PowerPlantDto> PowerPlants { get; set; }

    }
}
