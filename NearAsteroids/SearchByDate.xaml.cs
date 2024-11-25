using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NearAsteroids
{


    
    public partial class SearchByDate : Window
    {
        public const string ApiKey = "IWGgjwLcwHodG0uvcjlLmXXUDmXiZw0k6qVSrQHn"; // Replace with your actual NASA API key
        List<Asteroid1> asteroidList = new List<Asteroid1>();
        List<Asteroid1> closestAsteroids = new List<Asteroid1>();

        public SearchByDate()
        {
            InitializeComponent();
        }

        private async void ConfirmBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ConfirmBtn.IsEnabled = false;

            if (StartDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a start date.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime startDate = StartDate.SelectedDate.Value;

            // Calculate the end date
            DateTime endDate = startDate.AddDays(6);

            // Format the dates to the required format (e.g., YYYY-MM-DD)
            string formattedStartDate = startDate.ToString("yyyy-MM-dd");
            string formattedEndDate = endDate.ToString("yyyy-MM-dd");

            List<Asteroid1> asteroids = new List<Asteroid1>();

            // Fetch data from NASA API
            try
            {
                asteroids = await FetchAsteroids(formattedStartDate, formattedEndDate);

                // Clear previous results
                searchByDate.Children.Clear();

                

                


                // Display results
                foreach (var asteroid in asteroids)
                {
                    Grid grid = new Grid
                    {
                        Width = 332,
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
                        FontSize = 14,
                        Padding = new Thickness(50, 0, 0, 0) // Padding: 10 units on the left
                    };

                    Label label2 = new Label
                    {
                        Content = asteroid.ApproachDate,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Gilroy ☞"),
                        FontSize = 14,
                        Padding = new Thickness(0, 0, 50, 0) // Padding: 
                    };

                    grid.Children.Add(label2);
                    grid.Children.Add(label1);
                    searchByDate.Children.Add(grid);

                }

                MessageBox.Show("Asteroids have been successfully displayed.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while fetching data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ConfirmBtn.IsEnabled = true;
            }
        }

        private async Task<List<Asteroid1>> FetchAsteroids(string startDate, string endDate)
        {
            string url = $"https://api.nasa.gov/neo/rest/v1/feed?start_date={startDate}&end_date={endDate}&api_key={ApiKey}";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Fetch the JSON response from NASA API
                    string jsonResponse = await client.GetStringAsync(url);

                    // Parse the JSON response using Newtonsoft.Json
                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(jsonResponse);

                    // Iterate through each date and its corresponding asteroid data
                    foreach (var dateProperty in jsonObject["near_earth_objects"])
                    {
                        foreach (var asteroid in dateProperty.First())
                        {
                            // Extract relevant data for each asteroid
                            string name = asteroid["name"]?.ToString();
                            string approachDate = dateProperty.Path.Split('.').Last();

                            // Extract miss distance in kilometers
                            var missDistanceStr = asteroid["close_approach_data"]?[0]?["miss_distance"]?["kilometers"]?.ToString();
                            decimal missDistance = !string.IsNullOrEmpty(missDistanceStr) ? decimal.Parse(missDistanceStr) : decimal.MaxValue;

                            // Add to the list of asteroids
                            asteroidList.Add(new Asteroid1
                            {
                                Name = name,
                                ApproachDate = approachDate,
                                MissDistance = missDistance
                            });
                        }
                    }

                    // Get the 5 closest asteroids sorted by miss distance
                    closestAsteroids = asteroidList
                        .OrderBy(a => a.MissDistance)
                        .Take(5)
                        .ToList();

                    return closestAsteroids;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while fetching data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return null;
                }
            }
        }
    }
}
