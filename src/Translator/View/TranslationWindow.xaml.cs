using System.Windows;

namespace Translator.View
{
    /// <summary>
    /// Represents a window that displays translated text to the user
    /// </summary>
    public partial class TranslationWindow : Window
    {
        public TranslationWindow(string translatedText)
        {
            InitializeComponent();
            TranslatedText.Text = translatedText;
        }

        public TranslationWindow()
        {
            InitializeComponent();
            TranslatedText.Text = "no text";
        }
    }
}
