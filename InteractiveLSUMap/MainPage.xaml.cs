using System.Text.Json;

namespace InteractiveLSUMap
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            InitializeMap();
        }

        private async void InitializeMap()
        {
            try
            {
                var html = await FileSystem.OpenAppPackageFileAsync("wwwroot/index.html");
                using var reader = new StreamReader(html);
                mapView.Source = new HtmlWebViewSource
                {
                    Html = await reader.ReadToEndAsync()
                };
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load map: {ex.Message}", "OK");
            }
        }

        // Handle messages from JavaScript
        private async void OnJavaScriptMessage(string message)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
            if (data["action"] == "markerClicked")
            {
                await DisplayAlert(data["data"], "Building details here...", "OK");
            }
        }

        private async void OnProfileButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Use absolute route for navigation
                await Shell.Current.GoToAsync("///profile");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.Message}");
                await DisplayAlert("Error", "Failed to navigate to the Profile Page.", "OK");
            }
        }
    }

}