using KiwiClient.Classes;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace KiwiClient.Classes
{
    public enum Keys
    {
        VK_CAPITAL = 0x14,
        VK_RETURN = 0x0D,
        VK_BACK = 0x08
    }

    public enum Modifiers
    {
        MOD_NONE = 0x0000, //(none)
        MOD_ALT = 0x0001, //ALT
        MOD_CONTROL = 0x0002, //CTRL
        MOD_SHIFT = 0x0004, //SHIFT
        MOD_WIN = 0x0008 //WINDOWS
    }

    public class Hotkey
    {
        private Window _hwnd;
        private const int HOTKEY_ID = 9000;


        public delegate void HotkeyPressedHandler(HotkeyEventArgs e);
        public event HotkeyPressedHandler HotkeyPressed;

        public Hotkey(Window hwnd)
        {
            _hwnd = hwnd;
            var source = HwndSource.FromHwnd(new WindowInteropHelper(_hwnd).Handle);
            source.AddHook(HwndHook);
            Register(new WindowInteropHelper(_hwnd).Handle);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private void Register(IntPtr handle)
        {
            RegisterHotKey(handle, HOTKEY_ID, (uint)Modifiers.MOD_NONE, (uint)Keys.VK_RETURN); 
        }

        
        public void RegisterKey(Keys key, Modifiers modifier = Modifiers.MOD_NONE)
        {
            RegisterHotKey(new WindowInteropHelper(_hwnd).Handle, HOTKEY_ID, (uint)modifier, (uint)key); 
        }



        /// <summary>
        /// Needs to be called before the window is closed
        /// </summary>
        public void Unregister()
        {
            var source = HwndSource.FromHwnd(new WindowInteropHelper(_hwnd).Handle);
            source.RemoveHook(HwndHook);
            UnregisterHotKey(new WindowInteropHelper(_hwnd).Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            Keys vkey = (Keys)(((uint)lParam >> 16) & 0xFFFF);
                            HotkeyPressed.Invoke(new HotkeyEventArgs(vkey));
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }

    public class HotkeyEventArgs : EventArgs
    {
        public Keys PressedKey { get; }
        public Modifiers PressedModifier { get; }

        public HotkeyEventArgs()
        {
        }

        public HotkeyEventArgs(Keys key, Modifiers modifier = Modifiers.MOD_NONE)
        {
            PressedKey = key;
            PressedModifier = modifier;
        }
    }
}
