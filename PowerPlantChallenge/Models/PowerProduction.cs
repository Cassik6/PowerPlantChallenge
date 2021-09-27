using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Models
{
    public class PowerProduction
    {
        [JsonPropertyName("name")]
        public string PowerplantName { get; set; }
        [JsonPropertyName("p")]
        public double PowerGenerated { get; set; }

        [JsonIgnore]
        public double CostPerUnit { get; set; }

        public PowerProduction(string powerplantName, double powerGenerated, double costPerUnit)
        {
            PowerplantName = powerplantName;
            PowerGenerated = powerGenerated;
            CostPerUnit = costPerUnit;
        }

        

        
        
    }
}
