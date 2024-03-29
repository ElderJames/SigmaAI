using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Services;
using Microsoft.SemanticKernel.TextGeneration;
using Sdcb.SparkDesk;
using System;
using System.ComponentModel;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace Sigma.LLM.SparkDesk
{
    public class SparkDeskTextCompletion : ITextGenerationService, IAIService
    {
        private readonly Dictionary<string, object?> _attributes = new();
        private readonly SparkDeskClient _client;
        private string _chatId;
        private readonly SparkDeskOptions _options;

        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public IReadOnlyDictionary<string, object?> Attributes => _attributes;

        public SparkDeskTextCompletion(SparkDeskOptions options, string chatId)
        {
            _options = options;
            _chatId = chatId;
            _client = new(options.AppId, options.ApiKey, options.ApiSecret);
        }

        public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            StringBuilder sb = new();
            var parameters = new ChatRequestParameters
            {
                ChatId = _chatId,
            };

            OpenAIPromptExecutionSettings chatExecutionSettings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            parameters.Temperature = (float)chatExecutionSettings.Temperature;
            parameters.MaxTokens = chatExecutionSettings.MaxTokens ?? parameters.MaxTokens;

            var messages = PromptHelper.GetHistories(prompt).Select(m => new ChatMessage(m.Role, m.Message)).ToArray();

            await foreach (StreamedChatResponse msg in _client.ChatAsStreamAsync(_options.ModelVersion, messages, parameters))
            {
                sb.Append(msg);
            };

            return [new(sb.ToString())];
        }

        public IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(string prompt, PromptExecutionSettings? executionSettings = null, Kernel? kernel = null, CancellationToken cancellationToken = default)
        {
            var parameters = new ChatRequestParameters
            {
                ChatId = _chatId,
            };
            OpenAIPromptExecutionSettings chatExecutionSettings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

            parameters.Temperature = (float)chatExecutionSettings.Temperature;
            parameters.MaxTokens = chatExecutionSettings.MaxTokens ?? parameters.MaxTokens;

            IList<KernelFunctionMetadata> functions = kernel?.Plugins.GetFunctionsMetadata().Where(x => x.PluginName == "SigmaFunctions").ToList() ?? [];
            var functionDefs = functions.Select(func => new FunctionDef(func.Name, func.Description, func.Parameters.Select(p => new FunctionParametersDef(p.Name, p.ParameterType?.IsClass == true ? "object" : "string", func.Description, p.IsRequired)).ToList())).ToList();

            var messages = PromptHelper.GetHistories(prompt).Select(m => new ChatMessage(m.Role, m.Message)).ToArray();

            return GetStreamingMessageAsync();

            async IAsyncEnumerable<StreamingTextContent> GetStreamingMessageAsync()
            {
                await foreach (StreamedChatResponse msg in _client.ChatAsStreamAsync(_options.ModelVersion, messages, parameters, functionDefs.Count > 0 ? [.. functionDefs] : null, cancellationToken: cancellationToken))
                {
                    if (msg.FunctionCall != null)
                    {
                        var func = functions.Where(x => x.Name == msg.FunctionCall.Name).FirstOrDefault();

                        if (func == null)
                        {
                            yield return new($"插件{msg.FunctionCall.Name}未注册");
                            yield break;
                        }

                        if (kernel.Plugins.TryGetFunction(func.PluginName, func.Name, out var function))
                        {
                            var JsonElement = JsonDocument.Parse(msg.FunctionCall.Arguments).RootElement;

                            var parameters = JsonParameterParser.ParseJsonToDictionary(JsonElement, func.Parameters.ToDictionary(x => x.Name, x => x.ParameterType!));
                            var arguments = new KernelArguments(parameters);
                            var result = (await function.InvokeAsync(kernel, arguments, cancellationToken)).GetValue<object>() ?? string.Empty;
                            var stringResult = ProcessFunctionResult(result, chatExecutionSettings.ToolCallBehavior);
                            messages = [ChatMessage.FromSystem($"""
                                请结合用户问题与意图结果总结陈词

                                用户意图{func.Description},结果是{stringResult}
                                """),
                               messages.LastOrDefault()];

                            functionDefs = [];

                            await foreach (var content in GetStreamingMessageAsync())
                            {
                                yield return content;
                            }
                        }
                    }
                    else
                    {
                        yield return new(msg);
                    }
                };
            }
        }

        private static string? ProcessFunctionResult(object functionResult, ToolCallBehavior? toolCallBehavior)
        {
            if (functionResult is string stringResult)
            {
                return stringResult;
            }

            if (functionResult is ChatMessageContent chatMessageContent)
            {
                return chatMessageContent.ToString();
            }

            if (functionResult is RestApiOperationResponse { } response)
            {
                return ProcessFunctionResult(response.Content, toolCallBehavior);
            }

            return JsonSerializer.Serialize(functionResult, _jsonSerializerOptions);
        }

        public static Dictionary<string, object> ParseJsonElement(JsonElement element, string propertyName)
        {
            Dictionary<string, object> dict = new();

            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (JsonProperty property in element.EnumerateObject())
                    {
                        dict.Add(property.Name, ParseJsonElement(property.Value, property.Name));
                    }
                    break;

                case JsonValueKind.Array:
                    List<object> list = new List<object>();
                    foreach (JsonElement arrayElement in element.EnumerateArray())
                    {
                        list.Add(ParseJsonElement(arrayElement, ""));
                    }
                    dict.Add(propertyName, list);
                    break;

                case JsonValueKind.String:
                    dict.Add(propertyName, element.GetString());
                    break;

                case JsonValueKind.Number:
                    dict.Add(propertyName, element.GetInt32());
                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    dict.Add(propertyName, element.GetBoolean());
                    break;

                default:
                    dict.Add(propertyName, "Unsupported value type");
                    break;
            }

            return dict;
        }
    }
}