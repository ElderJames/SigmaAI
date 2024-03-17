using AntSK.Domain.Repositories;
using Microsoft.SemanticKernel;

namespace AntSK.Domain.Domain.Interface
{
    public interface IKernelService
    {
        Kernel GetKernelByApp(Apps app);
        Task ImportFunctionsByApp(Apps app, Kernel _kernel);
        Task<string> HistorySummarize(Kernel _kernel, string questions, string history);
    }
}
