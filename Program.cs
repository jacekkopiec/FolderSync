using System;
using System.IO;
using System.Timers;

using CommandLine;
using Serilog;

using FolderSync.Utils;

namespace FolderSync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ParserResult<Options> opt = Parser.Default.ParseArguments<Options>(args);

                string sourceDirPath = opt.Value.Source;
                string replicaDirPath = opt.Value.Replica;
                string logPath = opt.Value.Log;
                double intervalMs = opt.Value.Interval * 60000;
                ConsoleKeyInfo userKey;

                using var log = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console()
                    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
                
                Log.Logger = log;

                if (!Directory.Exists(sourceDirPath) || !Directory.Exists(replicaDirPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Please provide paths to existing source and replica folders.");
                    Console.ResetColor();
                    throw new Exception("Invalid arguments.");
                }

                Console.WriteLine($"----------- FolderSync -----------");
                Log.Information($"Application started {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}");
                Log.Information($"Source folder: {sourceDirPath}");
                Log.Information($"Replica folder: {replicaDirPath}");
                Log.Information($"Synchronization interval = {intervalMs / 60000} min");

                Sync(sourceDirPath, replicaDirPath);

                Timer intervalTimer = new Timer(intervalMs);
                intervalTimer.Elapsed += (sender, e) => Sync(sourceDirPath, replicaDirPath);
                intervalTimer.AutoReset = true;
                intervalTimer.Enabled = true;

                Console.WriteLine("\nApplication running...\nPress ESC key to terminate.\n");

                do
                {
                    userKey = Console.ReadKey();
                } while (userKey.Key != ConsoleKey.Escape);

                intervalTimer.Stop();
                intervalTimer.Dispose();
                Log.Information("Application terminated.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }
        }

        static void Sync(string source, string replica)
        {
            try
            {
                Log.Information($"Synchronization started.");
                FilesHandler.SyncFiles(source, replica);
                FoldersHandler.SyncEmptyFolders(source, replica);
                Log.Information("Synchronization completed.");
            }
            catch (Exception ex)
            {
                Log.Warning(ex.Message);
            }  
        }
    }

    public class Options
    {
        [Option('s', "source", Required = true, HelpText = "Full path to the source directory.")]
        public string Source { get; set; }

        [Option('r', "replica", Required = true, HelpText = "Full path to the replica directory.")]
        public string Replica { get; set; }

        [Option('i', "interval", Required = true, HelpText = "Synchronization interval in minutes.")]
        public double Interval { get; set; }

        [Option('l', "log", Required = true, HelpText = "Full path to the log file.")]
        public string Log { get; set; }
    }
}
