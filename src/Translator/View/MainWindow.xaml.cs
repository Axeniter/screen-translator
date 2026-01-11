using System.Windows;
using System.Windows.Input;
using Translator.Model;
using Translator.ViewModel;

namespace Translator.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        private GlobalHotkey? _hotkey;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        public void AssignButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new HotkeyDialog { Owner=this};
            if (dialog.ShowDialog() == true)
            {
                _viewModel.Hotkey = FormatHotkey(dialog.Modifiers, dialog.Key);

                RegisterGlobalHotkey(dialog.Modifiers, dialog.Key);
            }
        }

        private void RegisterGlobalHotkey(ModifierKeys modifiers, Key key)
        {
            _hotkey?.Dispose();
            _hotkey = null;

            if (key == Key.None) return;

            try
            {
                _hotkey = new GlobalHotkey(modifiers, key, this);
                _hotkey.Pressed += OnHotkeyPressed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void OnHotkeyPressed(object sender, EventArgs e)
        {
            MessageBox.Show("Горячая клавиша нажата!");
        }

        private string FormatHotkey(ModifierKeys modifiers, Key key)
        {
            var text = "";
            if (modifiers.HasFlag(ModifierKeys.Control)) text += "Ctrl + ";
            if (modifiers.HasFlag(ModifierKeys.Alt)) text += "Alt + ";
            if (modifiers.HasFlag(ModifierKeys.Shift)) text += "Shift + ";
            if (modifiers.HasFlag(ModifierKeys.Windows)) text += "Win + ";
            text += key.ToString();
            return text;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _hotkey?.Dispose();
        }
    }
}