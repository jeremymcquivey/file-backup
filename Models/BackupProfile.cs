using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackupMonitor.Models
{
    public class BackupProfile
    {
        private IDirectoryAdapter _originalDirectory;
        private IDirectoryAdapter _backupDirectory;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ProfileId { set; get; }

        public string Name { set; get; }

        public string OriginalPath { set; get; }

        public string BackupPath { set; get; }

        public string OriginalDirectoryType { set; get; }

        public string BackupDirectoryType { set; get; }

        [NotMapped]
        public IDirectoryAdapter OriginalDirectory
        {
            get
            {
                if (null == _originalDirectory)
                {
                    var adapter = Activator.CreateInstance("BackupMonitor", OriginalDirectoryType).Unwrap();
                    _originalDirectory = (IDirectoryAdapter)adapter;
                    _originalDirectory.PathFromRoot = OriginalPath;
                }

                return _originalDirectory;
            }
        }

        [NotMapped]
        public IDirectoryAdapter BackupDirectory
        {
            get
            {
                if (null == _backupDirectory)
                {
                    var adapter = Activator.CreateInstance("BackupMonitor", BackupDirectoryType).Unwrap();
                    _backupDirectory = (IDirectoryAdapter)adapter;
                    _backupDirectory.PathFromRoot = BackupPath;
                }

                return _backupDirectory;
            }
        }
    }
}
