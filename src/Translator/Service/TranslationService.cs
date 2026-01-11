using GTranslate.Translators;

namespace Translator.Service
{
    /// <summary>
    /// Provides text translation services using Google Translate
    /// </summary>
    /// <remarks>
    /// This service uses the GTranslate library to perform asynchronous text translations
    /// to various target languages
    /// </remarks>
    public class TranslationService
    {
        private GoogleTranslator2 _translator = new();

        /// <summary>
        /// Translates the specified text to the target language
        /// </summary>
        /// <param name="text">The text to translate</param>
        /// <param name="language">The display name of the target language ("English", "Japanese")</param>
        /// <returns>
        /// A task that represents the asynchronous translation operation.
        /// The task result contains the translated text, or null if translation failed
        /// </returns>
        public async Task<string?> TranslateTextAsync(string text, string language)
        {
            string languageCode;
            try
            {
                languageCode = LanguageService.GetTranslationLanguageCode(language);
            }
            catch
            {
                return null;
            }

            var result = await _translator.TranslateAsync(text, languageCode);

            return result.Translation;
        }
    }
}
