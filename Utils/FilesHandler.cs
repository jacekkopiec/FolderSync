using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

using Serilog;

namespace FolderSync.Utils
{
    internal class FilesHandler
    {
        public static void SyncFiles(string sourceDirPath, string replicaDirPath)
        {
            DirectoryInfo sourceDirInfo = new DirectoryInfo(sourceDirPath);
            DirectoryInfo replicaDirInfo = new DirectoryInfo(replicaDirPath);

            List<FileInfo> sourceFiles = new (sourceDirInfo.GetFiles("*", SearchOption.AllDirectories));
            List<FileInfo> replicaFiles = new (replicaDirInfo.GetFiles("*", SearchOption.AllDirectories));

            string sourceDir, sourceFile, sourceFileMD5, replicaDir, replicaFile, replicaFileMD5 = "";

            foreach (FileInfo srcFile in sourceFiles)
            {
                replicaDir = srcFile.DirectoryName.Replace(sourceDirPath, replicaDirPath);
                replicaFile = srcFile.FullName.Replace(sourceDirPath, replicaDirPath);

                if (!File.Exists(replicaFile))
                {
                    Directory.CreateDirectory(replicaDir);
                    File.Copy(srcFile.FullName, replicaFile);
                    Log.Information($"File copied: {replicaFile}");
                }
                else if (srcFile.Length != replicaFiles.Find(x => x.FullName == replicaFile).Length)
                {
                    File.Copy(srcFile.FullName, replicaFile, true);
                    Log.Information($"File updated: {replicaFile}");
                }
                else
                {
                    using (MD5 md5 = MD5.Create())
                    {
                        using (FileStream sourceFileStream = File.OpenRead(srcFile.FullName))
                        {
                            sourceFileMD5 = BitConverter.ToString(md5.ComputeHash(sourceFileStream));
                        }

                        using (FileStream replicaFileStream = File.OpenRead(replicaFile))
                        {
                            replicaFileMD5 = BitConverter.ToString(md5.ComputeHash(replicaFileStream));
                        }
                    }

                    if (sourceFileMD5 != replicaFileMD5)
                    {
                        File.Copy(srcFile.FullName, replicaFile, true);
                        Log.Information($"File updated: {replicaFile}");
                    }
                }
            }

            foreach (FileInfo repFile in replicaFiles)
            {
                sourceDir = repFile.DirectoryName.Replace(replicaDirPath, sourceDirPath);
                sourceFile = repFile.FullName.Replace(replicaDirPath, sourceDirPath);

                if (!File.Exists(sourceFile))
                {
                    File.Delete(repFile.FullName);
                    Log.Information($"File removed: {repFile.FullName}");
                }
            }
        }
    }
}
