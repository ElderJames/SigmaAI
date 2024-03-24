using Sigma.Core.Domain.Interface;
using Sigma.Core.Domain.Model.Dto;
using Microsoft.AspNetCore.Components;

namespace Sigma.Components.Pages.KmsPage
{
    public partial class KmsDetailList
    {
        [Parameter]
        public string KmsId { get; set; }
        [Parameter]
        public string FileId { get; set; }

        [Inject]
        protected IKMService iKMService { get; set; }

        private List<KMFile> _data = new List<KMFile>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _data = await iKMService.GetDocumentByFileID(KmsId, FileId);
        }

        private void NavigateBack()
        {
            NavigationManager.NavigateTo($"/kms/detail/{KmsId}");
        }
    }
}
