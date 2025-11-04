using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UWPExamProject.Model;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPExamProject.Pages
{
    public sealed partial class Anagrafica : Page
    {
        List<AnagraficaModel> listAnag = new List<AnagraficaModel>();
        public Anagrafica()
        {
            this.InitializeComponent();
        }

        private void btnInsertRegistry_Click(object sender, RoutedEventArgs e)
        {
            var anagrafica = new AnagraficaModel();
            anagrafica.Enrollment = txtEnrollment.Text;
            anagrafica.FullName = txtFullName.Text;
            anagrafica.Email = txtEmail.Text;
            anagrafica.Phone = txtPhone.Text;
            listAnag.Add(anagrafica);
            
            StatusOperation.Text = $"Inserito {anagrafica.FullName}";
        }

        private async void btnSaveJSON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await storageFolder.CreateFileAsync("Anagrafica.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(storageFile, JsonConvert.SerializeObject(listAnag));
            }
            catch(Exception ex)
            {
                StatusOperation.Text = "ERRORE: " + ex.Message;
            }
        }

        private async void btnReadJSON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await storageFolder.GetFileAsync("Anagrafica.json");
                string jsonText = await FileIO.ReadTextAsync(storageFile);

                listAnag = JsonConvert.DeserializeObject<List<AnagraficaModel>>(jsonText);

                if (listAnag == null || listAnag.Count == 0)
                {
                    StatusOperation.Text = "File JSON letto correttamente, ma non contiene dati.";
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Letti {listAnag.Count} record dal file:");
                sb.AppendLine("-----------------------------------");

                foreach (AnagraficaModel anagrafica in listAnag)
                {
                    sb.AppendLine($"Matricola: {anagrafica.Enrollment}");
                    sb.AppendLine($"Nominativo: {anagrafica.FullName}");
                    sb.AppendLine($"Email: {anagrafica.Email}");
                    sb.AppendLine($"Telefono: {anagrafica.Phone}");
                    sb.AppendLine();
                }

                StatusOperation.Text = sb.ToString();
            }
            catch (FileNotFoundException)
            {
                StatusOperation.Text = "ERRORE: Il file 'Anagrafica.json' non è stato trovato.";
            }
            catch (JsonException)
            {
                // Questo errore scatta se il JSON è malformato
                StatusOperation.Text = "ERRORE: Il file JSON è corrotto o non è nel formato corretto.";
            }
            catch (Exception ex)
            {
                // Un gestore generico per qualsiasi altro errore (es. permessi)
                StatusOperation.Text = "ERRORE: " + ex.Message;
            }
        }
    }
}
