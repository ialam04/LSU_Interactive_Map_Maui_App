using System.Collections.ObjectModel;
using System.Windows.Input;
using InteractiveLSUMap.Models;
using Microsoft.Maui.Controls;
using InteractiveLSUMap;

namespace InteractiveLSUMap.ViewModels
{
    public class MemoriesViewModel : BaseViewModel
    {
        public ObservableCollection<Memory> Memories { get; set; }
        public ICommand AddNewMemoryCommand { get; }

        public MemoriesViewModel()
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
            Memories.Add(memory);
        }
    }
}