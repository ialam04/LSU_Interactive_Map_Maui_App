namespace InteractiveLSUMap
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            
            // Add Pinch Gesture Recognizer
            var pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += OnPinchUpdated;
            scrollView.GestureRecognizers.Add(pinchGesture);
        }
        
        private const double MinScale = 1;
        private const double MaxScale = 4;
        private double currentScale = 1;
        private double startScale = 1;

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (e.Status == GestureStatus.Started)
            {
                // Store the current scale factor at the start of the pinch
                startScale = scrollView.Scale;
            }
            else if (e.Status == GestureStatus.Running)
            {
                // Calculate the scale factor
                currentScale = Math.Clamp(startScale * e.Scale, MinScale, MaxScale);
                scrollView.Scale = currentScale;
            }
        }

        private void OnPinClicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            var pinId = button?.CommandParameter as string;
            
            // Show building information for the clicked pin
            switch (pinId)
            {
                case "Nicholson":
                    DisplayAlert("Nicholson Hall", "Details about Nicholson Hall...", "OK");
                    break;
                case "Prescott":
                    DisplayAlert("Prescott Hall", "Details about Prescott Hall...", "OK");
                    break;
                case "Coates":
                    DisplayAlert("Coates Hall", "Details about Coates Hall...", "OK");
                    break;
                case "Library":
                    DisplayAlert("Library", "Details about Middleton Library...", "OK");
                    break;            }
        }
        
    }

}