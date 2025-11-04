using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text.Json;

namespace UWPExamProject.Pages
{
    public sealed partial class WebHttp : Page
    {
        private static readonly HttpClient _httpClient = new();

        public WebHttp()
        {
            this.InitializeComponent();
        }

        private async void btnTestWebHttp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var json = await _httpClient.GetStringAsync("https://swapi.dev/api/people");
                var root = JsonSerializer.Deserialize<SwapiList<SWCharacter>>(json, options);
                var items = root?.results ?? new List<SWCharacter>();

                lstWebJsonComments.ItemsSource = items;
                lstStarships.ItemsSource = null;

                await PopulateStarshipsForPeopleAsync(items);
            }
            catch (Exception ex)
            {
                var md = new MessageDialog($"Error retrieving or parsing data: {ex.Message}");
                await md.ShowAsync();
            }
        }

        private async void lstWebJsonComments_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SWCharacter selectedCharacter = (SWCharacter)lstWebJsonComments.SelectedItem;
            if (selectedCharacter != null)
            {
                if (selectedCharacter.starshipRecords != null && selectedCharacter.starshipRecords.Count > 0)
                {
                    lstStarships.ItemsSource = selectedCharacter.starshipRecords;
                }
                else if (selectedCharacter.starships != null && selectedCharacter.starships.Count > 0)
                {
                    try
                    {
                        prgStarships.Visibility = Visibility.Visible;
                        prgStarships.IsActive = true;

                        var ships = await GetStarshipsAsync(selectedCharacter.starships);
                        selectedCharacter.starshipRecords = ships ?? new List<Starship>();
                        lstStarships.ItemsSource = selectedCharacter.starshipRecords;
                    }
                    finally
                    {
                        prgStarships.IsActive = false;
                        prgStarships.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    lstStarships.ItemsSource = new List<Starship>();
                }
            }
            else
            {
            }
        }
        private async Task PopulateStarshipsForPeopleAsync(IEnumerable<SWCharacter> people)
        {
            var peopleList = people?.ToList() ?? new List<SWCharacter>();

            var tasks = peopleList.Select(async person =>
            {
                if (person.starships != null && person.starships.Count > 0)
                {
                    person.starshipRecords = await GetStarshipsAsync(person.starships);
                }
                else
                {
                    person.starshipRecords = new List<Starship>();
                }
            }).ToArray();

            await Task.WhenAll(tasks);
        }
        private async Task<List<Starship>> GetStarshipsAsync(List<string> urls)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = new List<Starship>();

            if (urls == null || urls.Count == 0)
                return result;

            var fetchTasks = urls.Select(url =>
            {
                var fetchUrl = url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    ? "https://" + url.Substring("http://".Length)
                    : url;
                return _httpClient.GetStringAsync(fetchUrl);
            }).ToArray();

            try
            {
                var responses = await Task.WhenAll(fetchTasks);
                foreach (var json in responses)
                {
                    var ship = JsonSerializer.Deserialize<Starship>(json, options);
                    if (ship != null)
                        result.Add(ship);
                }
            }
            catch
            {
            }

            return result;
        }
    }
    public class SwapiList<T>
    {
        public int count { get; set; }
        public string next { get; set; }
        public string previous { get; set; }
        public List<T> results { get; set; }
    }

    public class SWCharacter
    {
        public string name { get; set; }
        public string height { get; set; }
        public string mass { get; set; }
        public string hair_color { get; set; }
        public string skin_color { get; set; }
        public string eye_color { get; set; }
        public string birth_year { get; set; }
        public string gender { get; set; }
        public string homeworld { get; set; }
        public List<string> films { get; set; }
        public List<object> species { get; set; }
        public List<string> vehicles { get; set; }
        public List<string> starships { get; set; }
        public DateTime created { get; set; }
        public DateTime edited { get; set; }
        public string url { get; set; }
        public List<Starship> starshipRecords { get; set; }
    }
    public class Starship
    {
        public string name { get; set; }
        public string model { get; set; }
        public string manufacturer { get; set; }
        public string starship_class { get; set; }
        public string url { get; set; }
    }
}
