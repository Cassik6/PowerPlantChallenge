using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Models
{
    [DataContract]
    public class Payload
    {
        public double NeededLoad { get; set; }
        public FuelPrices FuelPrices { get; set; }
        public List<PowerPlant> PowerPlants { get; set; }

        public void UpdatePowerPlantsData()
        {
            PowerPlants = PowerPlants.Select(
                powerPlant => PowerPlant.Create(FuelPrices, powerPlant.Name, powerPlant.Type, powerPlant.Efficiency, powerPlant.PMax, powerPlant.PMin)
                ).ToList();
        }
    }
}
