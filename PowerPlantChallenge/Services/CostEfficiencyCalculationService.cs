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
            powerplants = powerplants.OrderBy(pp => pp.CostPerUnit).ToList();

            // Foreach Powerplant (ordered by costefficienty)
            for (int i = 0; i < powerplants.Count; i++)
            {

                if (powerplants[0].PMin > neededLoad)
                    continue;

                // Get out of the loop is somehow the remaining needed load is 0 (safeguard)
                if (remainingNeededLoad <= 0)
                    return powerProductions;

                // In Case we can't use a power plant because it has too high of a PMin
                if (powerplants[i].PMin > remainingNeededLoad)
                {

                    // If it's the first one in the list, just skip it
                    if (i == 0)
                        continue;

                    if (accumulatedPMin + powerplants[0].PMin >= neededLoad)
                    {
                        var deltaPmin = neededLoad - accumulatedPMin;
                        return ComparePowerProductionsByLoweringPMax(powerplants, neededLoad, deltaPmin, i);
                    }
                    else
                    {
                        // Take all all indexes of other already processed Powerplants that might cause issues, i.e.:
                        // this powerplant, the powerplant just before it and all powerplants before it that have Pmin higher than this one.
                        var disturberIndexes = powerplants.Take(i).Where(pow => pow.PMin > powerplants[i].PMin).Select((pow, index) => index).ToList();
                        var indexesToAvoid = new List<int> { i, i - 1 };
                        if (disturberIndexes != null)
                            indexesToAvoid.AddRange(disturberIndexes);
                        // Compare which scenario with 
                        return ComparePowerProductionsByRemovingPowerplants(powerplants, neededLoad, indexesToAvoid);
                    }
                }

                else if (powerplants[i].EffectivePMax > remainingNeededLoad)
                {
                    powerProductions.Add(new PowerProduction(powerplants[i].Name, remainingNeededLoad, powerplants[i].CostPerUnit));
                    
                    accumulatedPMin += powerplants[i].PMin;
                    remainingNeededLoad = 0;
                }

                else
                {
                    powerProductions.Add(new PowerProduction(powerplants[i].Name, powerplants[i].EffectivePMax, powerplants[i].CostPerUnit));

                    accumulatedPMin += powerplants[i].PMin;
                    remainingNeededLoad -= powerplants[i].EffectivePMax;
                }
            }

            return remainingNeededLoad > 0 ? null : powerProductions;
            
        }

       

        public int calcFac(int number)
        {
            if (number == 1)
                return 1;
            return number * calcFac(number - 1);
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
            // list containing all potential candidates for most efficient line up
            var results = new List<List<PowerProduction>>();

            foreach (var indexToAvoid in indexesToAvoid)
            {
                //TODO : faire la comparaison ici!
                // fills result with the result of each powerplant line up
                var powerplantsWithoutCandidate = powerplants.Where((pow, index) => index != indexToAvoid).ToList();
                var candidate = CalculatePowerProduction(powerplantsWithoutCandidate, neededLoad);
                if (candidate != null)
                    results.Add(candidate);
            }

            // TODO : unit test : list is null, total generated power if any list < neededLoad
            // returns the candidate with the lowest overall cost of production
            return results.OrderBy(list => list.Sum(powerProd => powerProd.PowerGenerated * powerProd.CostPerUnit)).First();
        }
        /// <summary>
        /// Given a list of powerplants and a specific candidate (at index in powerplants list) 
        /// Checks weither it's worth lowering max production of some more efficient powerplants in order to allow firing the candidate at minimum Power.
        /// </summary>
        /// <param name="powerplants"></param>
        /// <param name="neededLoad"></param>
        /// <param name="delta">amount of power units we need to substract from more efficient power plants</param>
        /// <param name="index">index of the powerplant candidate </param>
        /// <returns>List of the most efficient powerproductions between a scenario without the candidate and Pmax reduction from most efficient powerplants</returns>
        private List<PowerProduction> ComparePowerProductionsByLoweringPMax(List<Powerplant> powerplants, double neededLoad, double delta, int index)
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
            for (int  i = index -1; i >= 0; i--)
            {
                var unitsToSubstract = Math.Min(updatedPowerplants[i].EffectivePMax - updatedPowerplants[i].PMin, remainingUnitsToSubstract);

                if (unitsToSubstract == updatedPowerplants[i].EffectivePMax)
                    updatedPowerplants.RemoveAt(i);
                else updatedPowerplants[i].EffectivePMax -= unitsToSubstract;

                remainingUnitsToSubstract -= unitsToSubstract;

                if (remainingUnitsToSubstract <= 0)
                    break;
            }

            var powerProdsWithtoutCandidate = CalculatePowerProduction(powerplants.Where((pow, i) => i != index).ToList(), neededLoad);
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
            return powerProds1.Sum(powerProd => powerProd.PowerGenerated * powerProd.CostPerUnit)
                < powerProds2.Sum(powerProd => powerProd.PowerGenerated * powerProd.CostPerUnit) ?
                powerProds1 : powerProds2;
        }

    }
}
