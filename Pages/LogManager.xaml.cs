using System;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using UWPExamProject.BLogic;

namespace UWPExamProject.Pages
{
    public sealed partial class LogManager : Page
    {
        private const string LogFileName = "Log.txt";

        public LogManager()
        {
            this.InitializeComponent();
        }

        private async void btnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int x = 1;
                int y = 0;
                int result = x / y;
            }
            catch (Exception ex)
            {
                await LogHandler.Write(ex);
                await new Windows.UI.Popups.MessageDialog("Errore generato e scritto nel log.").ShowAsync();
            }
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            lvLog.Items.Clear();
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile logFile = await localFolder.GetFileAsync(LogFileName);
                var lines = await FileIO.ReadLinesAsync(logFile);

                foreach (var line in lines)
                    lvLog.Items.Add(line);
            }
            catch (FileNotFoundException)
            {
                await new Windows.UI.Popups.MessageDialog("Nessun file di log trovato.").ShowAsync();
            }
            catch (Exception ex)
            {
                await LogHandler.Write(ex);
                await new Windows.UI.Popups.MessageDialog("Errore durante la lettura del log.").ShowAsync();
            }
        }

        /// <summary>
        /// NUOVO METODO: Svuota la ListView e sovrascrive il file di log con un file vuoto.
        /// </summary>
        private async void btnResetLog_Click(object sender, RoutedEventArgs e)
        {
            lvLog.Items.Clear();
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                // Creando il file con ReplaceExisting, sovrascriviamo quello vecchio con uno nuovo vuoto.
                await localFolder.CreateFileAsync(LogFileName, CreationCollisionOption.ReplaceExisting);

                await new Windows.UI.Popups.MessageDialog("File di log resettato con successo.").ShowAsync();
            }
            catch (Exception ex)
            {
                await LogHandler.Write(ex);
                await new Windows.UI.Popups.MessageDialog("Errore durante il reset del log.").ShowAsync();
            }
        }
    }
}