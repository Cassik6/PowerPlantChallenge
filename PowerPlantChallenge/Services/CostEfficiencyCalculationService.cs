using PowerPlantChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PowerPlantChallenge.Services
{
    public class CostEfficiencyCalculationService : ICostEfficiencyCalculationService
    {
        /// <summary>
        /// returns the most efficient powerproduction schema based on a list of powerplans and a given need of load
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
                var powerPlant = powerPlants[powerPlantIndex];

                // if the powerplant minimum power production is higher than what is need, there's no need to consider her.
                if (powerPlant.PMin > neededLoad)
                    continue;

                
                if (remainingNeededLoad <= 0)
                    return powerProductions;

                // In Case we can't use a power plant because of it's minimum power production being too high
                if (powerPlant.PMin > remainingNeededLoad)
                {
                    
                    if (accumulatedPMin + powerPlant.PMin <= neededLoad)
                    {
                        var deltaPmin =  powerPlant.PMin - remainingNeededLoad;
                        return ComparePowerProductionsByLoweringPMax(powerPlants, neededLoad, deltaPmin, powerPlantIndex);
                    }
                    else
                    {
                        // Take all all indexes of other already processed Powerplants that might cause issues, i.e.:
                        // this powerplant, the powerplant just before it and all powerplants before it that have Pmin higher than this one.
                        var disturberIndexes = powerPlants.Take(powerPlantIndex).Where(pow => pow.PMin > powerPlant.PMin).Select((pow, index) => index).ToList();
                        var indexesToAvoid = new List<int> { powerPlantIndex, powerPlantIndex - 1 };
                        
                        // Compare which scenario with or without the disturber powerplant is better
                        return ComparePowerProductionsByRemovingPowerPlants(powerPlants, neededLoad, indexesToAvoid);
                    }
                }

                if (powerPlant.EffectivePMax > remainingNeededLoad)
                {
                    powerProductions.Add(new PowerProduction(powerPlant.Name, remainingNeededLoad, powerPlant.CostPerUnit));
                    
                    accumulatedPMin += powerPlant.PMin;
                    remainingNeededLoad = 0;
                }

                else
                {
                    powerProductions.Add(new PowerProduction(powerPlant.Name, powerPlant.EffectivePMax, powerPlant.CostPerUnit));

                    accumulatedPMin += powerPlant.PMin;
                    remainingNeededLoad -= powerPlant.EffectivePMax;
                }
            }

            return remainingNeededLoad > 0 ? null : powerProductions;
            
        }

        /// <summary>
        /// Takes a list of powerplants and a list of indexes, for each index in the list this function will 
        /// calculate the most efficient power production without the power plant at that index in the powerplants list.
        /// Then returns the most efficient result.
        /// </summary>
        /// <param name="powerPlants"></param>
        /// <param name="neededLoad"></param>
        /// <param name="indexesToAvoid">indexes that wille be removed from the initial powerplan list</param>
        /// <returns></returns>
        private List<PowerProduction> ComparePowerProductionsByRemovingPowerPlants(List<PowerPlant> powerPlants, double neededLoad, List<int> indexesToAvoid)
        {
            List<PowerProduction> result = null;

            foreach (var indexToAvoid in indexesToAvoid)
            {
                var powerPlantsWithoutCandidate = powerPlants.Where((pow, index) => index != indexToAvoid).ToList();
                var candidate = CalculatePowerProduction(powerPlantsWithoutCandidate, neededLoad);
                if (result != null && candidate != null)
                    result = ComparePowerProductions(candidate, result);
                else if (candidate != null)
                    result = candidate;
            }

            return result;
        }
        /// <summary>
        /// Given a list of powerplants and a specific candidate (at index in powerplants list) 
        /// Checks weither it's worth lowering max production of some more efficient powerplants in order to allow firing the candidate at minimum Power.
        /// </summary>
        /// <param name="powerPlants"></param>
        /// <param name="neededLoad"></param>
        /// <param name="delta">amount of power units we need to substract from more efficient power plants</param>
        /// <param name="candidateIndex">index of the powerplant candidate </param>
        /// <returns>List of the most efficient powerproductions between a scenario without the candidate and Pmax reduction from most efficient powerplants</returns>
        private List<PowerProduction> ComparePowerProductionsByLoweringPMax(List<PowerPlant> powerPlants, double neededLoad, double delta, int candidateIndex)
        {
            
            var updatedPowerPlants = powerPlants.Select(x => new PowerPlant
            {
               CostPerUnit = x.CostPerUnit,
               EffectivePMax = x.EffectivePMax,
               Efficiency = x.Efficiency,
               Name = x.Name,
               PMax = x.PMax,
               PMin = x.PMin,
               Type = x.Type
            }).ToList();

            var remainingUnitsToSubstract = delta;
           
            // foreach element in the list starting at the index of the candidate and going upwards from there.
            for (var  powerPlantIndex = candidateIndex -1; powerPlantIndex >= 0; powerPlantIndex--)
            {
                var unitsToSubtract = Math.Min(updatedPowerPlants[powerPlantIndex].EffectivePMax - updatedPowerPlants[powerPlantIndex].PMin, remainingUnitsToSubstract);

                if (unitsToSubtract == updatedPowerPlants[powerPlantIndex].EffectivePMax)
                    updatedPowerPlants.RemoveAt(powerPlantIndex);
                else updatedPowerPlants[powerPlantIndex].EffectivePMax -= unitsToSubtract;

                remainingUnitsToSubstract -= unitsToSubtract;

                if (remainingUnitsToSubstract <= 0)
                    break;
            }

            var powerProdsWithtoutCandidate = CalculatePowerProduction(powerPlants.Where((pow, i) => i != candidateIndex).ToList(), neededLoad);
            var powerProdsByDiminishingPMaxes = CalculatePowerProduction(updatedPowerPlants, neededLoad);
            return ComparePowerProductions(powerProdsWithtoutCandidate, powerProdsByDiminishingPMaxes);
        }
        /// <summary>
        /// compares 2 lists of powerproduction and returns the most cost efficient one
        /// </summary>
        /// <param name="powerProd1"></param>
        /// <param name="powerProd2"></param>
        /// <returns></returns>
        private List<PowerProduction> ComparePowerProductions(List<PowerProduction> powerProds1, List<PowerProduction> powerProds2)
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
