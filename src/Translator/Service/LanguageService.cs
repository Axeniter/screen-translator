namespace Translator.Service
{
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

        public static bool IsOcrLanguageSupported(string language)
        {
            return _ocrLanguages.ContainsKey(language);
        }
        public static string GetOcrLanguageCode(string language)
        {
            if (!IsOcrLanguageSupported(language)) throw new ArgumentException("Language ocr not supported");
            var code = _ocrLanguages[language];
            return code;
        }
        public static bool IsTranslationLanguageSupported(string language)
        {
            return _translationLanguages.ContainsKey(language);
        }
        public static string GetTranslationLanguageCode(string language)
        {
            if (!IsTranslationLanguageSupported(language)) throw new ArgumentException("Language translation not supported");
            var code = _translationLanguages[language];
            return code;
        }
        public static List<string> GetAllOcrLanguages()
        {
            var result = new List<string>();
            foreach (var lang in  _ocrLanguages.Keys) result.Add(lang);
            return result;
        }
        public static List<string> GetAllTranslationLanguages()
        {
            var result = new List<string>();
            foreach (var lang in _translationLanguages.Keys) result.Add(lang);
            return result;
        }
    }
}
