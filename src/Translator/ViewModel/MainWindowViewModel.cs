using ReactiveUI;
using System.Collections.ObjectModel;
using Translator.Service;

namespace Translator.ViewModel
{
    public class MainWindowViewModel : ReactiveObject
    {
        private string? _selectedSourceLanguage = null;
        private string? _selectedTargetLanguage = null;
        private string? _hotkey = "non assigned";

        public ObservableCollection<string> SourceLanguages { get; } = new(LanguageService.GetAllOcrLanguages());
        public ObservableCollection<string> TargetLanguages { get; } = new(LanguageService.GetAllTranslationLanguages());

        public string? SelectedSourceLanguage
        { 
            get => _selectedSourceLanguage;
            set => this.RaiseAndSetIfChanged(ref _selectedSourceLanguage, value);
        }

        public string? SelectedTargetLanguage
        {
            get => _selectedTargetLanguage;
            set => this.RaiseAndSetIfChanged(ref _selectedTargetLanguage, value);
        }

        public string? Hotkey
        {
            get => _hotkey;
            set => this.RaiseAndSetIfChanged(ref _hotkey, value);
        }


        public MainWindowViewModel()
        {

        }

    }
}