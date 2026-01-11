using System.Drawing;
using System.IO;
using Tesseract;

namespace Translator.Service
{
    /// <summary>
    /// Provides OCR operations using Tesseract engine
    /// </summary>
    public class OcrService : IDisposable
    {
        private readonly string _tessDataPath;
        private readonly Dictionary<string, TesseractEngine> _engines = new();

        /// <summary>
        /// Initializes a new instance of the OcrService class
        /// </summary>
        /// <remarks>
        /// Ensures the tessdata directory exists in the LocalApplicationData folder
        /// </remarks>
        public OcrService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _tessDataPath = Path.Combine(appData, "Translator", "tessdata");
            if (!Directory.Exists(_tessDataPath)) Directory.CreateDirectory(_tessDataPath);
        }

        /// <summary>
        /// Recognizes text in the bitmap image using the given language
        /// </summary>
        /// <param name="image">The bitmap image containing text to recognize</param>
        /// <param name="language">The display name of the language to use for OCR ("English", "Russian")</param>
        /// <returns>
        /// Recognized text as a string, or null if recognition failed or the language is not supported
        /// </returns>
        /// <remarks>
        /// Creates and caches Tesseract engine instances for each language to improve performance
        /// on subsequent recognitions with the same language
        /// </remarks>
        public string? RecognizeImage(Bitmap image, string language)
        {
            string languageCode;
            try
            {
                languageCode = LanguageService.GetOcrLanguageCode(language);
            }
            catch
            {
                return null;
            }

            if (!_engines.ContainsKey(languageCode))
            {
                var engine = new TesseractEngine(_tessDataPath, languageCode, EngineMode.Default);
                _engines[languageCode] = engine;
            }

            using (var pix = PixConverter.ToPix(image))
            using (var page = _engines[languageCode].Process(pix))
            {
                var result = page.GetText();
                if (string.IsNullOrEmpty(result)) return null;
                return result;
            }
        }

        public void Dispose()
        {
            foreach (var engine in _engines.Values) engine.Dispose();
        }

    }
}
