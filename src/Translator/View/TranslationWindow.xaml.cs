using System.Windows;

namespace Translator.View
{
    /// <summary>
    /// Interaction logic for TranslationWindow.xaml
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
