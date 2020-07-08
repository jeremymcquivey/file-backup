using System;
using System.Collections.Generic;

namespace BackupMonitor.Models
{
    public interface IDirectoryAdapter
    {
        string PathFromRoot { set; get; }

        string Username { set; get; }

        string Password { set; get; }

        string Type { get; }

        Guid LocationId { get; set; }

        IEnumerable<File> GetFiles(Directory directory);

        IEnumerable<Directory> GetDirectories(Directory directory);

        bool FileExists(string fullRelativePathName);

        File GetFile(string fullRelativePathName);

        bool PlaceFile(string fullRelativePathName, System.IO.FileStream file, bool overwrite);

        BackupStatus Compare(File file);
    }
}
