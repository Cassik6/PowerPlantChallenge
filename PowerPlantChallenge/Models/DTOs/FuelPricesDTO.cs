using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Models.DTOs
{
    public class FuelPricesDTO
    {
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
