namespace InteractiveLSUMap;

public partial class LandingPage : ContentPage
{
	public LandingPage()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await PageContainer.FadeTo(1, 1000);

	}

	private async void OnEnterSignIn(object sender, EventArgs e)
	{
		var fadeOut = SignInContainer.FadeTo(0, 1000);
		var moveLogo = logo.TranslateTo(0, logo.Y + 200, 1000);

		await Task.WhenAll(fadeOut, moveLogo);

		await claw1.FadeTo(1, 500);
        await claw2.FadeTo(1, 500);
        await claw3.FadeTo(1, 500);
        await claw4.FadeTo(1, 500);

		await Task.Delay(500);

        await Shell.Current.GoToAsync("//map");
    }
}