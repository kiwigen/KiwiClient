using KiwiClient.Classes;
using System;
using System.ComponentModel;
using System.Windows;

namespace KiwiClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window 
    {


        Hotkey _hotkey;


        public MainWindow()
        {
            InitializeComponent();
            LCU lcu = new LCU();
            this.DataContext = lcu;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _hotkey.HotkeyPressed -= HotkeyPressed;
            _hotkey.Unregister();
            base.OnClosing(e);  
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            
            base.OnSourceInitialized(e);
            _hotkey = new Hotkey(this);
            _hotkey.HotkeyPressed += HotkeyPressed;
        }

        private void HotkeyPressed(HotkeyEventArgs e)
        {
            Keys key = e.PressedKey;
            switch (key)
            {
                case Keys.VK_CAPITAL:
                    break;
                case Keys.VK_RETURN:
                    {
                        if (this.DataContext is LCU lCU)
                        {
                            lCU.AcceptCommand.Execute(lCU.LeagueClient);
                        }
                        break;
                    }
                case Keys.VK_BACK:
                    {
                        if (this.DataContext is LCU lCU)
                        {
                            lCU.DeclineCommand.Execute(lCU.LeagueClient);
                        }
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
