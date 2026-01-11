using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Translator.Model
{
    /// <summary>
    /// Registers a global system-wide hotkey that works even when the application is not in focus
    /// </summary>
    public class GlobalHotkey : IDisposable
    {
        private const int WM_HOTKEY = 0x0312;

        private readonly IntPtr _hwnd;
        private readonly HwndSource _source;
        private readonly ModifierKeys _modifiers;
        private readonly Key _key;
        private int _id;
        private bool _registered;

        /// <summary>
        /// Event that is raised when hotkey is pressed
        /// </summary>
        public event EventHandler? Pressed;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// Initializes new instance of the GlobalHotkey class and registers hotkey
        /// </summary>
        /// <param name="modifiers">Modifier keys (Alt, Ctrl, Shift, Windows) to combine with the key</param>
        /// <param name="key">Primary key of hotkey combination</param>
        /// <param name="window">Window that will receive the hotkey notifications</param>
        public GlobalHotkey(ModifierKeys modifiers, Key key, Window window)
        {
            var helper = new WindowInteropHelper(window);

            _hwnd = helper.EnsureHandle();
            _source = HwndSource.FromHwnd(_hwnd) ?? throw new InvalidOperationException();
            _source.AddHook(WndProc);
            _id = GetHashCode();
            _modifiers = modifiers;
            _key = key;

            Register();
        }
        private static uint ConvertModifiers(ModifierKeys modifiers)
        {
            uint mask = 0;
            if (modifiers.HasFlag(ModifierKeys.Alt)) mask |= 0x0001;
            if (modifiers.HasFlag(ModifierKeys.Control)) mask |= 0x0002;
            if (modifiers.HasFlag(ModifierKeys.Shift)) mask |= 0x0004;
            if (modifiers.HasFlag(ModifierKeys.Windows)) mask |= 0x0008;
            return mask;
        }

        private void Register()
        {
            uint virtualKey = (uint)KeyInterop.VirtualKeyFromKey(_key);
            uint modifierMask = ConvertModifiers(_modifiers);

            _registered = RegisterHotKey(_hwnd, _id, modifierMask, virtualKey);
            if (!_registered)
            {
                throw new InvalidOperationException();
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == _id)
            {
                Pressed?.Invoke(this, EventArgs.Empty);
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_registered)
            {
                UnregisterHotKey(_hwnd, _id);
                _registered = false;
            }
            Pressed = null;
            _source.RemoveHook(WndProc); 
        }

        ~GlobalHotkey()
        {
            Dispose();
        }
    }
}