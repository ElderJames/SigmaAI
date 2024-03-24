using Sigma.Core.Domain.Interface;
using Sigma.Core.Repositories;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.Text;
using Sigma.Core.Utils;
using Sigma.Core.Domain.Model.Dto;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using LLMJson;
using Sigma.Core.Domain.Model.Dto;

namespace Sigma.Core.Domain.Service
{
    public class ChatService(
        IKernelService _kernelService,
        IKMService _kMService,
        IKmsDetails_Repositories _kmsDetails_Repositories
        ) : IChatService
    {
        JsonSerializerOptions JsonSerializerOptions = new()
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="app"></param>
        /// <param name="questions"></param>
        /// <param name="history"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<StreamingKernelContent> SendChatByAppAsync(Apps app, string questions, string history)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            var temperature = app.Temperature / 100;//存的是0~100需要缩小
            OpenAIPromptExecutionSettings settings = new() { Temperature = temperature };
            var useIntentionRecognition = app.AIModel?.UseIntentionRecognition == true;

            if (!string.IsNullOrEmpty(app.ApiFunctionList) || !string.IsNullOrEmpty(app.NativeFunctionList))//这里还需要加上本地插件的
            {
                await _kernelService.ImportFunctionsByApp(app, _kernel);
                settings.ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions;
            }

            if (string.IsNullOrEmpty(app.Prompt) || !app.Prompt.Contains("{{$input}}"))
            {
                //如果模板为空，给默认提示词
                app.Prompt = app.Prompt?.ConvertToString() + "{{$input}}";
            }

            var prompt = app.Prompt;

            if (useIntentionRecognition)
            {
                prompt = GenerateFuncionPrompt(_kernel) + prompt;
            }

            await foreach (var content in Execute())
                yield return content;

            async IAsyncEnumerable<StreamingKernelContent> Execute()
            {
                var func = _kernel.CreateFunctionFromPrompt(prompt, settings);
                var chatResult = _kernel.InvokeStreamingAsync(function: func, arguments: new KernelArguments() { ["input"] = $"{history}{Environment.NewLine} user:{questions}" });

                if (!useIntentionRecognition)
                {
                    await foreach (var content in chatResult)
                        yield return content;

                    yield break;
                }

                var result = "";
                var successMatch = false;

                List<StreamingKernelContent> contentBuffer = [];

                await foreach (var content in chatResult)
                {
                    result += content.ToString();
                    successMatch = result.Contains("func", StringComparison.InvariantCultureIgnoreCase);

                    // wait for function until the result lenght is more than 20
                    if (result.Length > 20 && !successMatch)
                    {
                        if (contentBuffer.Count > 0)
                        {
                            foreach (var c in contentBuffer)
                                yield return c;

                            contentBuffer.Clear();
                        }

                        yield return content;
                    }
                    else
                    {
                        contentBuffer.Add(content);
                    }
                }

                if (!successMatch)
                {
                   foreach (var c in contentBuffer)
                        yield return c;

                    yield break;
                }

                var functioResults = JsonParser.FromJson<List<FunctionSchema>>(result);

                var callResult = new List<string>();

                foreach (var functioResult in functioResults)
                {
                    var plugin = _kernel?.Plugins.GetFunctionsMetadata().Where(x => x.PluginName == "SigmaFunctions").ToList().FirstOrDefault(f => f.Name == functioResult.Function);
                    if (plugin == null)
                    {
                        yield break;
                    }

                    if (!_kernel.Plugins.TryGetFunction(plugin.PluginName, plugin.Name, out var function))
                    {
                        yield break;
                    }
                    var arguments = new KernelArguments(functioResult.Arguments);
                    var funcResult = (await function.InvokeAsync(_kernel, arguments)).GetValue<object>() ?? string.Empty;
                    callResult.Add($"用户意图{functioResult.Reason}结果是{JsonSerializer.Serialize(funcResult, JsonSerializerOptions)}");
                }


                history = $"""
                    system: {string.Join("\r\n", callResult)}。
                    {history}
                    
                    请结合用户最后的问题作答：
                    """;
                
                //questions = "请将这个结果重新组织语言";
                prompt = "{{$input}}";
                useIntentionRecognition = false;

                await foreach (var content in Execute())
                    yield return content;
            }
        }

        public async IAsyncEnumerable<StreamingKernelContent> SendKmsByAppAsync(Apps app, string questions, string history, List<RelevantSource> relevantSources = null)
        {
            var _kernel = _kernelService.GetKernelByApp(app);
            var relevantSourceList = await _kMService.GetRelevantSourceList(app.KmsIdList, questions);
            var dataMsg = new StringBuilder();
            if (relevantSourceList.Any())
            {
                relevantSources?.AddRange(relevantSourceList);
                foreach (var item in relevantSources)
                {
                    dataMsg.AppendLine(item.ToString());
                }
                KernelFunction jsonFun = _kernel.Plugins.GetFunction("KMSPlugin", "Ask");
                var chatResult = _kernel.InvokeStreamingAsync(function: jsonFun,
                    arguments: new KernelArguments() { ["doc"] = dataMsg, ["history"] = history, ["questions"] = questions });

                await foreach (var content in chatResult)
                {
                    yield return content;
                }
            }
            else
            {
                yield return new StreamingTextContent("知识库未搜索到相关内容");
            }
        }

        private string GenerateFuncionPrompt(Kernel kernel)
        {
            var functions = kernel?.Plugins.GetFunctionsMetadata().Where(x => x.PluginName == "SigmaFunctions").ToList() ?? [];
            if (!functions.Any())
                return "";

            var functionNames = functions.Select(x => x.Description).ToList();
            var functionKV = functions.ToDictionary(x => x.Description, x => new { Function = $"{x.Name}", Parameters = x.Parameters.Select(x => $"{x.Name}:{x.ParameterType?.Name}({(x.ParameterType?.IsArray == true ? "数组" : "非数组")})") });
            var template = $$"""
                          请对用户的最后一个提问完成意图识别任务。已知的意图有{{JsonSerializer.Serialize(functionNames, JsonSerializerOptions)}}，分别对应的函数如下：
                          {{JsonSerializer.Serialize(functionKV, JsonSerializerOptions)}}，其中 Parameters 的括号只表示参数类型和是否数组，不是参数名的一部分。

                          从已知的意图中识别出一个或多个意图，并直接给出以下json格式的对象，不要输出 markdown 及其他多余文字。 
                          
                          输出格式如下：

                          [{
                             "function": string   // 意图对应的function
                             "intention": string  // 用户的意图
                             "arguments": object  // 参数
                             "reason": string     // 问题中体现这参数的关键词
                          },{
                             "function": string   // 意图对应的function
                             "intention": string  // 用户的意图
                             "arguments": object  // 参数
                             "reason": string     // 问题中体现这参数的关键词
                          }]

                          
                          如果用户意图无法识别，则直接回答用户的问题，只输出markdown，不要有其他多余文字。
                          """;

            return template;
        }

    }
}