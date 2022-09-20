using MaJiCSoft.ChatOverlay.Events;
using System.Windows;

namespace MaJiCSoft.ChatOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(EventAggregator aggregator)
        {
            InitializeComponent();
        }
    }
}
