using InteractiveLSUMap.ViewModels;
using Microsoft.Maui.Controls;

namespace InteractiveLSUMap
{
    public partial class MemoriesPage : ContentPage
    {
        public MemoriesPage()
        {
            InitializeComponent();
            BindingContext = new MemoriesViewModel();
            NavigationPage.SetHasNavigationBar(this, false);
        }

    }
}