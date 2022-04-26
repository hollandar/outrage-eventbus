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
    public enum ShowingEnum { settings1, settings2 }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IClientEventBus clientEventBus;

        public ShowingEnum Showing { get; set; } = ShowingEnum.settings1;

        public bool ShowSettings1 => Showing == ShowingEnum.settings1;
        public bool ShowSettings2 => Showing == ShowingEnum.settings2;


        public MainWindow()
        {
            DataContext = this;
            this.clientEventBus = App.ServiceProvider.GetRequiredService<IClientEventBus>();
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged = default!;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Settings1_Click(object sender, RoutedEventArgs e)
        {
            this.Showing = ShowingEnum.settings1;
            this.OnPropertyChanged(nameof(ShowSettings1));
            this.OnPropertyChanged(nameof(ShowSettings2));
        }

        private void Settings2_Click(object sender, RoutedEventArgs e)
        {
            this.Showing = ShowingEnum.settings2;
            this.OnPropertyChanged(nameof(ShowSettings1));
            this.OnPropertyChanged(nameof(ShowSettings2));
        }


        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            clientEventBus.PublishAsync<ShowSettingsMessage>();
        }
    }
}
