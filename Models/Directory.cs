using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BackupMonitor.Models
{
    public class Directory : INotifyPropertyChanged
    {
        private string _fullPath;
        private string _relativePath;
        private string _absolutePath;

        public event PropertyChangedEventHandler PropertyChanged;

        public string FullPath
        {
            set
            {
                _fullPath = value;
                OnPropertyChanged("DirectoryName");
                OnPropertyChanged("FullPath");
            }
            get
            {
                return _fullPath;
            }
        }

        public string DirectoryName
        {
            get
            {
                return FullPath?.Split('\\').LastOrDefault();
            }
        }

        public string RelativePath
        {
            set
            {
                _relativePath = value;
                OnPropertyChanged("RelativePath");
            }
            get
            {
                return _relativePath;
            }
        }

        public string AbsolutePath
        {
            set
            {
                _absolutePath = value;
                OnPropertyChanged("AbsolutePath");
            }
            get
            {
                return _absolutePath;
            }
        }

        public List<Directory> Directories { private set; get; }

        public List<File> Files { private set; get; }

        public Directory(string fullPath)
        {
            FullPath = fullPath;
            RelativePath = "./";

            Directories = new List<Directory>();
            Files = new List<File>();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
