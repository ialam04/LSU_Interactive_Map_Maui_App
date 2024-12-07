using Microsoft.Maui.Controls;
using InteractiveLSUMap.ViewModels;

namespace InteractiveLSUMap
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Use absolute route for navigation
                await Shell.Current.GoToAsync("///map");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation failed: {ex.Message}");
                await DisplayAlert("Error", "Failed to navigate to the Map Page.", "OK");
            }
        }

        private async void OnEditProfilePictureClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Edit Profile Picture", "Feature to change profile picture coming soon!", "OK");
        }

        private async void OnAddClubClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Add Club", "Feature to add clubs coming soon!", "OK");
        }

        private async void OnAddEventInterestClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Add Event Interest", "Feature to add event interests coming soon!", "OK");
        }
    }
}