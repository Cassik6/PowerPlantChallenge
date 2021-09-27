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
        
        [JsonPropertyName("load")]
        public double NeededLoad { get; set; }
        [JsonPropertyName("fuels")]
        public FuelPrices FuelPrices { get; set; }

        [JsonPropertyName("powerplants")]
        public List<Powerplant> Powerplants { get; set; }


        public void UpdatePowerplantsData()
        {
            Powerplants = Powerplants.Select(
                powerplant => Powerplant.Create(FuelPrices, powerplant.Name, powerplant.Type, powerplant.Efficiency, powerplant.PMax, powerplant.PMin)
                ).ToList();
        }
    }
}
