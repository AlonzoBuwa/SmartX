using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;

        public RecommendationController(
            RecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
        }

        // ============================================================
        // GET CURRENT RECOMMENDATIONS
        // ============================================================

        [HttpGet]
        public IActionResult GetRecommendations()
        {
            var recommendations =
                _recommendationService.GenerateRecommendations();

            return Ok(recommendations);
        }
    }
}