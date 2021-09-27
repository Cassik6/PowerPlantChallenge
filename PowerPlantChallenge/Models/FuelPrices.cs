using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PowerPlantChallenge.Models
{
    [DataContract]
    public class FuelPrices
    {
        // TODO : decimals / double???
        [JsonPropertyName("gas(euro/MWh)")]
        public double Gas { get; set; }
        [JsonPropertyName("kerosine(euro/MWh)")]
        public double Kerosine { get; set; }
        [JsonPropertyName("co2(euro/ton)")]
        public double CO2 { get; set; }
        [JsonPropertyName("wind(%)")]
        public double Wind { get; set; }

    }
}