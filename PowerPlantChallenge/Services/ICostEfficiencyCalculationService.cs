using PowerPlantChallenge.Models;
using System.Collections.Generic;

namespace PowerPlantChallenge.Services
{
    public interface ICostEfficiencyCalculationService
    {
        List<PowerProduction> CalculatePowerProduction(List<PowerPlant> powerPlants, double neededLoad);
    }
}