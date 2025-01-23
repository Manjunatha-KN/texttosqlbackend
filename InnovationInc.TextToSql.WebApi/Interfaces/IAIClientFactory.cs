using Azure.AI.OpenAI;

namespace InnovationInc.TextToSql.WebApi.Interfaces
{
    public interface IAIClientFactory
    {
        AzureOpenAIClient? CreateClient(string clientName);
    }
}
