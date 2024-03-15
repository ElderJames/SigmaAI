using AntSK.Domain.Domain.Interface;
using AntSK.Domain.Domain.Model;
using AntSK.Domain.Domain.Model.Enum;
using AntSK.Domain.Repositories;
using Coravel.Queuing.Interfaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace AntSK.Controllers
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="_taskBroker"></param>
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class KMSController : ControllerBase
    {
        private readonly IKmsDetails_Repositories _kmsDetails_Repositories;
        private readonly IKMService _iKMService;
        private IQueue _queue;
        private IImportKMSService _importKMSService;

        public KMSController(
            IKmsDetails_Repositories kmsDetails_Repositories,
            IKMService iKMService
            )
        {
            _kmsDetails_Repositories = kmsDetails_Repositories;
            _iKMService = iKMService;
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
                CreateTime = DateTime.Now,
                Status = ImportKmsStatus.Loadding,
                Type = model.ImportType.ToString().ToLower()
            };

            _kmsDetails_Repositories.Insert(detail);
            req.KmsDetail = detail;

            this._queue.QueueAsyncTask(async () =>
            {
                _importKMSService.ImportKMSTask(req);
            });

            //_taskBroker.QueueWorkItem(req);
            Console.WriteLine("api/kms/ImportKMSTask  结束");
            return Ok();
        }
    }
}