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
        private string? _hotkey = "not assigned";

        public ObservableCollection<string> SourceLanguages { get; } = new(LanguageService.GetAllOcrLanguages());
        public ObservableCollection<string> TargetLanguages { get; } = new(LanguageService.GetAllTranslationLanguages());

        public string? SelectedSourceLanguage
        {
            get => _selectedSourceLanguage;
            set => SetSourceLanguageWithCheck(value);
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

        private void SetSourceLanguageWithCheck(string? language)
        {
            if (language == null)
            {
                this.RaiseAndSetIfChanged(ref _selectedSourceLanguage, null);
                return;
            }

            if (!_ocrLanguageManager.IsLanguageInstalled(language))
            {
                var result = MessageBox.Show(
                    $"Language '{language}' is not installed. Install now?",
                    "Language Installation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question
                );

                if (result == MessageBoxResult.Yes)
                {
                    InstallLanguageAsync(language);
                    this.RaiseAndSetIfChanged(ref _selectedSourceLanguage, language);
                }
                else
                {
                    return;
                }
            }
            else
            {
                this.RaiseAndSetIfChanged(ref _selectedSourceLanguage, language);
            }
        }

        private async void InstallLanguageAsync(string language)
        {
            try
            {
                await _ocrLanguageManager.InstallLanguageAsync(language);
                MessageBox.Show($"{language} installed successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to install {language}: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.RaiseAndSetIfChanged(ref _selectedSourceLanguage, null, nameof(SelectedSourceLanguage));
            }
        }

        public async Task<string?> TranslateFromBitmapAsync(Bitmap bitmap)
        {
            if (SelectedSourceLanguage == null ||
                SelectedTargetLanguage == null ||
                bitmap == null ||
                !_ocrLanguageManager.IsLanguageInstalled(SelectedSourceLanguage))
            {
                return null;
            }

            var sourceText = _ocrService.RecognizeImage(bitmap, SelectedSourceLanguage);

            if (string.IsNullOrEmpty(sourceText))
                return null;

            return await _translationService.TranslateTextAsync(sourceText, SelectedTargetLanguage);
        }
    }
}