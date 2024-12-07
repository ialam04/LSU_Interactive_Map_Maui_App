using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace InteractiveLSUMap.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public string FullName { get; set; }
        public string Degree { get; set; }
        public ObservableCollection<string> Clubs { get; }
        public ObservableCollection<string> Interests { get; }
        
        private string _profilePicturePath = "profile_photo.png"; // Default profile picture
        public string ProfilePicturePath
        {
            get => _profilePicturePath;
            set => SetProperty(ref _profilePicturePath, value);
        }

        public ICommand AddClubCommand { get; }
        public ICommand AddInterestCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        
        public ICommand EditProfilePictureCommand { get; }

        public ProfileViewModel()
        {
            // Example initial data
            FullName = "Ibrahim Alam";
            Degree = "Computer Science - SWE, BS";

            Clubs = new ObservableCollection<string> { "WebDev", "SWE", "SASE" };
            Interests = new ObservableCollection<string> { "Sports", "Music" };

            AddClubCommand = new Command(OnAddClub);
            AddInterestCommand = new Command(OnAddInterest);
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
            EditProfilePictureCommand = new Command(OnEditProfilePicture);
        }

        private async void OnAddClub()
        {
            string newClub = await Application.Current.MainPage.DisplayPromptAsync("Add Club", "Enter the name of the new club:");
            if (!string.IsNullOrWhiteSpace(newClub))
            {
                Clubs.Add(newClub);
            }
        }

        private async void OnAddInterest()
        {
            string newInterest = await Application.Current.MainPage.DisplayPromptAsync("Add Interest", "Enter the name of the new interest:");
            if (!string.IsNullOrWhiteSpace(newInterest))
            {
                Interests.Add(newInterest);
            }
        }

        private async void OnSave()
        {
            // Logic to save profile data (e.g., to a database or file)
            await Application.Current.MainPage.DisplayAlert("Success", "Your profile has been updated!", "OK");
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void OnCancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        
        private async void OnEditProfilePicture()
        {
            try
            {
                // Show the action sheet to choose between taking a photo or picking one
                string action = await Application.Current.MainPage.DisplayActionSheet(
                    "Change Profile Picture", 
                    "Cancel", 
                    null, 
                    "Take Photo", 
                    "Choose from Gallery");

                if (action == "Take Photo")
                {
                    // Take a photo using the camera
                    var photo = await MediaPicker.CapturePhotoAsync();
                    if (photo != null)
                    {
                        // Save the photo to a local file and update the profile picture
                        var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                        using var stream = await photo.OpenReadAsync();
                        using var newStream = File.OpenWrite(newFile);
                        await stream.CopyToAsync(newStream);

                        // Set the image path to the new file
                        ProfilePicturePath = newFile;
                        OnPropertyChanged(nameof(ProfilePicturePath));
                    }
                }
                else if (action == "Choose from Gallery")
                {
                    // Pick a photo from the gallery
                    var photo = await MediaPicker.PickPhotoAsync();
                    if (photo != null)
                    {
                        // Save the photo to a local file and update the profile picture
                        var newFile = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                        using var stream = await photo.OpenReadAsync();
                        using var newStream = File.OpenWrite(newFile);
                        await stream.CopyToAsync(newStream);

                        // Set the image path to the new file
                        ProfilePicturePath = newFile;
                        OnPropertyChanged(nameof(ProfilePicturePath));
                    }
                }
            }
            catch (PermissionException)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Permission Denied", 
                    "Camera or gallery permissions are required to change your profile picture.", 
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", 
                    $"An error occurred while changing your profile picture: {ex.Message}", 
                    "OK");
            }
        }

    }
}
