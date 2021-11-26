using PowerPlantChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerPlantChallenge.Services
{
    public class CostEfficiencyCalculationService : ICostEfficiencyCalculationService
    {
        /// <summary>
        /// returns the most efficient powerProduction schema based on a list of powerPlans and a given need of load
        /// </summary>
        /// <param name="powerPlants"></param>
        /// <param name="neededLoad"></param>
        /// <returns></returns>
        public List<PowerProduction> CalculatePowerProduction(List<PowerPlant> powerPlants, double neededLoad)
        {
            List<PowerProduction> powerProductions = new();
            var remainingNeededLoad = neededLoad;
            var accumulatedPMin = 0.0;
            powerPlants = powerPlants.Where(pp => pp.EffectivePMax > 0).OrderBy(pp => pp.CostPerUnit).ToList();


            for (var powerPlantIndex = 0; powerPlantIndex < powerPlants.Count; powerPlantIndex++)
            {
                var currentPowerPlant = powerPlants[powerPlantIndex];

                // if the powerPlant minimum power production is higher than what is need, there's no need to consider her.
                if (currentPowerPlant.PMin > neededLoad)
                    continue;


                if (remainingNeededLoad <= 0)
                    return powerProductions;

                // In Case we can't use a power plant because of it's minimum power production being too high
                if (currentPowerPlant.PMin > remainingNeededLoad)
                {
                    if (accumulatedPMin + currentPowerPlant.PMin <= neededLoad)
                    {
                        var deltaPMin = currentPowerPlant.PMin - remainingNeededLoad;
                        return ComparePowerProductionsByLoweringPMax(powerPlants, neededLoad, deltaPMin, powerPlantIndex);
                    }

                    // Take all all indexes of other already processed Powerplants that might cause issues, i.e.:
                    // this powerPlant, the powerPlant just before it and all powerPlants before it that have PMin higher than this one.
                    var indexesToAvoid = new List<int> { powerPlantIndex, powerPlantIndex - 1 };
                    // Compare which scenario with or without the disturber powerPlant is better
                    return ComparePowerProductionsByRemovingPowerPlants(powerPlants, neededLoad, indexesToAvoid);
                }

                accumulatedPMin += currentPowerPlant.PMin;
                if (currentPowerPlant.EffectivePMax > remainingNeededLoad)
                {
                    powerProductions.Add(new PowerProduction(currentPowerPlant.Name, remainingNeededLoad, currentPowerPlant.CostPerUnit));
                    remainingNeededLoad = 0;
                }

                else
                {
                    powerProductions.Add(new PowerProduction(currentPowerPlant.Name, currentPowerPlant.EffectivePMax, currentPowerPlant.CostPerUnit));
                    remainingNeededLoad -= currentPowerPlant.EffectivePMax;
                }
            }

            return remainingNeededLoad > 0 ? null : powerProductions;

        }

        /// <summary>
        /// Takes a list of powerPlants and a list of indexes, for each index in the list this function will 
        /// calculate the most efficient power production without the power plant at that index in the powerPlants list.
        /// Then returns the most efficient result.
        /// </summary>
        /// <param name="powerPlants"></param>
        /// <param name="neededLoad"></param>
        /// <param name="indexesToAvoid">indexes that will be removed from the initial powerPlan list</param>
        /// <returns></returns>
        private List<PowerProduction> ComparePowerProductionsByRemovingPowerPlants(List<PowerPlant> powerPlants, double neededLoad, List<int> indexesToAvoid)
        {
            List<PowerProduction> result = null;

            foreach (var indexToAvoid in indexesToAvoid)
            {
                var powerPlantsWithoutOutsider = powerPlants.Where((pow, index) => index != indexToAvoid).ToList();
                var candidates = CalculatePowerProduction(powerPlantsWithoutOutsider, neededLoad);

                result = ComparePowerProductions(candidates, result);
            }

            return result;
        }
        /// <summary>
        /// Given a list of powerPlants and a specific candidate (at index in powerPlants list) 
        /// Checks weither it's worth lowering max production of some more efficient powerPlants in order to allow firing the candidate at minimum Power.
        /// </summary>
        /// <param name="powerPlants"></param>
        /// <param name="neededLoad"></param>
        /// <param name="delta">amount of power units we need to subtract from more efficient power plants</param>
        /// <param name="candidateIndex">index of the powerPlant candidate </param>
        /// <returns>List of the most efficient powerProductions between a scenario without the candidate and PMax reduction from most efficient powerPlants</returns>
        private List<PowerProduction> ComparePowerProductionsByLoweringPMax(List<PowerPlant> powerPlants, double neededLoad, double delta, int candidateIndex)
        {

            var updatedPowerPlants = powerPlants.ConvertAll(PowerPlant.Clone).ToList();

            var remainingUnitsToSubtract = delta;

            // foreach element in the list starting at the index of the candidate and going upwards from there.
            for (var powerPlantIndex = candidateIndex - 1; powerPlantIndex >= 0; powerPlantIndex--)
            {
                var currentPowerPlant = updatedPowerPlants[powerPlantIndex];

                var unitsToSubtract = Math.Min(currentPowerPlant.EffectivePMax - currentPowerPlant.PMin, remainingUnitsToSubtract);

                if (unitsToSubtract == currentPowerPlant.EffectivePMax)
                    updatedPowerPlants.RemoveAt(powerPlantIndex);
                else currentPowerPlant.EffectivePMax -= unitsToSubtract;

                remainingUnitsToSubtract -= unitsToSubtract;

                if (remainingUnitsToSubtract <= 0)
                    break;
            }

            var powerProdsWithoutCandidate = CalculatePowerProduction(powerPlants.Where((pow, i) => i != candidateIndex).ToList(), neededLoad);
            var powerProdsByDiminishingPMaxes = CalculatePowerProduction(updatedPowerPlants, neededLoad);
            return ComparePowerProductions(powerProdsWithoutCandidate, powerProdsByDiminishingPMaxes);
        }
        /// <summary>
        /// compares 2 lists of powerProduction and returns the most cost efficient one
        /// </summary>
        /// <param name="powerProds1"></param>
        /// <param name="powerProds2"></param>
        /// <returns></returns>
        private static List<PowerProduction> ComparePowerProductions(List<PowerProduction> powerProds1, List<PowerProduction> powerProds2)
        {
            if (powerProds1 == null)
                return powerProds2;
            if (powerProds2 == null)
                return powerProds1;

            return powerProds1.Sum(powerProd => powerProd.PowerGenerated * powerProd.CostPerUnit)
                < powerProds2.Sum(powerProd => powerProd.PowerGenerated * powerProd.CostPerUnit) ?
                powerProds1 : powerProds2;
        }

    }
}
