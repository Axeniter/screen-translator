using System.IO;
using System.Net.Http;

namespace Translator.Service
{
    /// <summary>
    /// Manages the installation, removal, and status checking of Tesseract OCR language data files
    /// </summary>
    /// <remarks>
    /// This class downloads language training data files from Tesseract GitHub repository
    /// and manages them in the local application data directory
    /// </remarks>
    public class OcrLanguageManager
    {
        private readonly string _tessDataPath;
        private readonly HttpClient _http = new();

        /// <summary>
        /// Initializes a new instance of the OcrLanguageManager class
        /// </summary>
        /// <remarks>
        /// Creates the tessdata directory in the LocalApplicationData folder if it doesn't exist
        /// </remarks>
        public OcrLanguageManager() 
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _tessDataPath = Path.Combine(appData, "Translator", "tessdata");
            
            if (!Directory.Exists(_tessDataPath))
                Directory.CreateDirectory(_tessDataPath);
        }

        /// <summary>
        /// Downloads and installs the specified language's OCR training data file
        /// </summary>
        /// <param name="language">The display name of the language to install ("English", "Russian")</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation</param>
        /// <returns>
        /// true if the language was successfully installed and false if installation failed
        /// </returns>
        /// <remarks>
        /// Downloads the .traineddata file from the Tesseract GitHub repository.
        /// Uses temporary files and error recovery mechanisms to ensure data integrity.
        /// </remarks>
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

        /// <summary>
        /// Checks whether the specified language's OCR training data is installed locally
        /// </summary>
        /// <param name="language">The display name of the language to check ("English", "Russian")</param>
        /// <returns>true if the language is installed and false otherwise</returns>
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

        /// <summary>
        /// Deletes the specified language's OCR training data file from the local system
        /// </summary>
        /// <param name="language">The display name of the language to remove ("English", "Russian")</param>
        /// <returns>true if the language was successfully deleted and false otherwise</returns>
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

        /// <summary>
        /// Retrieves a list of all OCR languages that are currently installed locally
        /// </summary>
        /// <returns>A list of display language names that are installed</returns>
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
