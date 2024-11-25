using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for BrowseAsteroids.xaml
    /// </summary>
    public partial class BrowseAsteroids : Window
    {
        public const string ApiKey = "IWGgjwLcwHodG0uvcjlLmXXUDmXiZw0k6qVSrQHn";
        List<Asteroid2> asteroidList = new List<Asteroid2>();

        public BrowseAsteroids()
        {
            InitializeComponent();

            
        }

        private async Task<List<Asteroid2>> FetchAsteroids()
        {
            string url = $"https://api.nasa.gov/neo/rest/v1/neo/browse?api_key={ApiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Fetch the JSON response from NASA API
                    string jsonResponse = await client.GetStringAsync(url);

                    // Parse the JSON response using Newtonsoft.Json
                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);
                    var asteroidList = new List<Asteroid2>();

                    // Iterate through each date and its corresponding asteroid data
                    foreach (var asteroid in jsonObject["near_earth_objects"])
                    {
                        // Extract relevant data for each asteroid
                        string name = asteroid["name"].ToString();
                        string hazardous = asteroid["is_potentially_hazardous_asteroid"].ToString();

                        // Extract closest approach date
                        string closestApproachDate = asteroid["close_approach_data"]?
                            .OrderBy(data => DateTime.Parse(data["close_approach_date"].ToString()))
                            .FirstOrDefault()?["close_approach_date"]
                            .ToString() ?? "N/A";

                        // Add to the list of asteroids
                        asteroidList.Add(new Asteroid2
                        {
                            Name = name,
                            IsPotentiallyHazardousAsteroid = hazardous,
                            ClosestApproachDate = closestApproachDate
                        });
                    }

                    return asteroidList;

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching data: {ex.Message}");
                    return null;
                }
            }
        }

        private async void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            List<Asteroid2> asteroids = new List<Asteroid2>();

            asteroids = await FetchAsteroids();

            LoadAsteroids(asteroids);
        }

        private async void ApplyBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Hazardous.IsChecked == true)
            {
                List<Asteroid2> asteroids = new List<Asteroid2>();
                asteroids = await FetchAsteroids();

                List<Asteroid2> hazardousAsteroids = new List<Asteroid2>();

                foreach (var asteroid in asteroids)
                {
                    if (asteroid.IsPotentiallyHazardousAsteroid == "True")
                    {

                        hazardousAsteroids.Add(asteroid);
                    }
                }

                LoadAsteroids(hazardousAsteroids);
            }
            else
            {
                List<Asteroid2> asteroids = new List<Asteroid2>();

                asteroids = await FetchAsteroids();

                LoadAsteroids(asteroids);
            }
        }

        private void LoadAsteroids(List<Asteroid2> asteroids)
        {
            try
            {
                // Clear previous results
                BrowseAsteroidsWp.Children.Clear();

                foreach (var asteroid in asteroids)
                {
                    Grid grid = new Grid
                    {
                        Width = 447,
                        Height = 40,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    Label label1 = new Label
                    {
                        Content = asteroid.Name,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Gilroy ☞"),
                        FontSize = 14, // Padding: 10 units on the left
                    };

                    Label label2 = new Label
                    {
                        Content = asteroid.IsPotentiallyHazardousAsteroid,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Gilroy ☞"),
                        FontSize = 14
                    };

                    Label label3 = new Label
                    {
                        Content = asteroid.ClosestApproachDate,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Gilroy ☞"),
                        FontSize = 14,
                        Padding = new Thickness(0, 0, 50, 0) // Padding: 
                    };

                    grid.Children.Add(label3);
                    grid.Children.Add(label2);
                    grid.Children.Add(label1);
                    BrowseAsteroidsWp.Children.Add(grid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
