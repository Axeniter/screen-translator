using GTranslate.Translators;

namespace Translator.Service
{
    public class TranslationService
    {
        private GoogleTranslator2 _translator = new();

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
