using System.Drawing;
using System.IO;
using Tesseract;

namespace Translator.Service
{
    public class OcrService : IDisposable
    {
        private readonly string _tessDataPath;
        private readonly Dictionary<string, TesseractEngine> _engines = new();

        public OcrService()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _tessDataPath = Path.Combine(appData, "Translator", "tessdata");
            if (!Directory.Exists(_tessDataPath)) Directory.CreateDirectory(_tessDataPath);
        }
        public string RecognizeImage(Bitmap image, string language)
        {
            if (!_engines.ContainsKey(language))
            {
                var engine = new TesseractEngine(_tessDataPath, language, EngineMode.Default);
                _engines[language] = engine;
            }

            using (var pix = PixConverter.ToPix(image))
            using (var page = _engines[language].Process(pix))
            {
                return page.GetText();
            }
        }

        public void Dispose()
        {
            foreach (var engine in _engines.Values) engine.Dispose();
        }

    }
}
