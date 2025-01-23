using InnovationInc.TextToSql.WebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InnovationInc.TextToSql.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationsController : ControllerBase
    {
        private readonly IAIService _aiService;
        private readonly ILogger<ConversationsController> _logger;

        public ConversationsController(IAIService aiService, ILogger<ConversationsController> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

        [HttpGet("{conversationId:guid?}")]
        public async Task<IActionResult> GetPromptResponseAsync(
            [FromRoute] Guid? conversationId,
            [FromQuery] string prompt,
            CancellationToken cancellationToken)
        {
            var response = await _aiService.GetResponseAsync(conversationId ?? Guid.NewGuid(), prompt, User, cancellationToken);
            return Ok(response);
        }

        [HttpGet("{conversationId:guid}/history")]
        public IActionResult GetHistoryAsync([FromRoute] Guid conversationId, CancellationToken cancellationToken)
        {
            var history = _aiService.GetConversationHistory(conversationId);
            return Ok(history);
        }
    }
}
