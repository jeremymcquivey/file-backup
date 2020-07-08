using BackupMonitor.Events;
using BackupMonitor.Models;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BackupMonitor.ViewModels
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private ICommand _scanCommand;
        private ICommand _backupCommand;
        private BackupProfile _backupProfile;
        private bool _isRunning = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public event FileStatusChangedEventHandler FileStatusChanged;

        public ObservableCollection<File> OriginalFiles { private set; get; }

        public ObservableCollection<File> BackupFiles { private set; get; }

        public BackupProfile BackupProfile
        {
            private set
            {
                _backupProfile = value;
                OnPropertyChanged(nameof(BackupProfile));
            }
            get
            {
                return _backupProfile;
            }
        }

        public ICommand ScanCommand => _scanCommand ?? (_scanCommand = new Command(async (obj) => await SetupFiles()));

        public ICommand BackupCommand => _backupCommand ?? (_backupCommand = new Command(async (selectedFiles) => await BackupSelected((IList)selectedFiles)));

        public int NumberOfUnbackedUp => OriginalFiles.Count(x => !x.IsBackedUp);

        public int NumberOfOutOfDate => OriginalFiles.Count(x => x.IsOutOfDate);

        public int TotalNumberOfFiles { get; private set; } = 0;

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            private set
            {
                _isRunning = value;
                OnPropertyChanged(nameof(IsRunning));
            }
        }

        public MainViewModel()
        {
            BackupProfile = new BackupProfile()
            {
                BackupDirectoryType = typeof(LocalFileStorageAdapter).FullName,
                OriginalDirectoryType = typeof(LocalFileStorageAdapter).FullName,
                Name = ConfigurationManager.AppSettings.GetValues("ProfileName").FirstOrDefault(),
                BackupPath = ConfigurationManager.AppSettings.GetValues("BackupDirectory").FirstOrDefault(),
                OriginalPath = ConfigurationManager.AppSettings.GetValues("OriginalDirectory").FirstOrDefault(),
            };

            BackupProfile.OriginalDirectory.PathFromRoot = BackupProfile.OriginalPath;
            BackupProfile.BackupDirectory.PathFromRoot = BackupProfile.BackupPath;

            App.Current.Dispatcher.Invoke(() =>
            {
                OriginalFiles = new ObservableCollection<File>();
                BackupFiles = new ObservableCollection<File>();
            });

            ScanCommand.Execute(null);
        }

        private async Task BackupSelected(IList selectedFiles)
        {
            SetRunning(true);
            await Task.Run(() =>
            {
                var allFiles = new ArrayList(selectedFiles);
                foreach (var file in allFiles)
                {
                    try
                    {
                        var item = OriginalFiles.FirstOrDefault(i => i == file);
                        if (item != null)
                        {
                            var destinationPath = System.IO.Path.Combine(BackupProfile.BackupDirectory.PathFromRoot, item.RelativePath);
                            var destinationFile = System.IO.Path.Combine(destinationPath, item.Filename);

                            if (!System.IO.Directory.Exists(destinationPath))
                            {
                                System.IO.Directory.CreateDirectory(destinationPath);
                            }

                            System.IO.File.Copy(item.FullPath, destinationFile, true);
                            item.Status = BackupProfile.BackupDirectory.Compare(item); 
                            
                            FileStatusChanged?.Invoke(this, new FileStatusChangedEventArgs
                            {
                                UpdatedFile = item
                            });

                            App.Current.Dispatcher.Invoke(() =>
                            {
                                /*if (item.Status == BackupStatus.BackedUp)
                                {
                                    OriginalFiles.Remove(item);
                                }*/

                                BackupFiles.Add(item);
                                OnPropertyChanged(nameof(NumberOfUnbackedUp));
                                OnPropertyChanged(nameof(NumberOfOutOfDate));
                            });
                        }
                    }
                    catch (Exception) { }
                }
            });
            SetRunning(false);
        }

        private void SetRunning(bool isRunning)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                IsRunning = isRunning;
            });
        }

        private async Task SetupFiles()
        {
            var backupDirectory = new Directory(BackupProfile.BackupDirectory.PathFromRoot);
            var originalDirectory = new Directory(BackupProfile.OriginalDirectory.PathFromRoot);

            App.Current.Dispatcher.Invoke(() =>
            {
                BackupFiles.Clear();
                OriginalFiles.Clear();
            });

            SetRunning(true);
            await ScanForfiles(originalDirectory, OriginalFiles);
            SetRunning(false);

            App.Current.Dispatcher.Invoke(() =>
            {
                OnPropertyChanged(nameof(NumberOfUnbackedUp));
                OnPropertyChanged(nameof(NumberOfOutOfDate));
                OnPropertyChanged(nameof(TotalNumberOfFiles));
            });
        }

        private async Task ScanForfiles(Directory directory, ObservableCollection<File> collection)
        {
            await Task.Run(async () =>
            {
                var directories = BackupProfile.OriginalDirectory.GetDirectories(directory);

                foreach (var dir in directories)
                {
                    await ScanForfiles(dir, collection);
                }

                var files = BackupProfile.OriginalDirectory.GetFiles(directory);

                foreach (var file in files)
                {
                    file.Status = BackupProfile.BackupDirectory.Compare(file);

                    App.Current.Dispatcher.Invoke(() =>
                    {
                        TotalNumberOfFiles++;
                        directory.Files.Add(file);
                        OnPropertyChanged(nameof(TotalNumberOfFiles));

                        if(file.Status != BackupStatus.BackedUp)
                        {
                            collection.Add(file);
                            OnPropertyChanged(nameof(NumberOfUnbackedUp));
                            OnPropertyChanged(nameof(NumberOfOutOfDate));
                        }
                    });
                }
            });
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
