using InnovationInc.TextToSql.WebApi.Models;
using System.Security.Claims;

namespace InnovationInc.TextToSql.WebApi.Interfaces
{
    public interface IAIService
    {
        Task<PromptResponse> GetResponseAsync(Guid conversationId, string promptText, ClaimsPrincipal user, CancellationToken cancellationToken);
        Conversation? GetConversationHistory(Guid conversationId);
    }
}
