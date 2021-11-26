using PowerPlantChallenge.Models;
using PowerPlantChallenge.Services;
using System.Collections.Generic;
using Xunit;

namespace PowerPlantChallenge.Tests.Services
{
    public class CostEfficiencyServiceShould
    {

        private readonly CostEfficiencyCalculationService _calculationService; 
        public CostEfficiencyServiceShould()
        {
            _calculationService = new CostEfficiencyCalculationService();
        }

        [Fact]
        public void SkipWindTurbineIfNecessary()
        {

            var powerPlants = new List<PowerPlant>
                {
                    new ()
                    {
                        Efficiency = 1,
                        Name = "GasFiredEco",
                        PMax = 90,
                        PMin = 90,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 50,
                        EffectivePMax = 90
                    },
                    new ()
                    {
                         Efficiency = 0.9,
                        Name = "GasFiredEco2",
                        PMax = 10,
                        PMin = 10,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 500,
                        EffectivePMax = 10
                    },
                    new ()
                    {
                        Efficiency = 1,
                        Name = "WindPark",
                        PMax = 20,
                        PMin = 20,
                        Type = PowerPlantType.Windturbine,
                        CostPerUnit = 0,
                        EffectivePMax = 20
                    },
                    new ()
                    {
                        Efficiency = 1,
                        Name = "GasFiredToxic",
                        PMax = 80,
                        PMin = 0,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 1000000,
                        EffectivePMax = 80
                    }
                };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 100);

            Assert.Equal("GasFiredEco", result[0].PowerPlantName);
            Assert.Equal("GasFiredEco2", result[1].PowerPlantName);
            Assert.Equal(90, result[0].PowerGenerated);
            Assert.Equal(10, result[1].PowerGenerated);
        }
        [Fact]
        public void TakeMostEfficient()
        {

            var powerPlants = new List<PowerPlant>
                {
                    new ()
                    {
                        Efficiency = 1,
                        Name = "GasFiredEco",
                        PMax = 50,
                        PMin = 50,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 50,
                        EffectivePMax = 50
                    },
                    new ()
                    {
                        Efficiency = 0.9,
                        Name = "GasFiredEco2",
                        PMax = 10,
                        PMin = 10,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 500,
                        EffectivePMax = 10
                    },
                    new ()
                    {
                        Efficiency = 1,
                        Name = "WindPark",
                        PMax = 20,
                        PMin = 0,
                        Type = PowerPlantType.Windturbine,
                        CostPerUnit = 0,
                        EffectivePMax = 20
                    },
                    new ()
                    {
                        Efficiency = 1,
                        Name = "GasFiredToxic",
                        PMax = 80,
                        PMin = 0,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 1000000,
                        EffectivePMax = 80
                    }
                };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 100);

