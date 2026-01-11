using System.Windows;
using System.Windows.Input;

namespace Translator.View
{
    public partial class HotkeyDialog : Window
    {
        public ModifierKeys Modifiers { get; private set; }
        public Key Key { get; private set; }

        public HotkeyDialog()
        {
            try
            {
                InitializeComponent();

                Modifiers = ModifierKeys.None;
                Key = Key.None;
                Focusable = true;
                PreviewKeyDown += OnPreviewKeyDown;

                Loaded += (s, e) =>
                {
                    Focus();
                    UpdateButtonState();
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in HotkeyDialog: {ex.Message}");
                throw;
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsModifierKey(e.Key))
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter || e.Key == Key.Escape || e.Key == Key.System)
            {
                e.Handled = true;
                return;
            }

            Modifiers = Keyboard.Modifiers;
            Key = e.Key;

            UpdateDisplay();
            UpdateButtonState();

            e.Handled = true;
        }

        private bool IsModifierKey(Key key)
        {
            return key == Key.LeftCtrl || key == Key.RightCtrl ||
                   key == Key.LeftAlt || key == Key.RightAlt ||
                   key == Key.LeftShift || key == Key.RightShift ||
                   key == Key.LWin || key == Key.RWin;
        }

        private void UpdateDisplay()
        {
            if (Key == Key.None)
            {
                HotkeyTextBlock.Text = "(none)";
                return;
            }

            string display = "";

            if (Modifiers.HasFlag(ModifierKeys.Control))
                display += "Ctrl + ";
            if (Modifiers.HasFlag(ModifierKeys.Alt))
                display += "Alt + ";
            if (Modifiers.HasFlag(ModifierKeys.Shift))
                display += "Shift + ";
            if (Modifiers.HasFlag(ModifierKeys.Windows))
                display += "Win + ";

            display += GetKeyString(Key);

            HotkeyTextBlock.Text = display;
        }

        private void UpdateButtonState()
        {
            OkButton.IsEnabled = (Key != Key.None);
        }

        private string GetKeyString(Key key)
        {
            return key.ToString();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (Key == Key.None)
            {
                MessageBox.Show("Select a key combination",
                    "No Key Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = false;
            }
        }
    }
}