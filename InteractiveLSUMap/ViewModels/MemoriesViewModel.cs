using System.Collections.ObjectModel;
using System.Windows.Input;
using InteractiveLSUMap.Models;
using Microsoft.Maui.Controls;
using InteractiveLSUMap;
using System;
using System.Collections.Generic;

namespace InteractiveLSUMap.ViewModels
{
    public class MemoriesViewModel : BaseViewModel
    {
        private static MemoriesViewModel _instance;
        public static MemoriesViewModel Instance => _instance ??= new MemoriesViewModel();

        public ObservableCollection<Memory> Memories { get; set; }
        public ICommand AddNewMemoryCommand { get; }

        public event EventHandler MemoriesChanged;

        private readonly Dictionary<string, double[]> locationCoordinates = new()
        {
            {"Prescott", new[] {-91.1808370061389, 30.41350846773524}},
            {"Nicholson", new[] {-91.17865131560713, 30.41253870863463}},
            {"Coates", new[] {-91.17906528057395, 30.413187536318254}},
            {"Library", new[] {-91.18020361641825, 30.414403792577836}},
            {"Howe-Russell", new[] {-91.178737957784, 30.41176666593225}},
            {"Student Union", new[] {-91.1772527830808, 30.412686148493126}},
            {"Lockett", new[] {-91.1818003245514, 30.413306022930918}},
            {"Art & Design", new[] {-91.17999043269066, 30.411197019821028}},
            {"Free Speech Alley", new[] {-91.17779346677747, 30.41320399284682}},
            {"Parade Ground", new[] {-91.17782299799862, 30.414488951560106}}
        };
        private MemoriesViewModel()
        {
            Memories = new ObservableCollection<Memory>();
            AddNewMemoryCommand = new Command(OnAddNewMemory);
        }

        private async void OnAddNewMemory()
        {
            // Pass 'this' (the current instance of MemoriesViewModel) to the NewMemoryPage constructor
            await Application.Current.MainPage.Navigation.PushAsync(new NewMemoryPage(this));
        }

        public void AddMemory(Memory memory)
        {
            if (locationCoordinates.TryGetValue(memory.Location, out var coords))
            {
                memory.Coordinates = coords;
            }
            Memories.Add(memory);
            MemoriesChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}