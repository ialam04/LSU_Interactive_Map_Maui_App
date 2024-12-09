using System.Collections.ObjectModel;
using System.Windows.Input;
using InteractiveLSUMap.Models;
using InteractiveLSUMap.ViewModels;
using Microsoft.Maui.Controls;

namespace InteractiveLSUMap.ViewModels
{
    public class NewMemoryViewModel : BaseViewModel
    {
        private readonly MemoriesViewModel _memoriesViewModel;

        public ObservableCollection<string> Locations { get; }
        public string SelectedLocation { get; set; }
        public string Caption { get; set; }
        public string ImagePath { get; set; }

        public ICommand CompleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand AddPhotoCommand { get; }

        // Default constructor for XAML
        public NewMemoryViewModel()
        {
            Locations = new ObservableCollection<string>
            {
                "Prescott Hall",
                "Coates Hall",
                "LSU Library",
                "Howe-Russell Geoscience Complex",
                "Student Union",
                "Lockett Hall",
                "Art & Design Building",
                "Free Speech Plaza",
                "Parade Ground"
            };


            CompleteCommand = new Command(() => { });
            CancelCommand = new Command(() => { });
            AddPhotoCommand = new Command(() => { });
        }

        // Constructor with MemoriesViewModel parameter
        public NewMemoryViewModel(MemoriesViewModel memoriesViewModel) : this()
        {
            _memoriesViewModel = memoriesViewModel;
            CompleteCommand = new Command(OnComplete);
            CancelCommand = new Command(OnCancel);
            AddPhotoCommand = new Command(OnAddPhoto);
        }

        private async void OnComplete()
        {
            if (_memoriesViewModel == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Unable to save memory due to missing view model.", "OK");
                return;
            }

            var newMemory = new Memory
            {
                Location = SelectedLocation,
                Caption = Caption,
                ImagePath = ImagePath,
                Date = DateTime.Now.ToString("MM-dd-yyyy")
            };

            _memoriesViewModel.AddMemory(newMemory);
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void OnCancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async void OnAddPhoto()
        {
            try
            {
                string action = await Application.Current.MainPage.DisplayActionSheet("Add Photo", "Cancel", null, "Take Photo", "Choose from Gallery");

                FileResult photo = null;
                if (action == "Take Photo")
                {
                    photo = await MediaPicker.CapturePhotoAsync();
                }
                else if (action == "Choose from Gallery")
                {
                    photo = await MediaPicker.PickPhotoAsync();
                }

                if (photo != null)
                {
                    ImagePath = photo.FullPath;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
            catch (PermissionException)
            {
                await Application.Current.MainPage.DisplayAlert("Permission Denied", "Camera and photo library permissions are required to add a photo.", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Unable to add photo: {ex.Message}", "OK");
            }
        }
    }
}
