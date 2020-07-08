using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BackupMonitor.Models
{
    public class LocalFileStorageAdapter : IDirectoryAdapter
    {
        public Guid LocationId { set; get; }

        public string Password { set; get; }

        public string PathFromRoot { set; get; }

        public string Username { set; get; }

        public string Type => GetType().Name;

        public LocalFileStorageAdapter() : this(string.Empty) { }

        public LocalFileStorageAdapter(string pathFromRoot)
        {
            PathFromRoot = pathFromRoot;
        }

        public BackupStatus Compare(File file)
        {
            var fullPath = System.IO.Path.Combine(PathFromRoot, file.RelativePath, file.Filename);
            if (System.IO.File.Exists(fullPath))
            {
                var fileInfo = new System.IO.FileInfo(fullPath);
                return (fileInfo.LastWriteTimeUtc == file.Modified) ? BackupStatus.BackedUp : BackupStatus.OutOfDate;
            }

            return BackupStatus.NotBackedUp;
        }

        public bool FileExists(string fullRelativePathName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<File> GetFiles(Directory directory)
        {
            var result = new List<File>();

            if (System.IO.Directory.Exists(directory.FullPath))
            {
                var files = System.IO.Directory.GetFiles(directory.FullPath).ToList();
                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new System.IO.FileInfo(file);

                        var fileObj = new File(fileInfo.Name)
                        {
                            RelativePath = directory.RelativePath,
                            AbsolutePath = directory.FullPath,
                            Modified = fileInfo.LastWriteTimeUtc,
                        };

                        result.Add(fileObj);
                    }
                    catch (System.IO.PathTooLongException)
                    {
                        Debug.WriteLine($"Filename too long: {file}");
                    }
                }
            }

            return result;
        }

        public IEnumerable<Directory> GetDirectories(Directory directory)
        {
            var result = new List<Directory>();

            if (System.IO.Directory.Exists(directory.FullPath))
            {
                var directories = System.IO.Directory.GetDirectories(directory.FullPath).ToList();

                foreach (var dir in directories)
                {
                    try
                    {
                        var dirObj = new Directory(dir);
                        dirObj.RelativePath = System.IO.Path.Combine(directory.RelativePath, dirObj.DirectoryName);
                        directory.Directories.Add(dirObj);

                        result.Add(dirObj);
                    }
                    catch (System.IO.PathTooLongException)
                    {
                        Debug.WriteLine($"Path too long: {dir}");
                    }
                }
            }

            return result;
        }

        public File GetFile(string fullRelativePathName)
        {
            throw new NotImplementedException();
        }

        public bool PlaceFile(string fullRelativePathName, System.IO.FileStream file, bool overwrite)
        {
            throw new NotImplementedException();
        }
    }
}
