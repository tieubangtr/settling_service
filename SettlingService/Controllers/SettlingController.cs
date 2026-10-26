using Microsoft.AspNetCore.Mvc;
using SettlingEngine.Services;
using SettlingEngine.Utils;

namespace SettlingService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettlingController : ControllerBase
    {
        private readonly ISettlingService _settlingService;

        public SettlingController(ISettlingService settlingService)
        {
            _settlingService = settlingService;
        }

        [HttpGet("run")]
        public async Task<IActionResult> Settle([FromQuery] string? marketId, [FromQuery] string? debitCredit)
        {
            try
            {
                var response = await _settlingService.Settle(marketId, debitCredit);

                if (response.Code > 0)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            try
            {
                await _settlingService.Test();

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("create-settlement")]
        public async Task<IActionResult> CreateMemberSettlement()
        {
            try
            {
                var response = await _settlingService.CreateMemberSettlement();

                if (response.Code > 0)
                {
                    return Ok("Success");
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
                return BadRequest(ex.Message);
            }

            return BadRequest("Error");
        }

        [HttpGet("get-market")]
        public async Task<IActionResult> GetMarkets()
        {
            try
            {
                var response = await _settlingService.GetMarkets();

                if (response == null || response.Count == 0)
                {
                    return NoContent();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get-member-settlement")]
        public async Task<IActionResult> GetMemberSettlement([FromQuery] string? marketId)
        {
            try
            {
                var response = await _settlingService.GetMemberSettlements(marketId);

                if (response == null || response.Count == 0)
                {
                    return NoContent();
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
