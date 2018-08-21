using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.DataGrid;
using FastSQL.Core.UI.Models;
using FastSQL.Sync.Core.Constants;
using FastSQL.Sync.Core.Filters;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.Queues
{
    public class UCQueueContentViewModel : BaseViewModel
    {
        public QueueItemRepository QueueItemRepository { get; set; }
        public EntityRepository EntityRepository { get; set; }
        public AttributeRepository AttributeRepository { get; set; }

        private DataGridViewModel dataGridViewModel;
        public DataGridViewModel QueueItems
        {
            get => dataGridViewModel;
            set
            {
                dataGridViewModel = value;
                dataGridViewModel.SetGridContextMenus(new List<MenuItemDefinition> {
                    new MenuItemDefinition
                    {
                        Name = "Run",
                        CommandName = "Run"
                    }
                });
                dataGridViewModel.OnFilter += DataGridViewModel_OnFilter;
                dataGridViewModel.OnEvent += DataGridViewModel_OnEvent;
                OnPropertyChanged(nameof(QueueItems));
            }
        }

        private async void DataGridViewModel_OnFilter(object sender, FilterArguments args)
        {
            await LoadData(args.Filters, args.Limit, args.Offset, false);
        }

        private void DataGridViewModel_OnEvent(object sender, Events.DataGridCommandEventArgument args)
        {
            var selectedItem = args.SelectedItems
                .Select(i => (IndexItemModel)i)
                .FirstOrDefault();
            switch (args.CommandName)
            {
                case "Run":
                    //syncService.StartTest(selectedItem);
                    //entityRepository.ChangeIndexedItems(_indexModel,
                    //    ItemState.Changed,
                    //    ItemState.Removed | ItemState.Invalid | ItemState.Processed,
                    //    selectedItemIds.ToArray());
                    break;
                case "Stop":
                    //syncService.Stop();
                    break;
            }
            //await LoadData(null, SchedulerOptions.GetLimit(), SchedulerOptions.GetOffset(), true);
        }


        private async Task LoadData(IEnumerable<FilterArgument> filters, int limit, int offset, bool reset)
        {
            await Task.Run(() =>
            {
                var items = QueueItemRepository
                    .FilterQueueItems(filters, limit, offset, out int totalCount);
                if (reset)
                {
                    QueueItems.CleanFilters();
                }
                QueueItems.SetTotalCount(totalCount);
                QueueItems.SetOffset(limit, offset);
                QueueItems.SetData(items);
            });
        }

        public async void Loaded()
        {
            await LoadData(null, DataGridContstants.PageLimit, 0, true);
        }
    }
}
