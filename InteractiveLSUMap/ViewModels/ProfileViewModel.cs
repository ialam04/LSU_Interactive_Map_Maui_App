using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace InteractiveLSUMap.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private static ProfileViewModel _instance;
        public static ProfileViewModel Instance => _instance ??= new ProfileViewModel();

        public string FullName { get; set; }
        public string Degree { get; set; }
        public event EventHandler ClassesChanged;
        public ObservableCollection<string> Clubs { get; }
        public ObservableCollection<string> Classes { get; }
        public ObservableCollection<string> AvailableClubs { get; }
        public ObservableCollection<string> AvailableClasses { get; }
        public Dictionary<string, string> ClassLocations { get; }
        private string _profilePicturePath = "profile_photo.png"; // Default profile picture
        public string ProfilePicturePath
        {
            get => _profilePicturePath;
            set => SetProperty(ref _profilePicturePath, value);
        }

        private string _selectedClub;
        public string SelectedClub
        {
            get => _selectedClub;
            set
            {
                if (SetProperty(ref _selectedClub, value))
                    OnPropertyChanged(nameof(IsAddClubEnabled)); // Notify UI
            }
        }

        private string _selectedClass;
        public string SelectedClass
        {
            get => _selectedClass;
            set
            {
                if (SetProperty(ref _selectedClass, value))
                    OnPropertyChanged(nameof(IsAddClassEnabled)); // Notify UI
            }
        }

        public bool IsAddClubEnabled => !string.IsNullOrWhiteSpace(SelectedClub);
        public bool IsAddClassEnabled => !string.IsNullOrWhiteSpace(SelectedClass);

        public ICommand ConfirmAddClubCommand { get; }
        public ICommand ConfirmAddClassCommand { get; }
        public ICommand EditProfilePictureCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ProfileViewModel()
        {

            FullName = "Ibrahim Alam";
            Degree = "Mathematics - Data Science, BS";

            Clubs = new ObservableCollection<string>
            {
                "DADA",
                "CASA"
            };
            Classes = new ObservableCollection<string>
            {
                "MATH 4066", "MATH 4153", "MATH 2090", "ENGL 2000"
            };

            AvailableClubs = new ObservableCollection<string>
            {
                "DADA",
                "CASA",
                "Kayaking Club",
                "Chess Club"
            };

            AvailableClasses = new ObservableCollection<string>
            {
                "MATH 4058", "MATH 4066", "MATH 4153", "MATH 2060", "MATH 2065",
                "MATH 2090", "MATH 1550", "MATH 2070", "ENGL 2000"
            };

            ClassLocations = new Dictionary<string, string>
            {
                { "MATH 4058", "Lockett Hall" },
                { "MATH 4066", "Lockett Hall" },
                { "MATH 4153", "Lockett Hall" },
                { "MATH 2060", "Coates Hall" },
                { "MATH 2065", "Coates Hall" },
                { "MATH 2090", "Coates Hall" },
                { "MATH 1550", "Nicholson Hall" },
                { "MATH 2070", "Nicholson Hall" },
                { "ENGL 2000", "Coates Hall" }
            };

            ConfirmAddClubCommand = new Command(OnConfirmAddClub);
            ConfirmAddClassCommand = new Command(OnConfirmAddClass);
            EditProfilePictureCommand = new Command(OnEditProfilePicture);
            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
        }

        public void GetClassLocation(string className)
        {
            var classData = new ProfileViewModel();

            if (classData.ClassLocations.TryGetValue(className, out string location))
            {
                Console.WriteLine($"{className} is located in {location}");
            }
            else
            {
                Console.WriteLine($"Location for {className} not found");
            }
        }

        private void OnConfirmAddClub()
        {
            if (!string.IsNullOrWhiteSpace(SelectedClub) && !Clubs.Contains(SelectedClub))
            {
                Clubs.Add(SelectedClub);
                if (this != Instance)
                {
                    Instance.Clubs.Add(SelectedClub);
                }
                SelectedClub = null; // Reset selection
            }
        }

        private void OnConfirmAddClass()
        {
            if (!string.IsNullOrWhiteSpace(SelectedClass) && !Classes.Contains(SelectedClass))
            {
                // Verify class has a valid location before adding
                if (ClassLocations.TryGetValue(SelectedClass, out string building))
                {
                    // Update both local and instance collections
                    Classes.Add(SelectedClass);
                    if (this != Instance)
                    {
                        Instance.Classes.Add(SelectedClass);
                    }

                    Console.WriteLine($"Added class {SelectedClass} in {building}"); // Debug log

                    SelectedClass = null;
                    ClassesChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    // Handle invalid class location
                    Application.Current.MainPage.DisplayAlert(
                        "Error",
                        $"Location not found for {SelectedClass}",
                        "OK");
                }
            }
        }

        public void RemoveClass(string className)
        {
            if (Classes.Contains(className))
            {
                Classes.Remove(className);
                ClassesChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private async void OnSave()
        {
            await Application.Current.MainPage.DisplayAlert("Success", "Profile updated!", "OK");
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
