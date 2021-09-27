using Microsoft.AspNetCore.Mvc;
using PowerPlantChallenge.Mapping;
using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.DTOs;
using PowerPlantChallenge.Services;
using System.Collections.Generic;

namespace PowerPlantChallenge.Controllers
{
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        private readonly ICostEfficiencyCalculationService costEfficiencyCalculationService;

        public ProductionPlanController(ICostEfficiencyCalculationService costEfficiencyCalculationService)
        {
            this.costEfficiencyCalculationService = costEfficiencyCalculationService;
        }

        [Route("/productionplan")]
        [HttpPost]
        public ActionResult<List<PowerProduction>> ProductionPlan([FromBody] PayloadDTO dtoPayload)
        {
            var payload = DTOToPayLoad.Map(dtoPayload);

            return costEfficiencyCalculationService.CalculatePowerProduction(payload.Powerplants, payload.NeededLoad);
        }
    }
}
