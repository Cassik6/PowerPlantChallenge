using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PowerPlantChallenge.Models
{

    public class Powerplant
    {
        // TODO : double decimal??
        [JsonPropertyName( "name")]
        public string Name { get; set; }
        [JsonPropertyName( "type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PowerplantType Type { get; set; }

        [JsonPropertyName( "efficiency")]
        public double Efficiency { get; set; }
        [JsonPropertyName( "pmax")]
        public double PMax { get; set; }
        [IgnoreDataMember]
        public double EffectivePMax { get; set; }
        [JsonPropertyName( "pmin")]
        public double PMin { get; set; }
        [IgnoreDataMember]
        public double CostPerUnit { get; set; }

        public static Powerplant Create(FuelPrices fuelPrice, string name, PowerplantType type, double efficiency, double pMax, double pMin)
        {
            var powerplant = new Powerplant
            {
                Efficiency = efficiency,
                Name = name,
                PMax = pMax,
                PMin = pMin,
                Type = type,
                EffectivePMax = CalculateEffectivePMax(fuelPrice.Wind, pMax, type)
            };

            powerplant.CostPerUnit = type switch
            {
                PowerplantType.Gasfired => CalculatePowerCostGasFired(fuelPrice, efficiency),
                PowerplantType.Turbojet => CalculatePowerCostTurboJet(fuelPrice, efficiency),
                PowerplantType.Windturbine => 0,
                _ => throw new System.ArgumentNullException("Powerplant Type is not set on this object"),
            };
            return powerplant;
        }

        private static double CalculateEffectivePMax(double windStrenght, double pMax, PowerplantType type)
        {
            return type == PowerplantType.Windturbine ? windStrenght * pMax / 100 : pMax;
        }

        private static double CalculatePowerCostGasFired(FuelPrices fuelPrice, double efficiency)
        {
            return fuelPrice.Gas / efficiency + 0.3 * fuelPrice.CO2;
            
        }

        private static double CalculatePowerCostTurboJet(FuelPrices fuelPrices, double efficiency)
        {
            return fuelPrices.Kerosine / efficiency;
        }
    }
}