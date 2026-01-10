using System.Collections.ObjectModel;
using ReactiveUI;

namespace Translator.ViewModel
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string _selectedSourceLanguage;
        private string _selectedTargetLanguage;
        private string _hotKey;

        public ObservableCollection<string> SourceLanguages { get; } = new() { "ru", "en" };
        public ObservableCollection<string> TargetLanguages { get; } = new() { "ru", "en" };

        public string SelectedSourceLanguage
        { 
            get => _selectedSourceLanguage;
            set => this.RaiseAndSetIfChanged(ref _selectedSourceLanguage, value);
        }

        public string SelectedTargetLanguage
        {
            get => _selectedTargetLanguage;
            set => this.RaiseAndSetIfChanged(ref _selectedTargetLanguage, value);
        }

        public string HotKey
        {
            get => _hotKey;
            set => this.RaiseAndSetIfChanged(ref _hotKey, value);
        }
    }
}