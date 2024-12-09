using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace InteractiveLSUMap.ViewModels
{
    public class EventsPageViewModel : BaseViewModel
    {
        public string ProfilePicturePath { get; set; } = "profile_photo.png";
        public string Greeting { get; set; } = "Hey Ibrahim!";

        public ObservableCollection<Event> EventsToday { get; set; }
        public ObservableCollection<Event> FilteredEventsToday { get; private set; }
        public ObservableCollection<Event> ClubEvents { get; set; }
        public ObservableCollection<Event> EnrolledClubEvents { get; private set; }
        public ObservableCollection<string> StudentClubs { get; set; }
        public ObservableCollection<Event> FilteredClubEvents { get; private set; }

        private Event _selectedEvent;
        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                OnPropertyChanged(nameof(SelectedEvent));
            }
        }

        private Event _selectedClubEvent;
        public Event SelectedClubEvent
        {
            get => _selectedClubEvent;
            set
            {
                _selectedClubEvent = value;
                OnPropertyChanged(nameof(SelectedClubEvent));
            }
        }

        public ICommand ViewMoreCommand { get; }
        public ICommand ViewMoreClubCommand { get; }
        public ICommand EventClickedCommand { get; }
        public ICommand CloseEventDetailsCommand { get; }
        public ICommand OpenMapCommand { get; }

        private bool _isEventDetailsVisible;
        public bool IsEventDetailsVisible
        {
            get => _isEventDetailsVisible;
            set
            {
                _isEventDetailsVisible = value;
                OnPropertyChanged(nameof(IsEventDetailsVisible));
            }
        }

        private string _viewMoreButtonText = "View More";
        private string _viewMoreClubButtonText = "View More";
        public string ViewMoreButtonText
        {
            get => _viewMoreButtonText;
            set
            {
                _viewMoreButtonText = value;
                OnPropertyChanged(nameof(ViewMoreButtonText));
            }
        }
        public string ViewMoreClubButtonText
        {
            get => _viewMoreClubButtonText;
            set
            {
                _viewMoreClubButtonText = value;
                OnPropertyChanged(nameof(ViewMoreClubButtonText));
            }
        }

        public EventsPageViewModel(ProfileViewModel profileViewModel)
        {
            // Sample data for LSU Events
            EventsToday = new ObservableCollection<Event>
            {
                new Event
                {
                    Title = "EsportsLSU Super Smash Bros Melee Tournament",
                    Organization = "EsportsLSU",
                    Location = "Student Union",
                    Time = "6:00 PM",
                    Image = "melee_tournament.png",
                    Description = "Competitive Super Smash Bros tournament.",
                    Date = "12/09/2024"
                },
                new Event
                {
                    Title = "Art Sale",
                    Organization = "Art Club",
                    Location = "Free Speech Alley",
                    Time = "10:00 AM",
                    Image = "art_sale.png",
                    Description = "A variety of student-created art for sale.",
                    Date = "12/09/2024"
                },
                new Event
                {
                    Title = "Night of 1000 Donuts",
                    Organization = "Library Club",
                    Location = "Library",
                    Time = "8:00 PM",
                    Image = "donut_night.png",
                    Description = "Enjoy donuts, coffee, and camaraderie.",
                    Date = "12/09/2024"
                },
                new Event
                {
                    Title = "Nifty Fifties Nutcracker Auditions",
                    Organization = "Theater Club",
                    Location = "Coates Hall",
                    Time = "5:00 PM",
                    Image = "nutcracker_auditions.png",
                    Description = "Audition for the Nutcracker in a fifties twist at Hopkins Black Box Theater.",
                    Date = "12/10/2024"
                }
            };

            // Sample data for Club Events
            ClubEvents = new ObservableCollection<Event>
            {
                new Event
                {
                    Title = "Digital Art Showcase",
                    Organization = "DADA",
                    Location = "LSU Art Building",
                    Time = "4:00 PM",
                    Image = "digital_art_showcase.png",
                    Description = "Showcase of digital art projects from members.",
                    Date = "12/12/2024"
                },
                new Event
                {
                    Title = "Ceramic Workshop",
                    Organization = "CASA",
                    Location = "Art & Design Building",
                    Time = "2:00 PM",
                    Image = "ceramic_workshop.png",
                    Description = "Hands-on ceramic making workshop.",
                    Date = "12/13/2024"
                },
                new Event
                {
                    Title = "Kayaking Trip Planning",
                    Organization = "Kayaking Club",
                    Location = "LSU Student Union",
                    Time = "6:00 PM",
                    Image = "kayaking_trip.png",
                    Description = "Plan and organize the upcoming kayaking trip.",
                    Date = "12/14/2024"
                },
                new Event
                {
                    Title = "Chess Tournament",
                    Organization = "Chess Club",
                    Location = "Prescott Hall",
                    Time = "3:00 PM",
                    Image = "chess_tournament.png",
                    Description = "Compete in a friendly chess tournament.",
                    Date = "12/15/2024"
                }
            };


            StudentClubs = new ObservableCollection<string>(profileViewModel.Clubs);
            EnrolledClubEvents = new ObservableCollection<Event>(ClubEvents.Where(e => StudentClubs.Contains(e.Organization)));
            FilteredClubEvents = new ObservableCollection<Event>(EnrolledClubEvents.Take(2));
            FilteredEventsToday = new ObservableCollection<Event>(EventsToday.Take(2));
            ViewMoreCommand = new Command(OnViewMore);
            ViewMoreClubCommand = new Command(OnViewMoreClub);
            EventClickedCommand = new Command<Event>(OnEventClicked);
            CloseEventDetailsCommand = new Command(CloseEventDetails);
            OpenMapCommand = new Command(async () => await OpenMap());
        }

        private void OnViewMore()
        {
            if (FilteredEventsToday.Count < EventsToday.Count)
            {
                FilteredEventsToday = new ObservableCollection<Event>(EventsToday);
                ViewMoreButtonText = "View Less";
            }
            else
            {
                FilteredEventsToday = new ObservableCollection<Event>(EventsToday.Take(2));
                ViewMoreButtonText = "View More";
            }
            OnPropertyChanged(nameof(FilteredEventsToday));
        }

        private void OnViewMoreClub()
        {
            if (FilteredClubEvents.Count < EnrolledClubEvents.Count)
            {
                FilteredClubEvents = new ObservableCollection<Event>(EnrolledClubEvents);
                ViewMoreClubButtonText = "View Less";
            }
            else
            {
                FilteredClubEvents = new ObservableCollection<Event>(EnrolledClubEvents.Take(2));
                ViewMoreClubButtonText = "View More";
            }
            OnPropertyChanged(nameof(FilteredClubEvents));
        }

        private void OnEventClicked(Event eventDetails)
        {
            if (eventDetails == null) return;

            SelectedEvent = eventDetails;
            IsEventDetailsVisible = true;
        }

        private void CloseEventDetails()
        {
            IsEventDetailsVisible = false;
        }

        private async Task OpenMap()
        {
            if (SelectedEvent != null)
            {
                var navigationParameter = new Dictionary<string, object>
        {
            { "eventTitle", SelectedEvent.Title },
            { "eventLocation", SelectedEvent.Location }
        };

                await Shell.Current.GoToAsync("main", navigationParameter);
            }
        }
    }

    public class Event
    {
        public string Title { get; set; }
        public string Organization { get; set; }
        public string Location { get; set; }
        public string Time { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
    }
}
