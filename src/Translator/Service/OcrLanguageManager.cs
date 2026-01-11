using System.IO;
using System.Net.Http;
using Translator.Model;

namespace Translator.Service
{
    public class OcrLanguageManager
    {
        private readonly string _tessDataPath;
        private readonly HttpClient _http = new();
        public OcrLanguageManager() 
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _tessDataPath = Path.Combine(appData, "Translator", "tessdata");

            if (!Directory.Exists(_tessDataPath))
                Directory.CreateDirectory(_tessDataPath);
        }
        public async Task<bool> InstallLanguageAsync(string language, CancellationToken cancellationToken = default)
        {
            string languageCode;
            try
            {
                languageCode = LanguageService.GetOcrLanguageCode(language);
            }
            catch
            {
                return false;
            }

            if (IsLanguageInstalled(language)) throw new InvalidOperationException("Language already installed");

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
            try
            {
                var languageCode = LanguageService.GetOcrLanguageCode(language);
                var file = Path.Combine(_tessDataPath, $"{languageCode}.traineddata");
                return File.Exists(file);
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteLanguage(string language)
        {
            if (!IsLanguageInstalled(language)) throw new InvalidOperationException("Language not installed");

            try
            {
                var languageCode = LanguageService.GetOcrLanguageCode(language);
                var fileName = $"{languageCode}.traineddata";
                var filePath = Path.Combine(_tessDataPath, fileName);
                File.Delete(filePath);
                return true;
            }
            catch 
            {
                return false;
            }
            
        }

        public List<OcrLanguage> GetAllLanguagesInfo()
        {
            var result = new List<OcrLanguage>();

            foreach (var lang in LanguageService.GetAllOcrLanguages())
            {
                result.Add(new OcrLanguage 
                { 
                    Code=LanguageService.GetOcrLanguageCode(lang), 
                    Name=lang, 
                    IsInstalled=IsLanguageInstalled(lang) 
                });
            }
            return result;
        }
    }
}
