using InteractiveLSUMap.ViewModels;
namespace InteractiveLSUMap;

public partial class EventsPage : ContentPage
{
	public EventsPage()
	{
		InitializeComponent();
		BindingContext = new EventsPageViewModel(ProfileViewModel.Instance);
	}
	
	protected override void OnAppearing()
	{
		base.OnAppearing();
		BindingContext = new EventsPageViewModel(ProfileViewModel.Instance);
	}

}