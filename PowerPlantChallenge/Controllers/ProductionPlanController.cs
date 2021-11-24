﻿using Microsoft.AspNetCore.Mvc;
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
        //[Route("/productionplan/ws")]
        //[HttpPost]
        //public async Task Post()
        //{
        //    if (HttpContext.WebSockets.IsWebSocketRequest)
        //    {
        //        using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        //        await Echo(HttpContext, webSocket);
        //    }
        //    else
        //    {
        //        HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        //    }
        //}

        //private async Task Echo(HttpContext context, WebSocket webSocket)
        //{
        //    var buffer = new byte[1024 * 4];
        //    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //    while (!result.CloseStatus.HasValue)
        //    {
        //        await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);

        //        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //    }
        //    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        //}
    }
}
