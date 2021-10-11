using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PowerPlantChallenge.Mapping;
using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.DTOs;
using PowerPlantChallenge.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PowerPlantChallenge.Controllers
{
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        private readonly ICostEfficiencyCalculationService costEfficiencyCalculationService;
        private readonly ILogger<ProductionPlanController> logger;

        public ProductionPlanController(ICostEfficiencyCalculationService costEfficiencyCalculationService, ILogger<ProductionPlanController> logger)
        {
            this.costEfficiencyCalculationService = costEfficiencyCalculationService;
            this.logger = logger;
        }

        [Route("/productionplan")]
        [HttpPost]
        public ActionResult<List<PowerProduction>> ProductionPlan([FromBody] PayloadDTO dtoPayload)
        {
            try
            {
                var payload = DTOToPayLoad.Map(dtoPayload);
            
                return costEfficiencyCalculationService.CalculatePowerProduction(payload.Powerplants, payload.NeededLoad);
            }
            catch (ValidationException exception)
            {
                logger.LogError(exception, exception.Message, exception.ValidationResult);
                return BadRequest(exception.Message);
               
            }
            catch(Exception exception)
            {
                logger.LogError(exception, exception.Message);
                return StatusCode(501);
            }

        }
    }
}
