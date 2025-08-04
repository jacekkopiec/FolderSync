# :open_file_folder: FolderSync

:white_check_mark: A program written in C# that synchronizes two folders: source and replica.

:white_check_mark: Synchronization is performed periodically with given time interval (in minutes).

:white_check_mark: File operations and additional info/error/warning messages are logged to a file (rolling daily) and in the console output.

:white_check_mark: Folder paths, synchronization interval and log file path are provided using the command line arguments.

:package: External libraries:
- [CommandLineParser](https://github.com/commandlineparser/commandline) - handling commandline arguments with validation, help screen etc.
- [Serilog](https://github.com/serilog/serilog) - easy console and file logging.

## :information_source: How to use

| Argument              | Required | Description                             |
|-----------------------|----------|-----------------------------------------|
| **-s, --source**      | Yes      | Full path to the source directory.      |
| **-r, --replica**     | Yes      | Full path to the replica directory.     |
| **-i, --interval**    | Yes      | Synchronization interval in minutes.    |
| **-l, --log**         | Yes      | Full path to the log file.              |
| --help                | -        | Display help screen.                    |
| --version             | -        | Display version information.            |

Example:

FolderSync.exe **--source** C:\Source **--replica** D:\Source_backup **--interval** 15 **--log** D:\Logs\log.txt

## :warning: Known issues & Limitations
- The application is synchronous, so it may be slow for folders with many large files.
- Some apps constantly access a file while it is open (for example: monitoring changes and saving a backup file by MS Office apps). This will generate an exception handled only by warning message in log. This does not affect synchronization of saved file changes.
- Source and replica folder paths must exist. The program does not create these folders.
- Renaming a file or folder triggers copy/create operation of a new (renamed) file/folder and deleting the old one.
- Unexpected behaviour may occur in case of short synchronization interval and large (in terms of the number and/or size of files) source directory. This case is not yet handled.

## :bulb: Enhancement ideas
- Asynchronous operation to process multiple file operation tasks simultaneously.
- Consider using FileSystemWatcher to prepare synchronization tasks between sync intervals. (Windows only?)
- Handle synchronization conflicts in case of short sync interval and large source folder.
- If source and/or replica folder path does not exist, ask the user to create them.
- Refactor 🙂
