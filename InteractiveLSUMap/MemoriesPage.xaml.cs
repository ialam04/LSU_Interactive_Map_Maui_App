using InteractiveLSUMap.ViewModels;
using Microsoft.Maui.Controls;

namespace InteractiveLSUMap
{
    public partial class MemoriesPage : ContentPage
    {
        private readonly MemoriesViewModel _memoriesViewModel;

        public MemoriesPage()
        {
            InitializeComponent();
            _memoriesViewModel = MemoriesViewModel.Instance; // Use singleton instance
            BindingContext = _memoriesViewModel;
            NavigationPage.SetHasNavigationBar(this, false);
        }

    }
}