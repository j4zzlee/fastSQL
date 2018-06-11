using FastSQL.App.Events;
using FastSQL.App.Interfaces;
using FastSQL.Sync.Core.Models;
using FastSQL.Sync.Core.Repositories;
using Prism.Events;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FastSQL.App.UserControls.Reporters
{
    public class UCRepoterListViewViewModel: BaseViewModel
    {

        private readonly ReporterRepository reporterRepository;
        private readonly IEventAggregator eventAggregator;
        private ReporterModel _selectedReporter;
        private ObservableCollection<ReporterModel> _reporters;

        public BaseCommand SelectItemCommand => new BaseCommand(o => true, o => {
            var id = Guid.Parse(o.ToString());
            eventAggregator.GetEvent<SelectReporterEvent>().Publish(new SelectReporterEventArgument
            {
                ReporterId = id
            });
        });

        public ObservableCollection<ReporterModel> Reporters
        {
            get
            {
                return _reporters;
            }
            set
            {
                _reporters = value ?? new ObservableCollection<ReporterModel>();
                OnPropertyChanged(nameof(Reporters));
            }
        }

        public ReporterModel SelectedChannel
        {
            get { return _selectedReporter; }
            set
            {
                _selectedReporter = value;

                OnPropertyChanged(nameof(SelectedChannel));
            }
        }

        public UCRepoterListViewViewModel(
            ReporterRepository reporterRepository,
            IEventAggregator eventAggregator)
        {
            this.reporterRepository = reporterRepository;
            this.eventAggregator = eventAggregator;
            Reporters = new ObservableCollection<ReporterModel>(reporterRepository.GetAll());
            eventAggregator.GetEvent<RefreshReporterListEvent>().Subscribe(OnRefreshReporters);
            var firstConection = Reporters.FirstOrDefault();
            if (firstConection != null)
            {
                eventAggregator.GetEvent<SelectReporterEvent>().Publish(new SelectReporterEventArgument
                {
                    ReporterId = firstConection.Id
                });
            }
        }

        private void OnRefreshReporters(RefreshReporterListEventArgument obj)
        {
            Reporters = new ObservableCollection<ReporterModel>(reporterRepository.GetAll());
            var selectedId = obj.SelectedReporterId;
            if (obj.SelectedReporterId == Guid.Empty)
            {
                var firstConnection = Reporters.FirstOrDefault();
                selectedId = firstConnection?.Id ?? Guid.Empty;
            }
            if (selectedId != Guid.Empty)
            {
                eventAggregator.GetEvent<SelectReporterEvent>().Publish(new SelectReporterEventArgument
                {
                    ReporterId = selectedId
                });
            }
        }

        public void Loaded()
        {
        }
    }
}
