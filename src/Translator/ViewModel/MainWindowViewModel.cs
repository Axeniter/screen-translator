using ReactiveUI;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using Translator.Service;

namespace Translator.ViewModel
{
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly OcrService _ocrService = new();
        private readonly OcrLanguageManager _ocrLanguageManager = new();
        private readonly TranslationService _translationService = new();

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

        public async Task<string?> TranslateFromBitmapAsync(Bitmap bitmap)
        {
            if (SelectedSourceLanguage == null) return null;
            if (SelectedTargetLanguage == null) return null;
            if (bitmap == null) throw new ArgumentNullException();
            if (!_ocrLanguageManager.IsLanguageInstalled(SelectedSourceLanguage))
            {
                MessageBox.Show("Language not installed");
            }

            var sourceText = _ocrService.RecognizeImage(bitmap, SelectedSourceLanguage);

            if (string.IsNullOrEmpty(sourceText)) return null;

            var translation = await _translationService.TranslateTextAsync(sourceText, SelectedTargetLanguage);

            return translation;
        }

    }
}