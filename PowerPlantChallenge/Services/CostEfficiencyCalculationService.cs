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
        /// <param name="powerplants"></param>
        /// <param name="neededLoad"></param>
        /// <returns></returns>
        public List<PowerProduction> CalculatePowerProduction(List<Powerplant> powerplants, double neededLoad)
        {

            List<PowerProduction> powerProductions = new();            
            var remainingNeededLoad = neededLoad;
            var accumulatedPMin = 0.0;
            powerplants = powerplants.Where(pp => pp.EffectivePMax > 0).OrderBy(pp => pp.CostPerUnit).ToList();

            // Foreach Powerplant (ordered by costefficienty)
            for (int powerplantIndex = 0; powerplantIndex < powerplants.Count; powerplantIndex++)
            {

                var powerplant = powerplants[powerplantIndex];

                if (powerplant.PMin > neededLoad)
                    continue;

                // Get out of the loop is somehow the remaining needed load is 0 (safeguard)
                if (remainingNeededLoad <= 0)
                    return powerProductions;

                // In Case we can't use a power plant because it has too high of a PMin
                if (powerplant.PMin > remainingNeededLoad)
                {

                    // If it's the first one in the list, just skip it
                    if (powerplantIndex == 0)
                        continue;

                    if (accumulatedPMin + powerplant.PMin <= neededLoad)
                    {
                        var deltaPmin =  powerplant.PMin - remainingNeededLoad;
                        return ComparePowerProductionsByLoweringPMax(powerplants, neededLoad, deltaPmin, powerplantIndex);
                    }
                    else
                    {
                        // Take all all indexes of other already processed Powerplants that might cause issues, i.e.:
                        // this powerplant, the powerplant just before it and all powerplants before it that have Pmin higher than this one.
                        var disturberIndexes = powerplants.Take(powerplantIndex).Where(pow => pow.PMin > powerplant.PMin).Select((pow, index) => index).ToList();
                        var indexesToAvoid = new List<int> { powerplantIndex, powerplantIndex - 1 };
                        if (disturberIndexes != null)
                            indexesToAvoid.AddRange(disturberIndexes);
                        // Compare which scenario with 
                        return ComparePowerProductionsByRemovingPowerplants(powerplants, neededLoad, indexesToAvoid);
                    }
                }

                else if (powerplant.EffectivePMax > remainingNeededLoad)
                {
                    powerProductions.Add(new PowerProduction(powerplant.Name, remainingNeededLoad, powerplant.CostPerUnit));
                    
                    accumulatedPMin += powerplant.PMin;
                    remainingNeededLoad = 0;
                }

                else
                {
                    powerProductions.Add(new PowerProduction(powerplant.Name, powerplant.EffectivePMax, powerplant.CostPerUnit));

                    accumulatedPMin += powerplant.PMin;
                    remainingNeededLoad -= powerplant.EffectivePMax;
                }
            }

            return remainingNeededLoad > 0 ? null : powerProductions;
            
        }

        /// <summary>
        /// Takes a list of powerplants and a list of indexes, for each index in the list this function will 
        /// calculate the most efficient power production without the power plant at that index in the powerplants list.
        /// Then returns the most efficient result.
        /// </summary>
        /// <param name="powerplants"></param>
        /// <param name="neededLoad"></param>
        /// <param name="indexesToAvoid">indexes that wille be removed from the initial powerplan list</param>
        /// <returns></returns>
        private List<PowerProduction> ComparePowerProductionsByRemovingPowerplants(List<Powerplant> powerplants, double neededLoad, List<int> indexesToAvoid)
        {
            List<PowerProduction> result = null;

            foreach (var indexToAvoid in indexesToAvoid)
            {
                var powerplantsWithoutCandidate = powerplants.Where((pow, index) => index != indexToAvoid).ToList();
                var candidate = CalculatePowerProduction(powerplantsWithoutCandidate, neededLoad);
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
        /// <param name="powerplants"></param>
        /// <param name="neededLoad"></param>
        /// <param name="delta">amount of power units we need to substract from more efficient power plants</param>
        /// <param name="candidateIndex">index of the powerplant candidate </param>
        /// <returns>List of the most efficient powerproductions between a scenario without the candidate and Pmax reduction from most efficient powerplants</returns>
        private List<PowerProduction> ComparePowerProductionsByLoweringPMax(List<Powerplant> powerplants, double neededLoad, double delta, int candidateIndex)
        {
            
            var updatedPowerplants = powerplants.Select(x => new Powerplant
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
            for (int  powerplantIndex = candidateIndex -1; powerplantIndex >= 0; powerplantIndex--)
            {
                var unitsToSubstract = Math.Min(updatedPowerplants[powerplantIndex].EffectivePMax - updatedPowerplants[powerplantIndex].PMin, remainingUnitsToSubstract);

                if (unitsToSubstract == updatedPowerplants[powerplantIndex].EffectivePMax)
                    updatedPowerplants.RemoveAt(powerplantIndex);
                else updatedPowerplants[powerplantIndex].EffectivePMax -= unitsToSubstract;

                remainingUnitsToSubstract -= unitsToSubstract;

                if (remainingUnitsToSubstract <= 0)
                    break;
            }

            var powerProdsWithtoutCandidate = CalculatePowerProduction(powerplants.Where((pow, i) => i != candidateIndex).ToList(), neededLoad);
            var powerProdsByDiminishingPMaxes = CalculatePowerProduction(updatedPowerplants, neededLoad);
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
            if (powerProds1 == null && powerProds2 == null)
                return null;

            return powerProds1.Sum(powerProd => powerProd.PowerGenerated * powerProd.CostPerUnit)
                < powerProds2.Sum(powerProd => powerProd.PowerGenerated * powerProd.CostPerUnit) ?
                powerProds1 : powerProds2;
        }

    }
}
