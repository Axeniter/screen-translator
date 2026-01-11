using System.IO;
using System.Net.Http;

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

            var fileName = $"{languageCode}.traineddata";
            var filePath = Path.Combine(_tessDataPath, fileName);
            var tempFilePath = Path.Combine(_tessDataPath, $"{languageCode}_{Guid.NewGuid()}.temp");

            if (File.Exists(tempFilePath))
            {
                return false;
            }

            try
            {
                var url = $"https://github.com/tesseract-ocr/tessdata/raw/main/{fileName}";

                using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

                await using var fileStream = new FileStream(
                    tempFilePath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    bufferSize: 81920,
                    useAsync: true
                );

                await stream.CopyToAsync(fileStream, cancellationToken);
                await fileStream.FlushAsync(cancellationToken);

                fileStream.Close();

                await Task.Delay(100);

                if (File.Exists(filePath))
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch (IOException ex)
                    {
                        var backupPath = $"{filePath}.backup_{DateTime.Now:yyyyMMddHHmmss}";
                        File.Move(filePath, backupPath);
                    }
                }
                File.Move(tempFilePath, filePath);

                await Task.Delay(500);
                return File.Exists(filePath);
            }
            catch (IOException ioEx)
            {
                await SafeDeleteFile(tempFilePath);
                await SafeDeleteFile(filePath);

                return false;
            }
            catch (Exception ex)
            {
                await SafeDeleteFile(tempFilePath);
                return false;
            }
        }
        private async Task SafeDeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    File.Delete(filePath);
                    return;
                }
                catch (IOException)
                {
                    await Task.Delay(100 * (i + 1));
                }
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

        public List<string> GetAllInstalledLanguages()
        {
            var result = new List<string>();

            foreach (var lang in LanguageService.GetAllOcrLanguages())
            {
                if (IsLanguageInstalled(lang)) result.Add(lang);
            }
            return result;
        }
    }
}
