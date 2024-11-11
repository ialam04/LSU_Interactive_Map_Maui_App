using InteractiveLSUMap.ViewModels;
using Microsoft.Maui.Controls;

namespace InteractiveLSUMap
{
    public partial class NewMemoryPage : ContentPage
    {
        public NewMemoryPage(MemoriesViewModel memoriesViewModel)
        {
            InitializeComponent();
            BindingContext = new NewMemoryViewModel(memoriesViewModel);
        }
        
        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}