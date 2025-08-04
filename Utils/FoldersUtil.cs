using System.Collections.Generic;
using System.IO;
using Serilog;

namespace FolderSync.Utils
{
    internal class FoldersUtil
    {
        public static void SyncEmptyFolders(string sourceDirPath, string replicaDirPath)
        {
            DirectoryInfo sourceDirInfo = new DirectoryInfo(sourceDirPath);
            DirectoryInfo replicaDirInfo = new DirectoryInfo(replicaDirPath);

            List<DirectoryInfo> sourceFolders = new (sourceDirInfo.GetDirectories("*", SearchOption.AllDirectories));
            List<DirectoryInfo> replicaFolders = new (replicaDirInfo.GetDirectories("*", SearchOption.AllDirectories));

            string sourceDir, replicaDir;

            foreach (DirectoryInfo srcDir in sourceFolders)
            {
                replicaDir = srcDir.FullName.Replace(sourceDirPath, replicaDirPath);

                if (!Directory.Exists(replicaDir))
                {
                    Directory.CreateDirectory(replicaDir);
                    Log.Information($"Created directory: {replicaDir}");
                }
            }

            replicaFolders.Reverse(); // Start checking nested subdirectories first.

            foreach (DirectoryInfo repDir in replicaFolders)
            {
                sourceDir = repDir.FullName.Replace(replicaDirPath, sourceDirPath);

                if (!Directory.Exists(sourceDir))
                {
                    Directory.Delete(repDir.FullName, true);
                    Log.Information($"Deleted directory: {repDir.FullName}");
                }
            } 
        }
    }
}
