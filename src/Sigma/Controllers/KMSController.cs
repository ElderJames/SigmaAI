using Sigma.Core.Domain.Interface;
using Sigma.Core.Domain.Model;
using Sigma.Core.Domain.Model.Enum;
using Sigma.Core.Repositories;
using Coravel.Queuing.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Sigma.Controllers
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="_taskBroker"></param>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class KMSController : ControllerBase
    {
        private readonly IKmsDetails_Repositories _kmsDetails_Repositories;
        private readonly IKMService _iKMService;
        private readonly IQueue _queue;
        private readonly IImportKMSService _importKMSService;
        private readonly IServiceScopeFactory _serviceScopeFactory;


        public KMSController(
            IKmsDetails_Repositories kmsDetails_Repositories,
            IKMService iKMService, 
            IQueue queue,
            IServiceScopeFactory serviceScopeFactory)
        {
            _kmsDetails_Repositories = kmsDetails_Repositories;
            _iKMService = iKMService;
            _serviceScopeFactory = serviceScopeFactory;
            _queue = queue;
        }

        [HttpPost]
        public async Task<IActionResult> ImportKMSTask(ImportKMSTaskDTO model)
        {
            Console.WriteLine("api/kms/ImportKMSTask  开始");
            ImportKMSTaskReq req = model.Adapt<ImportKMSTaskReq>();
            KmsDetails detail = new KmsDetails()
            {
                Id = Guid.NewGuid().ToString(),
                KmsId = req.KmsId.ToString(),
                CreatedAt = DateTime.Now,
                Status = ImportKmsStatus.Loadding,
                Type = model.ImportType.ToString().ToLower()
            };

            _kmsDetails_Repositories.Insert(detail);
            req.KmsDetail = detail;

            this._queue.QueueAsyncTask(() =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var importService = scope.ServiceProvider.GetRequiredService<IImportKMSService>();
                importService.ImportKMSTask(req);
                return Task.CompletedTask;
            });

            //_taskBroker.QueueWorkItem(req);
            Console.WriteLine("api/kms/ImportKMSTask  结束");
            return Ok();
        }
    }
}