using InteractiveLSUMap.ViewModels;
namespace InteractiveLSUMap;

public partial class EventsPage : ContentPage
{
	public EventsPage()
	{
		InitializeComponent();
		BindingContext = new EventsPageViewModel();
	}
}