using FastSQL.App.Interfaces;
using FastSQL.App.UserControls.DataGrid;
using FastSQL.Sync.Core.IndexExporters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.Entities
{
    public class ManageEntityViewModel: BaseViewModel
    {
        private DataGridViewModel dataGridViewModel;
        private readonly IEnumerable<IIndexExporter> indexExporters;

        public DataGridViewModel DataGridViewModel
        {
            get => dataGridViewModel;
            set
            {
                dataGridViewModel = value;
                OnPropertyChanged(nameof(DataGridViewModel));
            }
        }

        public ObservableCollection<IIndexExporter> Exporters
        {
            get => new ObservableCollection<IIndexExporter>(indexExporters);
        }
        public ManageEntityViewModel(DataGridViewModel dataGridViewModel, IEnumerable<IIndexExporter> indexExporters)
        {
            this.DataGridViewModel = dataGridViewModel;
            this.indexExporters = indexExporters;

            this.DataGridViewModel.SetGridContextMenus(new List<string> { "Change" });
        }
    }
}
