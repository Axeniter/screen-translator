using ReactiveUI;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using Translator.Service;

namespace Translator.ViewModel
{
    /// <summary>
    /// ViewModel for the main application window, providing data and operations for OCR and translation
    /// </summary>
    /// <remarks>
    /// Manages language selection and hotkey display
    /// </remarks>
    public class MainWindowViewModel : ReactiveObject
    {
        private readonly OcrService _ocrService = new();
        private readonly OcrLanguageManager _ocrLanguageManager = new();
        private readonly TranslationService _translationService = new();

        private string? _selectedSourceLanguage = null;
        private string? _selectedTargetLanguage = null;

        /// <summary>
        /// Collection of source languages available for OCR
        /// </summary>
        /// <value>
        /// Observable collection containing display names of all supported OCR languages
        /// </value>
        public ObservableCollection<string> SourceLanguages { get; } = new(LanguageService.GetAllOcrLanguages());

        /// <summary>
        /// Collection of target languages available for translation
        /// </summary>
        /// <value>
        /// Observable collection containing display names of all supported translation languages
        /// </value>
        public ObservableCollection<string> TargetLanguages { get; } = new(LanguageService.GetAllTranslationLanguages());

        /// <summary>
        /// Gets or sets the currently selected source language for OCR
        /// </summary>
        /// <value>
        /// The display name of the selected source language, or null if no language is selected
        /// </value>
        /// <remarks>
        /// When setting a new language, automatically checks if the language is installed
        /// and prompts for installation if needed
        /// </remarks>
        public string? SelectedSourceLanguage
        {
            get => _selectedSourceLanguage;
            set => SetSourceLanguageWithCheck(value);
        }

        /// <summary>
        /// Gets or sets the currently selected target language for translation
        /// </summary>
        /// <value>
        /// The display name of the selected target language, or null if no language is selected
        /// </value>
        public string? SelectedTargetLanguage
        {
            get => _selectedTargetLanguage;
            set => this.RaiseAndSetIfChanged(ref _selectedTargetLanguage, value);
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

        /// <summary>
        /// Performs OCR on a bitmap image and translates the recognized text
        /// </summary>
        /// <param name="bitmap">Bitmap image containing text to recognize and translate</param>
        /// <returns>
        /// Task result contains translated text, or null if any step in the process fails
        /// </returns>
        /// <remarks>
        /// This method performs the following steps:
        /// 1. Validates that source and target languages are selected
        /// 2. Verifies the source language is installed
        /// 3. Performs OCR on the bitmap using the selected source language
        /// 4. Translates the recognized text to the selected target language
        /// </remarks>
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