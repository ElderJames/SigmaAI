using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntDesign.TableModels;
using Elsa.Api.Client.Shared.Models;
using Elsa.Studio.DomInterop.Contracts;
using Elsa.Studio.Workflows.Domain.Contracts;
using Microsoft.AspNetCore.Components;
using Elsa.Api.Client.Resources.WorkflowDefinitions.Requests;
using Elsa.Api.Client.Resources.WorkflowDefinitions.Enums;
using Elsa.Studio.Components;
using AntDesign;

namespace Sigma.Client.Components.Workflows;

public partial class WorkflowDefinitionList : StudioComponentBase
{
    private IEnumerable<WorkflowDefinitionRow> _selectedRows = [];
    private int _totalCount;

    /// <summary>
    /// An event that is invoked when a workflow definition is edited.
    /// </summary>
    [Parameter] public EventCallback<string> EditWorkflowDefinition { get; set; }

    [Inject] private IWorkflowDefinitionService WorkflowDefinitionService { get; set; } = default!;
    [Inject] private IWorkflowInstanceService WorkflowInstanceService { get; set; } = default!;
    [Inject] private IFiles Files { get; set; } = default!;
    [Inject] private IDomAccessor DomAccessor { get; set; } = default!;
    
    [Inject] private ModalService ModalService { get; set; } = default!;
    [Inject] private MessageService MessageService { get; set; } = default!;

    private string SearchTerm { get; set; } = string.Empty;
    private List<WorkflowDefinitionRow> _data;

    private  async Task OnTableChange(QueryModel<WorkflowDefinitionRow> queryModel)
    {
        var request = new ListWorkflowDefinitionsRequest
        {
            IsSystem = false,
            Page = queryModel.PageIndex - 1,
            PageSize = queryModel.PageSize,
            SearchTerm = SearchTerm,
            OrderBy = GetOrderBy(queryModel.SortModel.FirstOrDefault()?.FieldName),
            OrderDirection = queryModel.SortModel.FirstOrDefault()?.Sort == "descend"
             ? OrderDirection.Descending
             : OrderDirection.Ascending
        };


        _data = await base.InvokeWithBlazorServiceContext(async () =>
        {
            var latestWorkflowDefinitionsResponse = await WorkflowDefinitionService.ListAsync(request, VersionOptions.Latest);
            var unpublishedWorkflowDefinitionIds = latestWorkflowDefinitionsResponse.Items.Where(x => !x.IsPublished).Select(x => x.DefinitionId).ToList();

            var publishedWorkflowDefinitions = await WorkflowDefinitionService.ListAsync(new ListWorkflowDefinitionsRequest
            {
                DefinitionIds = unpublishedWorkflowDefinitionIds,
            }, VersionOptions.Published);

            _totalCount = (int)latestWorkflowDefinitionsResponse.TotalCount;

            var latestWorkflowDefinitions = latestWorkflowDefinitionsResponse.Items
                .Select(definition =>
                {
                    var latestVersionNumber = definition.Version;
                    var isPublished = definition.IsPublished;
                    var publishedVersion = isPublished
                        ? definition
                        : publishedWorkflowDefinitions.Items.FirstOrDefault(x => x.DefinitionId == definition.DefinitionId);
                    var publishedVersionNumber = publishedVersion?.Version;

                    return new WorkflowDefinitionRow(
                        definition.Id,
                        definition.DefinitionId,
                        latestVersionNumber,
                        publishedVersionNumber,
                        definition.Name,
                        definition.Description,
                        definition.IsPublished);
                })
                .ToList();

            return latestWorkflowDefinitions;
        });
    }

    private OrderByWorkflowDefinition? GetOrderBy(string? sortLabel)
    {
        return sortLabel switch
        {
            "Name" => OrderByWorkflowDefinition.Name,
            "Version" => OrderByWorkflowDefinition.Version,
            "Created" => OrderByWorkflowDefinition.Created,
            _ => null
        };
    }

	private async Task OnCreateWorkflowClicked()
    {
		var workflowName = await WorkflowDefinitionService.GenerateUniqueNameAsync();

        var modalRef = await ModalService.CreateModalAsync(new()
        {
            Content = WorkflowEdotForm(workflowName),
            OnOk = async (e) =>
            {
                if (!form.Validate())
                {
                    return;
                }
                var newWorkflowModel = model;
                var result = await InvokeWithBlazorServiceContext((() => WorkflowDefinitionService.CreateNewDefinitionAsync(newWorkflowModel.Name!, newWorkflowModel.Description!)));

                await result.OnSuccessAsync(definition => EditAsync(definition.DefinitionId));
                result.OnFailed(errors => _ = MessageService.Error(string.Join(Environment.NewLine, errors.Errors)));
            }
        });
    }

    private async Task OnRowClick(RowData<WorkflowDefinitionRow> rowData)
    {
        await EditAsync(rowData.Data.DefinitionId);
    }

    private async Task EditAsync(string definitionId)
    {
        await EditWorkflowDefinition.InvokeAsync(definitionId);
    }

    private record WorkflowDefinitionRow(
        string Id,
        string DefinitionId,
        int LatestVersion,
        int? PublishedVersion,
        string? Name,
        string? Description,
        bool IsPublished);
}


