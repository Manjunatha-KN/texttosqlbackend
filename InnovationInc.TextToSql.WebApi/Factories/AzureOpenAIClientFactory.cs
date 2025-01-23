using Azure;
using Azure.AI.OpenAI;
using InnovationInc.TextToSql.WebApi.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace InnovationInc.TextToSql.WebApi.Factories
{
    public class AzureOpenAIClientFactory : IAIClientFactory
    {
        private const string endpoint = "https://dlstg763680.openai.azure.com/";
        private const string key = "6799d99aa5a94cf8adf7d3d8156ace6c";

        private readonly IMemoryCache _memoryCache;

        public AzureOpenAIClientFactory(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public AzureOpenAIClient? CreateClient(string clientName)
        {
            if (!_memoryCache.TryGetValue(clientName, out AzureOpenAIClient? client))
            {
                client = new AzureOpenAIClient(new Uri(endpoint), new System.ClientModel.ApiKeyCredential(key));
                _memoryCache.Set(clientName, client);
            }

            return client;
        }
    }
}
