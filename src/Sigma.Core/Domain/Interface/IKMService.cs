using Sigma.Core.Domain.Model.Dto;
using Microsoft.KernelMemory;

namespace Sigma.Core.Domain.Interface
{
    public interface IKMService
    {
        MemoryServerless GetMemoryByKMS(string kmsID, SearchClientConfig searchClientConfig = null);

        Task<List<KMFile>> GetDocumentByFileID(string kmsid, string fileid);

        Task<List<RelevantSource>> GetRelevantSourceList(string kmsIdListStr, string msg);
    }
}