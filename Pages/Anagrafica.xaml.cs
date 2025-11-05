using UWPExamProject.BLogic;
using UWPExamProject.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace UWPExamProject.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Anagrafica : Page
    {
        List<AnagraficaModel> listAnag = new List<AnagraficaModel>();
        public Anagrafica()
        {
            this.InitializeComponent();
        }
        private void Helper_ResetErrors()
        {
            StatusOperation.Text = "";
            ErrorEnrollment.Text = "";
            ErrorEnrollment.Visibility = Visibility.Collapsed;
            ErrorFullName.Text = "";
            ErrorFullName.Visibility = Visibility.Collapsed;
            ErrorEmail.Text = "";
            ErrorEmail.Visibility = Visibility.Collapsed;
            ErrorPhone.Text = "";
            ErrorPhone.Visibility = Visibility.Collapsed;
        }

        private async void btnInsertRegistry_Click(object sender, RoutedEventArgs e)
        {
            Helper_ResetErrors();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtEnrollment.Text))
            {
                await LogHandler.WriteAction("La matricola non è stata inserita");
                ErrorEnrollment.Text = "La matricola è obbligatoria.";
                ErrorEnrollment.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                await LogHandler.WriteAction("Il nominativo è obbligatorio.");
                ErrorFullName.Text = "Il nominativo è obbligatorio.";
                ErrorFullName.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (txtFullName.Text.Any(char.IsDigit))
            {
                await LogHandler.WriteAction("Il nominativo non può contenere numeri.");
                ErrorFullName.Text = "Il nominativo non può contenere numeri.";
                ErrorFullName.Visibility = Visibility.Visible;
                isValid = false;
            }

            string emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                await LogHandler.WriteAction("L'email è obbligatoria.");
                ErrorEmail.Text = "L'email è obbligatoria.";
                ErrorEmail.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!Regex.IsMatch(txtEmail.Text, emailRegex))
            {
                await LogHandler.WriteAction("Formato email non valido (es. esempio@email.com).");
                ErrorEmail.Text = "Formato email non valido (es. esempio@email.com).";
                ErrorEmail.Visibility = Visibility.Visible;
                isValid = false;
            }

            string phoneToValidate = txtPhone.Text.Replace(" ", "").Replace("+", "");

            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                await LogHandler.WriteAction("Il telefono è obbligatorio.");
                ErrorPhone.Text = "Il telefono è obbligatorio.";
                ErrorPhone.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (!phoneToValidate.All(char.IsDigit))
            {
                await LogHandler.WriteAction("Il telefono deve contenere solo numeri.");
                ErrorPhone.Text = "Il telefono deve contenere solo numeri.";
                ErrorPhone.Visibility = Visibility.Visible;
                isValid = false;
            }
            else if (phoneToValidate.Length < 9 || phoneToValidate.Length > 15)
            {
                await LogHandler.WriteAction("Il telefono deve avere da 9 a 15 cifre.");
                ErrorPhone.Text = "Il telefono deve avere da 9 a 15 cifre.";
                ErrorPhone.Visibility = Visibility.Visible;
                isValid = false;
            }

            if (!isValid)
            {
                StatusOperation.Text = "Errore: Controlla i campi evidenziati.";
                return;
            }

            var anagrafica = new AnagraficaModel();
            anagrafica.Enrollment = txtEnrollment.Text;
            anagrafica.FullName = txtFullName.Text;
            anagrafica.Email = txtEmail.Text;
            anagrafica.Phone = txtPhone.Text;
            listAnag.Add(anagrafica);

            StatusOperation.Text = $"Inserito {anagrafica.FullName}";

            txtEnrollment.Text = "";
            txtFullName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
        }

        private async void btnSaveJSON_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile storageFile = await storageFolder.CreateFileAsync("Anagrafica.json", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(storageFile, JsonConvert.SerializeObject(listAnag));

                StatusOperation.Text = $"Salvati {listAnag.Count} record in Anagrafica.json";
            }
            catch (Exception ex)
            {
                await LogHandler.Write(ex);
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
                    listAnag = new List<AnagraficaModel>();
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
                StatusOperation.Text = "ERRORE: Il file JSON è corrotto o non è nel formato corretto.";
            }
            catch (Exception ex)
            {
                StatusOperation.Text = "ERRORE: " + ex.Message;
            }
        }
    }
}