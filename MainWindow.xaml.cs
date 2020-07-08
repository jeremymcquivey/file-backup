using BackupMonitor.Models;
using BackupMonitor.ViewModels;
using System.Windows;
using System.Windows.Data;

namespace BackupMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = DataContext as MainViewModel;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(OriginalFiles.ItemsSource);
            view.Filter = ItemFilter;
            _viewModel.FileStatusChanged += OriginalFileStatusChanged;
        }

        private void OriginalFileStatusChanged(object sender, Events.FileStatusChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(OriginalFiles.ItemsSource).Refresh();
        }

        private bool ItemFilter(object item)
        {
            var file = (item as File);
            var isBackedUp = file.IsBackedUp;
            return !isBackedUp;
        }

        private void OnBackupClicked(object sender, RoutedEventArgs e)
        {
            if (OriginalFiles.SelectedItems.Count < 1)
            {
                return;
            }

            _viewModel.BackupCommand.Execute(OriginalFiles.SelectedItems);
        }
    }
}
