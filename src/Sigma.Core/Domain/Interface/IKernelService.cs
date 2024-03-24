using Sigma.Core.Repositories;
using Microsoft.SemanticKernel;

namespace Sigma.Core.Domain.Interface
{
    public interface IKernelService
    {
        Kernel GetKernelByApp(Apps app);
        Task ImportFunctionsByApp(Apps app, Kernel _kernel);
        Task<string> HistorySummarize(Kernel _kernel, string questions, string history);
    }
}
