using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NearAsteroids
{
    /// <summary>
    /// Interaction logic for SearchByNASAID.xaml
    /// </summary>
    public partial class SearchByNASAID : Window
    {

        private const string ApiKey = "IWGgjwLcwHodG0uvcjlLmXXUDmXiZw0k6qVSrQHn";
        public SearchByNASAID()
        {
            InitializeComponent();
        }

        private async void ConfirmBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NasaSearchBar.Text))
            {
                MessageBox.Show("Please enter a NASA ID to search.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ConfirmBtn.IsEnabled = false; // Disable button during the request
            string nasaId = NasaSearchBar.Text.Trim();
            string url = $"https://api.nasa.gov/neo/rest/v1/neo/{nasaId}?api_key={ApiKey}";

            try
            {
                // Fetch the asteroid data
                JObject asteroidData = await FetchAsteroidDataAsync(url);

                if (asteroidData != null)
                {
                    NasaID.Text = $"NASA ID: {asteroidData["id"].ToString()}";
                    AsteroidName.Text = $"Name: {asteroidData["name"].ToString()}";
                    Designation.Text = $"Designation: {asteroidData["designation"].ToString()}";
                    Magnitude.Text = $"Absolute Magnitude: {asteroidData["absolute_magnitude_h"].ToString()}";

                    JObject estimatedDiameter = asteroidData["estimated_diameter"]?.ToObject<JObject>();
                    if (estimatedDiameter != null)
                    {
                        // Extract the minimum and maximum diameter in kilometers
                        double diameterMin = estimatedDiameter["kilometers"]?["estimated_diameter_min"]?.ToObject<double>() ?? 0;
                        double diameterMax = estimatedDiameter["kilometers"]?["estimated_diameter_max"]?.ToObject<double>() ?? 0;

                        double averageDiameter = (diameterMin + diameterMax) / 2;

                        // Format and display the diameter information
                        Diameter.Text = $"Estimated Diameter: {averageDiameter} km";
                    }

                    Hazardous.Text = $"Hazardous?: {asteroidData["is_potentially_hazardous_asteroid"].ToString()}";
                }
                else
                {
                    MessageBox.Show("No data found for the provided NASA ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    NasaSearchBar.Text = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConfirmBtn.IsEnabled = true; // Re-enable button
            }
        }

        private async Task<JObject> FetchAsteroidDataAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<JObject>(jsonResponse);
                }
                else
                {
                    throw new Exception($"Failed to fetch data. Status code: {response.StatusCode}");
                }
            }
        }
    }
}
