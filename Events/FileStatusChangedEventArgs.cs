using BackupMonitor.Models;
using System;

namespace BackupMonitor.Events
{
    internal class FileStatusChangedEventArgs: EventArgs
    {
        public File UpdatedFile { get; set; }
    }
}
