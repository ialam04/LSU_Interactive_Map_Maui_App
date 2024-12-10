using System.Text.Json;
using InteractiveLSUMap.ViewModels;

namespace InteractiveLSUMap
{
    [QueryProperty(nameof(EventTitle), "eventTitle")]
    [QueryProperty(nameof(EventLocation), "eventLocation")]
    public partial class MainPage : ContentPage
    {
        private string eventTitle;
        private string eventLocation;
        private ProfileViewModel profileVM;
        private MemoriesViewModel memoriesVM;



        public string EventTitle
        {
            get => eventTitle;
            set
            {
                eventTitle = value;
                FocusOnEvent();
            }
        }

        public string EventLocation
        {
            get => eventLocation;
            set
            {
                eventLocation = value;
                FocusOnEvent();
            }
        }

        private async void FocusOnEvent()
        {
            if (!string.IsNullOrEmpty(eventTitle) && !string.IsNullOrEmpty(eventLocation))
            {
                // Ensure map is loaded first
                await Task.Delay(500); // Give map time to initialize

                // First switch to events filter
                await mapView.InvokeJavaScriptFunction("showPins('events')");

                // Then focus on specific event
                await Task.Delay(100); // Small delay to ensure pins are placed
                await mapView.InvokeJavaScriptFunction($"focusOnEvent('{eventTitle}')");

                // Update UI state
                currentFilter = "events";
                dropdownButton.Text = "Events ▼";
            }
        }
        private string currentFilter = "locations";
        private bool isDropdownOpen = false;
        private readonly List<string> filterOptions = new()
        {
        "Locations",
        "Events",
        "Memories",
        "Clubs",
        "Classes"
        };

        public MainPage()
        {
            InitializeComponent();
            filterList.ItemsSource = filterOptions;

            // Use the singleton instance
            profileVM = ProfileViewModel.Instance;
            profileVM.ClassesChanged += OnClassesChanged;

            memoriesVM = MemoriesViewModel.Instance;
            memoriesVM.MemoriesChanged += OnMemoriesChanged;

            // Initialize map and set default view
            InitializeMap();

            // Set initial filter to locations
            currentFilter = "locations";
            dropdownButton.Text = "Locations ▼";
        }

        private async void InitializeMap()
        {
            try
            {
                var html = await FileSystem.OpenAppPackageFileAsync("wwwroot/index.html");
                using var reader = new StreamReader(html);
                var htmlContent = await reader.ReadToEndAsync();

                // Initialize class data
                await UpdateClassPinsData();

                // Initialize map view
                mapView.Source = new HtmlWebViewSource { Html = htmlContent };

                // Show initial pins after small delay
                await Task.Delay(500);
                await mapView.InvokeJavaScriptFunction("showPins('locations')");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load map: {ex.Message}", "OK");
            }
        }

        private async void OnClassesChanged(object sender, EventArgs e)
        {
            // Update class data
            await UpdateClassPinsData();

            // If currently viewing classes, update the display immediately
            if (currentFilter == "classes")
            {
                clearExistingPins();  // Add this line
                await mapView.InvokeJavaScriptFunction("showPins('classes')");
            }

            Console.WriteLine($"Classes updated. Current filter: {currentFilter}"); // Debug log
        }

        // Add this helper method to clear existing pins
        private async void clearExistingPins()
        {
            await mapView.InvokeJavaScriptFunction("clearMarkers()");
        }

        private async void UpdateClassPins()
        {
            await UpdateClassPinsData();
            await mapView.InvokeJavaScriptFunction("showPins('classes')");
        }

        private async Task UpdateClassPinsData()
        {
            try
            {
                var buildingClassesMap = new Dictionary<string, List<string>>();

                Console.WriteLine($"Current classes: {string.Join(", ", profileVM.Classes)}"); // Debug log

                foreach (var className in profileVM.Classes)
                {
                    if (profileVM.ClassLocations.TryGetValue(className, out string building))
                    {
                        Console.WriteLine($"Mapping {className} to {building}"); // Debug log

                        if (!buildingClassesMap.ContainsKey(building))
                        {
                            buildingClassesMap[building] = new List<string>();
                        }
                        buildingClassesMap[building].Add(className);
                    }
                    else
                    {
                        Console.WriteLine($"No location found for {className}"); // Debug log
                    }
                }

                var buildingCoords = new Dictionary<string, double[]>
        {
            {"Lockett Hall", new[] {-91.1818003245514, 30.413306022930918}},
            {"Coates Hall", new[] {-91.17906528057395, 30.413187536318254}},
            {"Nicholson Hall", new[] {-91.17865131560713, 30.41253870863463}}
        };

                var classLocationsData = buildingClassesMap
                    .Where(kvp => buildingCoords.ContainsKey(kvp.Key))
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => new { coordinates = buildingCoords[kvp.Key], classes = kvp.Value }
                    );

                Console.WriteLine($"Final location data: {JsonSerializer.Serialize(classLocationsData)}"); // Debug log

                await mapView.InvokeJavaScriptFunction($"window.userClasses = {JsonSerializer.Serialize(classLocationsData)};");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to update class data: {ex.Message}", "OK");
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

        private async void OnFilterSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is string selectedFilter)
            {
                currentFilter = selectedFilter.ToLower();
                dropdownButton.Text = selectedFilter + " ▼";

                if (currentFilter == "classes")
                {
                    UpdateClassPins(); // Update pins when switching to classes view
                }
                else
                {
                    await mapView.InvokeJavaScriptFunction($"showPins('{currentFilter}')");
                }

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

        private async void OnMemoriesChanged(object sender, EventArgs e)
        {
            try
            {
                await UpdateMemoryPinsData();
                if (currentFilter == "memories")
                {
                    await Task.Delay(100); // Small delay to ensure data is set
                    await mapView.InvokeJavaScriptFunction("showPins('memories')");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in OnMemoriesChanged: {ex.Message}");
            }
        }

        private async Task UpdateMemoryPinsData()
        {
            try
            {
                // Debug logging
                Console.WriteLine($"Updating memories, count: {memoriesVM.Memories.Count}");

                var memoriesData = memoriesVM.Memories
                    .Where(m => m.Coordinates != null)
                    .ToDictionary(
                        m => m.Caption,
                        m => new
                        {
                            coordinates = new[] { m.Coordinates[0], m.Coordinates[1] }, // Ensure correct format
                            date = m.Date,
                            caption = m.Caption
                        }
                    );

                // Debug logging
                Console.WriteLine($"Memories data: {JsonSerializer.Serialize(memoriesData)}");

                // Update JavaScript and force refresh
                await mapView.InvokeJavaScriptFunction($"window.userMemories = {JsonSerializer.Serialize(memoriesData)};");

                // Force refresh of pins if currently showing memories
                if (currentFilter == "memories")
                {
                    await mapView.InvokeJavaScriptFunction("clearMarkers();");
                    await mapView.InvokeJavaScriptFunction("showPins('memories');");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating memory pins: {ex.Message}");
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