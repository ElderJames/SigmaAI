using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntSK.Domain.Utils;
using Microsoft.KernelMemory;
using Markdig;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Dto;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.TypeChat;
using AntDesign;
using Sigma.Core;
using LLMJson;
using Sigma.Core.Domain.Model.Dto;

namespace AntSK.Domain.Domain.Service
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

            if (app.AIModel != null && app.AIModel.UseIntentionRecognition)
            {
                prompt = GenerateFuncionPrompt(_kernel) + prompt;

                //var pluginApi = new PluginApi(_kernel);
                //var pluginSchema = pluginApi.TypeInfo.ExportSchema(pluginApi.TypeName);
                //var translator = new ProgramTranslator(
                //    _kernel.ChatLanguageModel(app.AIModel.ModelDescription ?? "model"),
                //    new ProgramValidator(new PluginProgramValidator(pluginApi.TypeInfo)),
                //    pluginSchema
                //);
                //translator.MaxRepairAttempts = 2;
                //var interpreter = new ProgramInterpreter();
                //using Program program = await translator.TranslateAsync(questions);

                //string? result = await interpreter.RunAsync(program, pluginApi.InvokeAsync);

                //questions += $"""
                //    user: 以上的结果是：{result}，请重新组织语言回复。
                //    """;
            }

            var func = _kernel.CreateFunctionFromPrompt(prompt, settings);
            var chatResult = _kernel.InvokeStreamingAsync(function: func, arguments: new KernelArguments() { ["input"] = $"{history}{Environment.NewLine} user:{questions}" });

            var result = "";
            var successMatch = false;
            await foreach (var content in chatResult)
            {
                if (app.AIModel?.UseIntentionRecognition == true)
                {
                    result += content.ToString();

                    if (result.Length > 5 && result.Contains("func"))
                    {
                        successMatch = true;
                        continue;
                    }
                }

                yield return content;
            }

            if (successMatch)
            {
                var functioResult = new FunctionSchema();
                result.FromJson(functioResult);
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
            var functions = kernel?.Plugins.GetFunctionsMetadata().Where(x => x.PluginName == "AntSkFunctions").ToList() ?? [];
            if (!functions.Any())
                return "";

            var functionNames = functions.Select(x => x.Description).ToList();
            var functionKV = functions.ToDictionary(x => x.Description, x => new { Function = $"{x.Name}", Parameters = x.Parameters.Select(x => $"{x.Name}:{x.ParameterType?.Name}") });
            var template = $$"""
                          请完成意图识别任务，已知的意图有{{JsonSerializer.Serialize(functionNames, JsonSerializerOptions)}}，分别对应的函数如下：
                          {{JsonSerializer.Serialize(functionKV, JsonSerializerOptions)}}

                          请直接给出json对象,不要输出 markdown 及其他多余文字。
                          
                          {
                             "function": string   // 意图对应的function
                             "intention": string  // 用户的意图
                             "arguments: object   // 传入参数
                          }
                         
                          """;

            return template;
        }

    }
}