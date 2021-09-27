using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using PowerPlantChallenge.Models;
using PowerPlantChallenge.Services;
using Xunit;

namespace PowerPlantChallenge.Tests.Services
{
    public class CostEfficiencyServiceShould
    {
        [Fact]
        public void SkipWindTurbineIfNecessary()
        {

            var powerplants = new List<Powerplant>()
                {
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "GasFiredEco",
                        PMax = 90,
                        PMin = 90,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 50,
                        EffectivePMax = 90
                    },
                    new Powerplant()
                    {
                        Efficiency = 0.9,
                        Name = "GasFiredEco2",
                        PMax = 10,
                        PMin = 10,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 500,
                        EffectivePMax = 10
                    },
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "WindPark",
                        PMax = 20,
                        PMin = 0,
                        Type = PowerplantType.Windturbine,
                        CostPerUnit = 0,
                        EffectivePMax = 20
                    },
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "GasFiredToxic",
                        PMax = 80,
                        PMin = 0,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 1000000,
                        EffectivePMax = 80
                    }
                };

            CostEfficiencyCalculationService service = new();

            var result = service.CalculatePowerProduction(powerplants, 100);

            Assert.Equal("GasFiredEco", result[0].PowerplantName);
            Assert.Equal("GasFiredEco2", result[1].PowerplantName);
            Assert.Equal(90, result[0].PowerGenerated);
            Assert.Equal(10, result[1].PowerGenerated);
        }

        [Fact]
        public void TakeMostEfficient()
        {

            var powerplants = new List<Powerplant>()
                {
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "GasFiredEco",
                        PMax = 50,
                        PMin = 50,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 50,
                        EffectivePMax = 50
                    },
                    new Powerplant()
                    {
                        Efficiency = 0.9,
                        Name = "GasFiredEco2",
                        PMax = 10,
                        PMin = 10,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 500,
                        EffectivePMax = 10
                    },
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "WindPark",
                        PMax = 20,
                        PMin = 0,
                        Type = PowerplantType.Windturbine,
                        CostPerUnit = 0,
                        EffectivePMax = 20
                    },
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "GasFiredToxic",
                        PMax = 80,
                        PMin = 0,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 1000000,
                        EffectivePMax = 80
                    }
                };

            CostEfficiencyCalculationService service = new();

            var result = service.CalculatePowerProduction(powerplants, 100);

            Assert.Equal("WindPark", result[0].PowerplantName);
            Assert.Equal("GasFiredEco", result[1].PowerplantName);
            Assert.Equal("GasFiredEco2", result[2].PowerplantName);
            Assert.Equal("GasFiredToxic", result[3].PowerplantName);
            Assert.Equal(20, result[0].PowerGenerated);
            Assert.Equal(50, result[1].PowerGenerated);
            Assert.Equal(10, result[2].PowerGenerated);
            Assert.Equal(20, result[3].PowerGenerated);
        }

        [Fact]
        public void NotAlwayUseMaxProductionCapcity()
        {

            var powerplants = new List<Powerplant>()
                {
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "GasFiredEco",
                        PMax = 45,
                        PMin = 40,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 50,
                        EffectivePMax = 45
                    },
                    new Powerplant()
                    {
                        Efficiency = 0.9,
                        Name = "GasFiredEco2",
                        PMax = 45,
                        PMin = 40,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 500,
                        EffectivePMax = 45
                    },
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "GasFiredEco3",
                        PMax = 20,
                        PMin = 20,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 5000,
                        EffectivePMax = 20
                    },
                    new Powerplant()
                    {
                        Efficiency = 1,
                        Name = "GasFiredToxic",
                        PMax = 80,
                        PMin = 0,
                        Type = PowerplantType.Gasfired,
                        CostPerUnit = 1000000,
                        EffectivePMax = 80
                    }
                };

            CostEfficiencyCalculationService service = new();

            var result = service.CalculatePowerProduction(powerplants, 100);

            
            Assert.Equal("GasFiredEco", result[0].PowerplantName);
            Assert.Equal("GasFiredEco2", result[1].PowerplantName);
            Assert.Equal("GasFiredEco3", result[2].PowerplantName);
            Assert.Equal(40, result[0].PowerGenerated);
            Assert.Equal(40, result[1].PowerGenerated);
            Assert.Equal(20, result[2].PowerGenerated);
            
        }

    }
}