            Assert.Equal("WindPark", result[0].PowerPlantName);
            Assert.Equal("GasFiredEco", result[1].PowerPlantName);
            Assert.Equal("GasFiredEco2", result[2].PowerPlantName);
            Assert.Equal("GasFiredToxic", result[3].PowerPlantName);
            Assert.Equal(20, result[0].PowerGenerated);
            Assert.Equal(50, result[1].PowerGenerated);
            Assert.Equal(10, result[2].PowerGenerated);
            Assert.Equal(20, result[3].PowerGenerated);
        }
        [Fact]
        public void NotAlwayUseMaxProductionCapcity()
        {
            var powerPlants = new List<PowerPlant>
                {
                    new ()
                    {
                        Efficiency = 1,
                        Name = "GasFiredEco",
                        PMax = 45,
                        PMin = 40,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 50,
                        EffectivePMax = 45
                    },
                    new ()
                    {
                        Efficiency = 0.9,
                        Name = "GasFiredEco2",
                        PMax = 45,
                        PMin = 40,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 500,
                        EffectivePMax = 45
                    },
                    new ()
                    {
                        Efficiency = 1,
                        Name = "GasFiredEco3",
                        PMax = 20,
                        PMin = 20,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 5000,
                        EffectivePMax = 20
                    },
                    new ()
                    {
                        Efficiency = 1,
                        Name = "GasFiredToxic",
                        PMax = 80,
                        PMin = 0,
                        Type = PowerPlantType.Gasfired,
                        CostPerUnit = 1000000,
                        EffectivePMax = 80
                    }
                };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 100);

            Assert.Equal("GasFiredEco", result[0].PowerPlantName);
            Assert.Equal("GasFiredEco2", result[1].PowerPlantName);
            Assert.Equal("GasFiredEco3", result[2].PowerPlantName);
            Assert.Equal(40, result[0].PowerGenerated);
            Assert.Equal(40, result[1].PowerGenerated);
            Assert.Equal(20, result[2].PowerGenerated);
        }
        [Fact]
        public void ReturnNullIfNotEnoughEnergyCanBeProduced()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 100,
                pMin: 50),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas2",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 100,
                pMin: 50)

        };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 500);

            Assert.Null(result);
        }
        [Fact]
        public void ReturnNullIfTooMuchEnergyIsProduced()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 100,
                pMin: 50),

                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Wind1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 50,
                pMin: 0)

        };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 20);

            Assert.Null(result);
        }
        [Fact]
        public void ConsumeEnoughFromOneWindTurbine()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 100,
                pMin: 50),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Wind1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 50,
                pMin: 0)

        };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 25);

            Assert.Equal("Wind1", result[0].PowerPlantName);
            Assert.Equal(25, result[0].PowerGenerated);
        }
        [Fact]
        public void ConsumeWindAndGas()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Wind1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 50,
                pMin: 0)

        };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 50);

            Assert.Equal("Wind1", result[0].PowerPlantName);
            Assert.Equal(25, result[0].PowerGenerated);
            Assert.Equal("Gas1", result[1].PowerPlantName);
            Assert.Equal(25, result[1].PowerGenerated);
        }
        [Fact]
        public void ConsumeGasOnly()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Wind1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 50,
                pMin: 0)

        };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 20);

            Assert.Equal("Gas1", result[0].PowerPlantName);
            Assert.Equal(20, result[0].PowerGenerated);
        }
        [Fact]
        public void ConsumeMostEfficientGas()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas2",
                type: PowerPlantType.Gasfired,
                efficiency: 0.6,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas3",
                type: PowerPlantType.Gasfired,
                efficiency: 0.8,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas4",
                type: PowerPlantType.Gasfired,
                efficiency: 0.3,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas5",
                type: PowerPlantType.Gasfired,
                efficiency: 0.45,
                pMax: 100,
                pMin: 10)
            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 20);

            Assert.Equal("Gas3", result[0].PowerPlantName);
            Assert.Equal(20, result[0].PowerGenerated);

        }
        [Fact]
        public void ConsumeEveryGas()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas2",
                type: PowerPlantType.Gasfired,
                efficiency: 0.6,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas3",
                type: PowerPlantType.Gasfired,
                efficiency: 0.8,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas4",
                type: PowerPlantType.Gasfired,
                efficiency: 0.3,
                pMax: 100,
                pMin: 10),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas5",
                type: PowerPlantType.Gasfired,
                efficiency: 0.45,
                pMax: 100,
                pMin: 10)
            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 490);

            Assert.Equal("Gas3", result[0].PowerPlantName);
            Assert.Equal(100, result[0].PowerGenerated);
            Assert.Equal("Gas2", result[1].PowerPlantName);
            Assert.Equal(100, result[1].PowerGenerated);
            Assert.Equal("Gas1", result[2].PowerPlantName);
            Assert.Equal(100, result[2].PowerGenerated);
            Assert.Equal("Gas5", result[3].PowerPlantName);
            Assert.Equal(100, result[3].PowerGenerated);
            Assert.Equal("Gas4", result[4].PowerPlantName);
            Assert.Equal(90, result[4].PowerGenerated);

        }
        [Fact]
        public void SkipWindToUseGasWithPMin()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 200,
                pMin: 110),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas2",
                type: PowerPlantType.Gasfired,
                efficiency: 0.8,
                pMax: 150,
                pMin: 80),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Wind1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 50,
                pMin: 0),

            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 125);

            Assert.Equal("Wind1", result[0].PowerPlantName);
            Assert.Equal(25, result[0].PowerGenerated);
            Assert.Equal("Gas2", result[1].PowerPlantName);
            Assert.Equal(100, result[1].PowerGenerated);
        }
        [Fact]
        public void UseKerosine()
        {
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Kerosine1",
                type: PowerPlantType.Turbojet,
                efficiency: 0.5,
                pMax: 200,
                pMin: 0),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Gas1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.5,
                pMax: 200,
                pMin: 100),
                PowerPlant.Create(
                new FuelPrices { Co2 = 20, Wind = 50, Kerosine = 50, Gas = 15 },
                name: "Wind1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 150,
                pMin: 0),

            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 100);

            Assert.Equal("Wind1", result[0].PowerPlantName);
            Assert.Equal(75, result[0].PowerGenerated);
            Assert.Equal("Kerosine1", result[1].PowerPlantName);
            Assert.Equal(25, result[1].PowerGenerated);
        }
        [Fact]
        public void PassTrickyTest1()
        {
            var fuelPrices = new FuelPrices { Co2 = 0, Wind = 100, Kerosine = 50.8, Gas = 20 };
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "windpark1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 20,
                pMin: 0),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfired",
                type: PowerPlantType.Gasfired,
                efficiency: 0.9,
                pMax: 100,
                pMin: 50),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredinefficient",
                type: PowerPlantType.Gasfired,
                efficiency: 0.1,
                pMax: 100,
                pMin: 0),
            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 60);

            Assert.Equal("gasfired", result[0].PowerPlantName);
            Assert.Equal(60, result[0].PowerGenerated);
        }
        [Fact]
        public void PassTrickyTest2()
        {
            var fuelPrices = new FuelPrices { Co2 = 0, Wind = 100, Kerosine = 50.8, Gas = 20 };
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "windpark1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 60,
                pMin: 0),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfired",
                type: PowerPlantType.Gasfired,
                efficiency: 0.9,
                pMax: 100,
                pMin: 50),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredinefficient",
                type: PowerPlantType.Gasfired,
                efficiency: 0.1,
                pMax: 200,
                pMin: 0),
            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 80);

            Assert.Equal("gasfired", result[0].PowerPlantName);
            Assert.Equal(80, result[0].PowerGenerated);
        }
        [Fact]
        public void PassExamplePayload1_NoCO2()
        {
            var fuelPrices = new FuelPrices { Co2 = 0, Wind = 60, Kerosine = 50.8, Gas = 13.4 };
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredbig1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.53,
                pMax: 460,
                pMin: 100),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredbig2",
                type: PowerPlantType.Gasfired,
                efficiency: 0.53,
                pMax: 460,
                pMin: 100),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredsomewhatsmaller",
                type: PowerPlantType.Gasfired,
                efficiency: 0.37,
                pMax: 210,
                pMin: 40),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "tj1",
                type: PowerPlantType.Turbojet,
                efficiency: 0.3,
                pMax: 16,
                pMin: 0),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "windpark1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 150,
                pMin: 0),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "windpark2",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 36,
                pMin: 0),
            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 480);

            Assert.Equal("windpark1", result[0].PowerPlantName);
            Assert.Equal(90, result[0].PowerGenerated);
            Assert.Equal("windpark2", result[1].PowerPlantName);
            Assert.Equal(21.6, result[1].PowerGenerated);
            Assert.Equal("gasfiredbig1", result[2].PowerPlantName);
            Assert.Equal(368.4, result[2].PowerGenerated);
        }
        [Fact]
        public void PassExamplePayload2_NoCO2()
        {
            var fuelPrices = new FuelPrices { Co2 = 0, Wind = 0, Kerosine = 50.8, Gas = 13.4 };
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredbig1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.53,
                pMax: 460,
                pMin: 100),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredbig2",
                type: PowerPlantType.Gasfired,
                efficiency: 0.53,
                pMax: 460,
                pMin: 100),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredsomewhatsmaller",
                type: PowerPlantType.Gasfired,
                efficiency: 0.37,
                pMax: 210,
                pMin: 40),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "tj1",
                type: PowerPlantType.Turbojet,
                efficiency: 0.3,
                pMax: 16,
                pMin: 0),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "windpark1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 150,
                pMin: 0),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "windpark2",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 36,
                pMin: 0),
            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 480);

            Assert.Equal("gasfiredbig1", result[0].PowerPlantName);
            Assert.Equal(380, result[0].PowerGenerated);
            Assert.Equal("gasfiredbig2", result[1].PowerPlantName);
            Assert.Equal(100, result[1].PowerGenerated);
        }
        [Fact]
        public void PassExamplePayload3_NoCO2()
        {
            var fuelPrices = new FuelPrices { Co2 = 0, Wind = 60, Kerosine = 50.8, Gas = 13.4 };
            var powerPlants = new List<PowerPlant>
            {
                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredbig1",
                type: PowerPlantType.Gasfired,
                efficiency: 0.53,
                pMax: 460,
                pMin: 100),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredbig2",
                type: PowerPlantType.Gasfired,
                efficiency: 0.53,
                pMax: 460,
                pMin: 100),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "gasfiredsomewhatsmaller",
                type: PowerPlantType.Gasfired,
                efficiency: 0.37,
                pMax: 210,
                pMin: 40),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "tj1",
                type: PowerPlantType.Turbojet,
                efficiency: 0.3,
                pMax: 16,
                pMin: 0),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "windpark1",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 150,
                pMin: 0),

                PowerPlant.Create(
                fuelPrice: fuelPrices,
                name: "windpark2",
                type: PowerPlantType.Windturbine,
                efficiency: 1,
                pMax: 36,
                pMin: 0),
            };

            var result = _calculationService.CalculatePowerProduction(powerPlants, 910);

            Assert.Equal("windpark1", result[0].PowerPlantName);
            Assert.Equal(90, result[0].PowerGenerated);
            Assert.Equal("windpark2", result[1].PowerPlantName);
            Assert.Equal(21.6, result[1].PowerGenerated);
            Assert.Equal("gasfiredbig1", result[2].PowerPlantName);
            Assert.Equal(460, result[2].PowerGenerated);
            Assert.Equal("gasfiredbig2", result[3].PowerPlantName);
            Assert.Equal(338.4, result[3].PowerGenerated);
        }
    }
}
