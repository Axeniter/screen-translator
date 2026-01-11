namespace Translator.Service
{
    /// <summary>
    /// Provides language code mapping and validation services for OCR and translation operations
    /// </summary>
    /// <remarks>
    /// This service maintains mappings between human-readable language names and their corresponding
    /// codes for Tesseract OCR and Google Translate API
    /// </remarks>
    public static class LanguageService
    {
        private static readonly Dictionary<string, string> _ocrLanguages = new()
        {
            ["English"] = "eng",
            ["Russian"] = "rus",
            ["Japanse"] = "jpn",
            ["Chinese Simplified"] = "chi_sim",
            ["Chinese Traditional"] = "chi_tra"
        };

        private static readonly Dictionary<string, string> _translationLanguages = new()
        {
            ["English"] = "en",
            ["Russian"] = "ru",
            ["Japanese"] = "ja",
            ["Chinese Simplified"] = "zh-CN",
            ["Chinese Traditional"] = "zh-TW",
            ["Spanish"] = "es",
            ["French"] = "fr",
            ["German"] = "de",
            ["Arabic"] = "ar",
            ["Portuguese"] = "pt",
            ["Hindi"] = "hi",
            ["Korean"] = "ko",
            ["Italian"] = "it",
            ["Turkish"] = "tr"
        };

        /// <summary>
        /// Determines whether the specified language is supported for OCR operations
        /// </summary>
        /// <param name="language">Display name of the language ("English", "Russian")</param>
        /// <returns>true if the language is supported for OCR and otherwise false</returns>
        public static bool IsOcrLanguageSupported(string language)
        {
            return _ocrLanguages.ContainsKey(language);
        }

        /// <summary>
        /// Gets Tesseract OCR language code from the display language name
        /// </summary>
        /// <param name="language">Display name of the language ("English", "Russian")</param>
        /// <returns>Tesseract language code ("eng", "rus")</returns>
        public static string GetOcrLanguageCode(string language)
        {
            if (!IsOcrLanguageSupported(language)) throw new ArgumentException("Language ocr not supported");
            var code = _ocrLanguages[language];
            return code;
        }

        /// <summary>
        /// Determines whether the specified language is supported for translation operations
        /// </summary>
        /// <param name="language">Display name of the language ("English", "Russian")</param>
        /// <returns>true if the language is supported for translation and false otherwise</returns>
        public static bool IsTranslationLanguageSupported(string language)
        {
            return _translationLanguages.ContainsKey(language);
        }

        /// <summary>
        /// Gets Google Translate language code from the display language name
        /// </summary>
        /// <param name="language">Display name of the language ("English", "Russian")</param>
        /// <returns>Google Translate language code ("en", "ru")</returns>
        public static string GetTranslationLanguageCode(string language)
        {
            if (!IsTranslationLanguageSupported(language)) throw new ArgumentException("Language translation not supported");
            var code = _translationLanguages[language];
            return code;
        }

        /// <summary>
        /// Retrieves all display language names supported for OCR operations
        /// </summary>
        /// <returns>List of display language names supported for OCR</returns>
        public static List<string> GetAllOcrLanguages()
        {
            var result = new List<string>();
            foreach (var lang in  _ocrLanguages.Keys) result.Add(lang);
            return result;
        }

        /// <summary>
        /// Retrieves all display language names supported for translation operations
        /// </summary>
        /// <returns>List of display language names supported for translation</returns>
        public static List<string> GetAllTranslationLanguages()
        {
            var result = new List<string>();
            foreach (var lang in _translationLanguages.Keys) result.Add(lang);
            return result;
        }
    }
}
