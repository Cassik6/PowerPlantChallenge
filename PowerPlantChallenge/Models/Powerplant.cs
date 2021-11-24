using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace PowerPlantChallenge.Models
{

    public class PowerPlant
    {
        public string Name { get; set; }
        public PowerPlantType Type { get; set; }
        public double Efficiency { get; set; }
        public double PMax { get; set; }
        public double EffectivePMax { get; set; }
        public double PMin { get; set; }
        public double CostPerUnit { get; set; }

        public static PowerPlant Create(FuelPrices fuelPrice, string name, PowerPlantType type, double efficiency, double pMax, double pMin)
        {
            var powerPlant = new PowerPlant
            {
                Efficiency = efficiency,
                Name = name,
                PMax = pMax,
                PMin = pMin,
                Type = type,
                EffectivePMax = CalculateEffectivePMax(fuelPrice.Wind, pMax, type)
            };

            if (powerPlant.Type == PowerPlantType.Windturbine) 
                powerPlant.PMin = powerPlant.EffectivePMax;

            powerPlant.CostPerUnit = type switch
            {
                PowerPlantType.Gasfired => CalculatePowerCostGasFired(fuelPrice, efficiency),
                PowerPlantType.Turbojet => CalculatePowerCostTurboJet(fuelPrice, efficiency),
                PowerPlantType.Windturbine => 0,
                _ => throw new System.ArgumentNullException("Powerplant Type is not set on this object"),
            };
            return powerPlant;
        }

        private static double CalculateEffectivePMax(double windStrenght, double pMax, PowerPlantType type)
        {
            return Math.Round(type == PowerPlantType.Windturbine ? windStrenght * pMax / 100 : pMax, 1);
        }

        private static double CalculatePowerCostGasFired(FuelPrices fuelPrice, double efficiency)
        {
            return fuelPrice.Gas / efficiency + 0.3 * fuelPrice.Co2;
            
        }

        private static double CalculatePowerCostTurboJet(FuelPrices fuelPrices, double efficiency)
        {
            return fuelPrices.Kerosine / efficiency;
        }
    }
}