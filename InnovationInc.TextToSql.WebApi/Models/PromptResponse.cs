namespace InnovationInc.TextToSql.WebApi.Models
{
    public class PromptResponse
    {
        public Guid ConversationId { get; set; }
        public string? UserText { get; set; }
        public string? SqlQuery { get; set; }
        public string? SqlQueryExplanation { get; set; }
        public string? AIResponse { get; set; }
    }
}
