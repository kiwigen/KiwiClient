using LolAltClient.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LolAltClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            LCU lcu = new LCU();
            lcu.Activate += BringWindowToFront;
            this.DataContext = lcu;            
        }


        private void BringWindowToFront(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
                {
                    this.Activate();
                });

        }
    }
}
