using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.DataGrid;
using FastSQL.Core;
using FastSQL.Sync.Core;
using FastSQL.Sync.Core.Constants;
using FastSQL.Sync.Core.Enums;
using FastSQL.Sync.Core.Filters;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using FastSQL.Sync.Core.Workflows;
using FastSQL.Sync.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.Schedulers
{
    public class UCSchedulerContentViewModel : BaseViewModel
    {
        private DataGridViewModel dataGridViewModel;
        private readonly ScheduleOptionRepository scheduleOptionRepository;
        private readonly EntityRepository entityRepository;
        private readonly AttributeRepository attributeRepository;
        private readonly IEnumerable<IBaseWorkflow> workflows;
        private readonly SyncService syncService;
        private readonly ResolverFactory resolverFactory;

        public BaseCommand SaveCommand => new BaseCommand(o => true, OnSaveOptions);

        public BaseCommand RunWorkflowCommand => new BaseCommand(o => true, OnRunWorkflow);
        public BaseCommand StopWorkflowCommand => new BaseCommand(o => true, OnStopWorkflow);

        public DataGridViewModel SchedulerOptions
        {
            get => dataGridViewModel;
            set
            {
                dataGridViewModel = value;
                dataGridViewModel.SetGridContextMenus(new List<string> {
                    "Run",
                    "Stop"
                });
                dataGridViewModel.OnFilter += DataGridViewModel_OnFilter;
                dataGridViewModel.OnEvent += DataGridViewModel_OnEvent;
                OnPropertyChanged(nameof(SchedulerOptions));
            }
        }

        public UCSchedulerContentViewModel(
            ScheduleOptionRepository scheduleOptionRepository,
            EntityRepository entityRepository,
            AttributeRepository attributeRepository,
            IEnumerable<IBaseWorkflow> workflows,
            SyncService syncService
        )
        {
            this.scheduleOptionRepository = scheduleOptionRepository;
            this.entityRepository = entityRepository;
            this.attributeRepository = attributeRepository;
            this.workflows = workflows;
            this.syncService = syncService;
            syncService.SetMode(WorkflowMode.Test);
        }

        private async void DataGridViewModel_OnFilter(object sender, FilterArguments args)
        {
            await LoadData(args.Filters, args.Limit, args.Offset, false);
        }

        private void DataGridViewModel_OnEvent(object sender, Events.DataGridCommandEventArgument args)
        {
            var selectedItem = args.SelectedItems
                .Select(i => (ScheduleOptionModel)i)
                .FirstOrDefault();
            switch (args.CommandName)
            {
                case "Run":
                    syncService.StartTest(selectedItem);
                    //entityRepository.ChangeIndexedItems(_indexModel,
                    //    ItemState.Changed,
                    //    ItemState.Removed | ItemState.Invalid | ItemState.Processed,
                    //    selectedItemIds.ToArray());
                    break;
                case "Stop":
                    syncService.Stop();
                    break;
            }
            //await LoadData(null, SchedulerOptions.GetLimit(), SchedulerOptions.GetOffset(), true);
        }

        private async Task LoadData(IEnumerable<FilterArgument> filters, int limit, int offset, bool reset)
        {
            await Task.Run(() =>
            {
                var entities = entityRepository.GetAll().Select(e => e as IIndexModel);
                var attributes = attributeRepository.GetAll().Select(e => e as IIndexModel);
                var allIndexes = entities.Union(attributes);
                var items = scheduleOptionRepository
                .FilterOptions(filters, limit, offset, out int totalCount)
                    .Select(o =>
                    {
                        o.TargetEntityName = allIndexes.FirstOrDefault(i => i.Id == o.TargetEntityId && i.EntityType == o.TargetEntityType)?.Name;
                        return o;
                    });
                if (reset)
                {
                    SchedulerOptions.CleanFilters();
                }
                SchedulerOptions.SetTotalCount(totalCount);
                SchedulerOptions.SetOffset(limit, offset);
                SchedulerOptions.SetData(items);
            });
        }

        private void OnRunWorkflow(object obj)
        {
            syncService.Start();
        }

        private void OnStopWorkflow(object obj)
        {
            syncService.Stop();
        }

        private async void OnSaveOptions(object obj)
        {
            await Task.Run(async () =>
            {
                try
                {
                    // It is harder than you think!!!
                    scheduleOptionRepository.BeginTransaction();
                    var entities = entityRepository.GetAll().Select(e => e as IIndexModel);
                    var attributes = attributeRepository.GetAll().Select(e => e as IIndexModel);
                    var allIndexes = entities.Union(attributes);
                    var allOptions = workflows.SelectMany(w => allIndexes.Select(i => new ScheduleOptionModel
                    {
                        Interval = 1,
                        Status = ScheduleStatus.Enabled,
                        Priority = 100,
                        TargetEntityId = i.Id,
                        TargetEntityType = i.EntityType,
                        WorkflowId = w.Id
                    }));

                    // Only get the option that fits available options
                    var changedItems = SchedulerOptions.GetData<ScheduleOptionModel>()
                        .Where(i => allOptions.Any(o => {
                            return i.WorkflowId == o.WorkflowId && i.TargetEntityId == o.TargetEntityId && i.TargetEntityType == o.TargetEntityType;
                        }))
                        .ToList(); 

                    var items = scheduleOptionRepository
                        .GetAll()
                        .Where(i => allOptions.Any(o => i.WorkflowId == o.WorkflowId && i.TargetEntityId == o.TargetEntityId && i.TargetEntityType == o.TargetEntityType))
                        .Select(o =>
                        {
                            // Update with old values
                            var changedItem = changedItems.FirstOrDefault(i => i.Id == o.Id);
                            if (changedItem != null)
                            {
                                changedItem.TargetEntityName = allIndexes.FirstOrDefault(i => i.Id == o.TargetEntityId && i.EntityType == changedItem.TargetEntityType)?.Name;
                                return changedItem;
                            }
                            o.TargetEntityName = allIndexes.FirstOrDefault(i => i.Id == o.TargetEntityId && i.EntityType == o.TargetEntityType)?.Name;
                            return o;
                        })
                        .ToList();
                    
                    foreach (var o in allOptions)
                    {
                        var exists = items.Any(i => i.WorkflowId == o.WorkflowId && i.TargetEntityId == o.TargetEntityId && i.TargetEntityType == o.TargetEntityType);
                        if (!exists)
                        {
                            items.Add(o);
                        }
                    }
                    scheduleOptionRepository.DeleteByIds<ScheduleOptionModel>(items
                        .Where(i => !allOptions.Any(o => i.WorkflowId == o.WorkflowId && i.TargetEntityId == o.TargetEntityId && i.TargetEntityType == o.TargetEntityType))
                        .Select(i => i.Id.ToString()));
                    scheduleOptionRepository.BuildOptions(items);
                    scheduleOptionRepository.Commit();
                }
                catch
                {
                    scheduleOptionRepository.RollBack();
                    throw;
                }
                finally
                {
                    await LoadData(null, SchedulerOptions.GetLimit(), SchedulerOptions.GetOffset(), true);
                }
            });
        }

        public async void Loaded()
        {
            await LoadData(null, DataGridContstants.PageLimit, 0, true);
        }
    }
}
