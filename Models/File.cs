using System;
using System.ComponentModel;

namespace BackupMonitor.Models
{
    public class File : INotifyPropertyChanged
    {
        private string _filename;
        private string _relativePath;
        private string _absolutePath;
        private DateTime? _modified;
        private BackupStatus _backupStatus;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Filename
        {
            set
            {
                _filename = value;
                OnPropertyChanged("Filename");
                OnPropertyChanged("FullPath");
            }
            get
            {
                return _filename;
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
                OnPropertyChanged("FullPath");
            }
            get
            {
                return _absolutePath;
            }
        }

        public bool IsBackedUp
        {
            get
            {
                return Status == BackupStatus.BackedUp;
            }
        }

        public bool IsOutOfDate
        {
            get
            {
                return Status == BackupStatus.OutOfDate;
            }
        }

        public DateTime? Modified
        {
            set
            {
                _modified = value;
                OnPropertyChanged("Modified");
            }
            get
            {
                return _modified;
            }
        }

        public string FullPath
        {
            get
            {
                return System.IO.Path.Combine(AbsolutePath, Filename);
            }
        }

        public BackupStatus Status
        {
            get
            {
                return _backupStatus;
            }
            set
            {
                _backupStatus = value;
                OnPropertyChanged("Status");
                OnPropertyChanged("IsBackedUp");
                OnPropertyChanged("IsOutOfDate");
            }
        }


        public File(string filename)
        {
            Filename = filename;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
