using System.Text.Json.Serialization;

namespace PowerPlantChallenge.Models
{
    public class PowerProduction
    {
        [JsonPropertyName("name")]
        public string PowerPlantName { get; set; }
        [JsonPropertyName("p")]
        public double PowerGenerated { get; set; }

        [JsonIgnore]
        public double CostPerUnit { get; set; }

        public PowerProduction(string powerPlantName, double powerGenerated, double costPerUnit)
        {
            PowerPlantName = powerPlantName;
            PowerGenerated = powerGenerated;
            CostPerUnit = costPerUnit;
        }
    }
}
