using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PowerPlantChallenge.Mapping;
using PowerPlantChallenge.Middleware;
using PowerPlantChallenge.Models;
using PowerPlantChallenge.Models.Dtos;
using PowerPlantChallenge.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace PowerPlantChallenge.Controllers
{
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        private readonly ICostEfficiencyCalculationService _costEfficiencyCalculationService;
        private readonly ILogger<ProductionPlanController> _logger;
        private readonly IWebSocketService _webSocketService;

        public ProductionPlanController(ICostEfficiencyCalculationService costEfficiencyCalculationService, ILogger<ProductionPlanController> logger, IWebSocketService webSocketService)
        {
            _costEfficiencyCalculationService = costEfficiencyCalculationService;
            _logger = logger;
            _webSocketService = webSocketService;
        }

        [Route("/productionPlan")]
        [HttpPost]
        public ActionResult<List<PowerProduction>> ProductionPlan([FromBody] PayloadDto dtoPayload)
        {
            try
            {
                var payload = PayloadDtoConvertor.Map(dtoPayload);
                var powerProductions = _costEfficiencyCalculationService.CalculatePowerProduction(payload.PowerPlants, payload.NeededLoad);
                _webSocketService.SendPowerProduction(powerProductions);

                return powerProductions;
            }
            catch (ValidationException exception)
            {
                _logger.LogError("{Message}", exception.Message);
                return BadRequest(exception.Message);
               
            }
            catch(Exception exception)
            {
                _logger.LogError("{Message}", exception.Message);
                return StatusCode(501);
            }
        }
    }
}
