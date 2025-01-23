using Azure.AI.OpenAI;
using InnovationInc.TextToSql.WebApi.Interfaces;
using InnovationInc.TextToSql.WebApi.Models;
using Microsoft.Extensions.Caching.Memory;
using OpenAI.Chat;
using System.Data;
using System.Security.Claims;
using System.Text;

namespace azure_openai_quickstart.Services
{
    public class AzOpenAIService : IAIService
    {
        private const string jsonlFilePath = "training/ecommerce.jsonl";

        private IDbService _dbService;
        private readonly AzureOpenAIClient _azureClient;
        private readonly ChatClient _chatClient;
        private readonly ILogger<AzOpenAIService> _logger;
        private readonly IMemoryCache _memoryCache;

        public AzOpenAIService(ILogger<AzOpenAIService> logger, IDbService dbService, IAIClientFactory aIClientFactory, IMemoryCache memoryCache)
        {
            _logger = logger;
            _dbService = dbService;
            _azureClient = aIClientFactory.CreateClient("az-open-ai") ?? throw new ArgumentNullException(nameof(aIClientFactory), "AzureOpenAIClient creation failed.");
            _chatClient = _azureClient.GetChatClient("AOAIDemo");
            _memoryCache = memoryCache;
        }

        public async Task<PromptResponse> GetResponseAsync(Guid conversationId, string promptText, ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            var response = new PromptResponse
            {
                ConversationId = conversationId,
                UserText = promptText
            };

            string schemaDetails = File.ReadAllText(jsonlFilePath);

            _memoryCache.TryGetValue(conversationId.ToString(), out Conversation? ongoingConversation);
            if (ongoingConversation == null)
            {
                await InitiateConversation(conversationId, cancellationToken);
                _memoryCache.TryGetValue(conversationId.ToString(), out ongoingConversation);

                if (ongoingConversation == null)
                {
                    throw new InvalidOperationException("Failed to initiate conversation.");
                }
            }

            response.SqlQuery = await Text2SQL(promptText, ongoingConversation, cancellationToken);

            if (response.SqlQuery.ToLower().Contains("not met") || response.SqlQuery.ToLower().Contains("not available"))
            {
                return response;
            }

            response.SqlQueryExplanation = await SQL2Text(response.SqlQuery);

            // Execute the generated SQL query on the in-memory database
            var queryResult = await _dbService.ExecuteSqlQueryAsync(response.SqlQuery, cancellationToken);

            response.AIResponse = await SQL2Text(DataTableToString(queryResult));

            ongoingConversation.History = new List<ChatMessage>(ongoingConversation.History) { ChatMessage.CreateAssistantMessage(response.SqlQueryExplanation), ChatMessage.CreateAssistantMessage(response.AIResponse) };

            _memoryCache.Set(conversationId.ToString(), ongoingConversation);

            return response;
        }

        public Conversation? GetConversationHistory(Guid conversationId)
        {
            _memoryCache.TryGetValue(conversationId.ToString(), out Conversation? conversation);
            return conversation;
        }

        private async Task<string> SQL2Text(string sqlQuery)
        {
            // Construct the prompt message for the model
            string prompt = $"Explain the following SQL query in plain English: {sqlQuery}";

            _logger.LogInformation($"Prompt: {prompt}");

            var response = await _chatClient.CompleteChatAsync(new[] { ChatMessage.CreateUserMessage(prompt) }, new ChatCompletionOptions
            {
                MaxOutputTokenCount = 150, // Adjust the response length
                Temperature = 0.5f // Control the randomness of the response
            });
            
            // Extract the text from the response object
            return response.Value.Content[0].Text;
        }

        private async Task<ChatMessage> InitiateConversation(Guid conversationId, CancellationToken cancellationToken)
        {
            string schemaDetails = File.ReadAllText(jsonlFilePath);
            var prompt = $"Answer should be according to Schema Details: {schemaDetails}. If details not available in schema, ask further questions to get more information and design query accordingly";
            // Send the prompt and get a response from the model
            var response = await _chatClient.CompleteChatAsync(
                new[] { ChatMessage.CreateAssistantMessage(prompt) }, 
                new ChatCompletionOptions
                {
                    MaxOutputTokenCount = 100, // Adjust the response length
                    Temperature = 0.1f // Control the randomness of the response
                }, 
                cancellationToken);

            var responseChatMessage = ChatMessage.CreateAssistantMessage(response.Value.Content[0].Text);

            _memoryCache.Set(conversationId.ToString(), new Conversation
            {
                Id = conversationId,
                History = [ChatMessage.CreateAssistantMessage(prompt), responseChatMessage]
            });

            // Extract and return the generated SQL query from the response
            return responseChatMessage;
        }

        private async Task<string> Text2SQL(string userInput, Conversation ongoingConversation, CancellationToken cancellationToken)
        {
            // Construct the prompt message for the model
            string prompt = $"UserInput : {userInput}";
            var completeChatMessage = new List<ChatMessage>(ongoingConversation.History)
            {
                ChatMessage.CreateUserMessage(prompt)
            };

            // Send the prompt and get a response from the model
            var response = await _chatClient.CompleteChatAsync(completeChatMessage, new ChatCompletionOptions
            {
                MaxOutputTokenCount = 100, // Adjust the response length
                Temperature = 0.5f // Control the randomness of the response
            },
            cancellationToken);

            _memoryCache.Set(ongoingConversation.Id, new Conversation
            {
                Id = ongoingConversation.Id,
                History = new List<ChatMessage>(completeChatMessage) { ChatMessage.CreateAssistantMessage(response.Value.Content[0].Text) }
            });

            // Extract and return the generated SQL query from the response
            return response.Value.Content[0].Text;
        }

        private static string DataTableToString(DataTable dataTable)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataColumn column in dataTable.Columns)
            {
                sb.Append(column.ColumnName + "\t");
            }
            sb.AppendLine();

            foreach (DataRow row in dataTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    sb.Append((item?.ToString() ?? string.Empty) + "\t");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
