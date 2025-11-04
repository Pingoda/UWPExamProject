using System;
using UWPExamProject.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPExamProject
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnAnag_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Mainframe.Navigate(typeof(Anagrafica));
        }

        private void btnWebHttp_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Mainframe.Navigate(typeof(WebHttp));
        }

        private void btnLogManager_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Mainframe.Navigate(typeof(LogManager));
        }

        private async void btnExit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ContentDialog contentDialog = new ContentDialog
            {
                Title = "Chiudi Appplicazione",
                Content = "Sicuro di volere chiudere e uscire dall'App?",
                PrimaryButtonText = "Si",
                CloseButtonText = "Annulla",
            };

            var result = await contentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Application.Current.Exit();
            }
        }
    }
}
