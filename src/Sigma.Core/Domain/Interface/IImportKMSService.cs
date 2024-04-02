using Sigma.Core.Domain.Model;

namespace Sigma.Core.Domain.Interface
{
    public interface IImportKMSService
    {
        Task ImportKMSTask(ImportKMSTaskReq req);
    }
}