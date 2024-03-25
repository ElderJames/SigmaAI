using Sigma.Core.Domain.Model;

namespace Sigma.Core.Domain.Interface
{
    public interface IImportKMSService
    {
        void ImportKMSTask(ImportKMSTaskReq req);
    }
}
