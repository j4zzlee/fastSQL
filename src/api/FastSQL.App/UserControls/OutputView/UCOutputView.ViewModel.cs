using FastSQL.App.Interfaces;
using FastSQL.Core.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastSQL.App.UserControls.OutputView
{
    public class UCOutputViewViewModel: BaseViewModel
    {
        private ObservableCollection<string> _messages;

        public ObservableCollection<string> Messages
        {
            get => _messages;
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public UCOutputViewViewModel(IEventAggregator eventAggregator)
        {
            Messages = new ObservableCollection<string>();
            eventAggregator.GetEvent<ApplicationOutputEvent>().Subscribe(OnApplicationOutput);
        }

        private void OnApplicationOutput(ApplicationOutputEventArgument obj)
        {
            App.Current.Dispatcher.Invoke(delegate // <--- HERE
            {
                if (Messages.Count >= 100)
                {
                    Messages.RemoveAt(0);
                }
                Messages.Add(obj.Message);
            });
        }
    }
}
