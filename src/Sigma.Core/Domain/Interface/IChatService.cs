using Sigma.Core.Domain.Model.Dto;
using Sigma.Core.Repositories;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sigma.Core.Domain.Model;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Sigma.Core.Domain.Interface
{
    public interface IChatService
    {
        IAsyncEnumerable<StreamingKernelContent> SendChatByAppAsync(Apps app, string questions, ChatHistory history);

        IAsyncEnumerable<StreamingKernelContent> SendKmsByAppAsync(Apps app, string questions, ChatHistory history, List<RelevantSource> relevantSources = null);

        ChatHistory GetChatHistory(List<Chat.ChatHistory> history);
    }
}
