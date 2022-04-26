using Microsoft.Extensions.DependencyInjection;
using Outrage.EventBus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Wpf.Example
{
    /// <summary>
    /// Interaction logic for Settings1.xaml
    /// </summary>
    public partial class Panel1 : UserControl, INotifyPropertyChanged
    {
        ISubscriber settingsSubscriber;

        public Panel1()
        {
            var clientEventBus = App.ServiceProvider.GetRequiredService<IClientEventBus>();
            settingsSubscriber = clientEventBus.Subscribe<ShowSettingsMessage>(ShowSettingsHandler);
            InitializeComponent();
        }

        public bool ShowSettings { get; set; } = false;

        public event PropertyChangedEventHandler? PropertyChanged = default!;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Task ShowSettingsHandler(EventContext context, IMessage message)
        {
            if (this.Visibility == Visibility.Visible)
            {
                this.ShowSettings = !this.ShowSettings;
                OnPropertyChanged(nameof(this.ShowSettings));
            }

            return Task.CompletedTask;
        }
    }
}
