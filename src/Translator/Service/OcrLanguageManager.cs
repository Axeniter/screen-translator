using System.IO;
using System.Net.Http;
using Translator.Model;

namespace Translator.Service
{
    public class OcrLanguageManager
    {
        private readonly string _tessDataPath;
        private readonly HttpClient _http = new();
        private readonly Dictionary<string, string> _languages = new()
        {
            ["English"] = "eng",
            ["Russian"] = "rus",
            ["Japanse"] = "jpn",
            ["Chinese Simplified"] = "chi_sim",
            ["Chinese Traditional"] = "chi_tra"

        };
        public OcrLanguageManager() 
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _tessDataPath = Path.Combine(appData, "Translator", "tessdata");

            if (!Directory.Exists(_tessDataPath))
                Directory.CreateDirectory(_tessDataPath);
        }
        public async Task<bool> InstallLanguageAsync(string language, CancellationToken cancellationToken = default)
        {
            if (!_languages.TryGetValue(language, out var languageCode)) throw new ArgumentException();
            if (IsLanguageInstalled(language)) return true;

            var fileName = $"{languageCode}.traineddata";
            var filePath = Path.Combine(_tessDataPath, fileName);

            var tempFilePath = $"{filePath}.temp";

            try
            {
                var url = $"https://github.com/tesseract-ocr/tessdata/raw/main/{fileName}";

                using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                await using var fileStream = new FileStream(tempFilePath, FileMode.Create, 
                    FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);

                await stream.CopyToAsync(fileStream, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);

                File.Move(tempFilePath, filePath, true);
                return true;
            }
            catch (OperationCanceledException)
            {
                File.Delete(tempFilePath);
                throw;
            }
            catch
            {
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);

                if (File.Exists(filePath))
                    File.Delete(filePath);

                return false;
            }
        }

        public bool IsLanguageInstalled(string language)
        {
            if (!_languages.ContainsKey(language)) return false;
            var languageCode = _languages[language];
            var file = Path.Combine(_tessDataPath, $"{languageCode}.traineddata");
            return File.Exists(file);
        }

        public bool DeleteLanguage(string language)
        {
            if (!_languages.TryGetValue(language, out var languageCode)) throw new ArgumentException();
            if (!IsLanguageInstalled(language)) return true;

            var fileName = $"{languageCode}.traineddata";
            var filePath = Path.Combine(_tessDataPath, fileName);

            try
            {
                File.Delete(filePath);
                return true;
            }
            catch 
            {
                return false;
            }
            
        }

        public string GetLanguageCode(string language)
        {
            return _languages[language];
        }

        public List<OcrLanguage> GetAllLanguagesInfo()
        {
            var result = new List<OcrLanguage>();

            foreach (var lang in _languages)
            {
                result.Add(new OcrLanguage { Code=lang.Value, Name=lang.Key, IsInstalled=IsLanguageInstalled(lang.Key) });
            }
            return result;
        }

        public List<string> GetAllLanguagesNames()
        {
            var result = new List<string>();

            foreach (var lang in _languages.Keys) result.Add(lang);
            return result;
        }
    }
}
