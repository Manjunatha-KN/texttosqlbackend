using OpenAI.Chat;

namespace InnovationInc.TextToSql.WebApi.Models
{
    public class Conversation
    {
        public Guid Id { get; set; }
        public IEnumerable<ChatMessage> History { get; set; } = new List<ChatMessage>();
    }
}
