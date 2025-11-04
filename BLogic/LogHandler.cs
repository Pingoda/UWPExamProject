using System;
using System.Threading.Tasks;
using UWPExamProject.BLogic;
using Windows.Storage;

namespace UWPExamProject.BLogic
{
    public static class LogHandler
    {
        private const string LogFileName = "Log.txt";

        public static async Task<bool> Write(Exception exception)
        {
            bool result = false;
            try
            {
                string outLog = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {exception.Message}\n{exception.StackTrace}\n\n";

                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await storageFolder.CreateFileAsync(
                    LogFileName, CreationCollisionOption.OpenIfExists);

                await FileIO.AppendTextAsync(storageFile, outLog);
                result = true;
            }
            catch
            {
            }

            return result;
        }

        public static async Task<bool> WriteAction(string message)
        {
            bool result = false;
            try
            {
                string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ACTION: {message}\n";

                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await storageFolder.CreateFileAsync(
                    LogFileName, CreationCollisionOption.OpenIfExists);

                await FileIO.AppendTextAsync(storageFile, logLine);
                result = true;
            }
            catch
            {
            }

            return result;
        }
    }
}