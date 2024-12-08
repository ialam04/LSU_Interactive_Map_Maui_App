using System.Text.Json;

namespace InteractiveLSUMap
{
    public partial class MainPage : ContentPage
    {
        private string currentFilter = "locations";
        private bool isDropdownOpen = false;
        private readonly List<string> filterOptions = new()
        {
        "Locations",
        "Events",
        "Memories",
        "Visited",
        "Clubs"
        };

        public MainPage()
        {
            InitializeComponent();
            filterList.ItemsSource = filterOptions;
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

        private void OnDropdownClicked(object sender, EventArgs e)
        {
            isDropdownOpen = !isDropdownOpen;
            dropdownList.IsVisible = isDropdownOpen;
            dropdownButton.Text = currentFilter.First().ToString().ToUpper() +
                                currentFilter[1..] +
                                (isDropdownOpen ? " ▲" : " ▼");
        }

        private void OnFilterSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is string selectedFilter)
            {
                currentFilter = selectedFilter.ToLower();
                dropdownButton.Text = selectedFilter + " ▼";
                mapView.InvokeJavaScriptFunction($"showPins('{currentFilter}')");
                dropdownList.IsVisible = false;
                isDropdownOpen = false;
            }
        }

        // Handle messages from JavaScript
        private async void OnJavaScriptMessage(string message)
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, string>>(message);
            if (data["action"] == "markerClicked")
            {
                var parts = data["data"].Split(':');
                var category = parts[0];
                var name = parts[1];

                string details = category switch
                {
                    "locations" => "Building Information",
                    "events" => "Upcoming Event Details",
                    "memories" => "Your Memory from this location",
                    "visited" => "Last visited on...",
                    "clubs" => "Club meeting times and info",
                    _ => "Details"
                };

                await DisplayAlert(name, details, "OK");
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