using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchHistoryController : ControllerBase
    {
        private readonly SearchHistoryService _searchHistoryService;

        public SearchHistoryController(
            SearchHistoryService searchHistoryService)
        {
            _searchHistoryService = searchHistoryService;
        }

        [HttpGet]
        public IActionResult GetHistory()
        {
            return Ok(_searchHistoryService.GetHistory());
        }

        [HttpGet("count")]
        public IActionResult GetCount()
        {
            return Ok(new
            {
                count = _searchHistoryService.GetSearchCount()
            });
        }

        [HttpPost]
        public IActionResult AddSearch([FromBody] SearchRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return BadRequest(new
                {
                    message = "Search term is required."
                });
            }

            _searchHistoryService.AddSearch(request.SearchTerm);

            return Ok(new
            {
                message = "Search recorded successfully."
            });
        }

        [HttpDelete]
        public IActionResult ClearHistory()
        {
            _searchHistoryService.ClearHistory();

            return Ok(new
            {
                message = "Search history cleared successfully."
            });
        }
    }

    public class SearchRequest
    {
        public string SearchTerm { get; set; } = string.Empty;
    }
}