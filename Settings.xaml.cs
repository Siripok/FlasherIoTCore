using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public string[] DevicesSettings = { "svet", "retrotop-up" };
        public Settings()
        {
            InitializeComponent();           
            ShowListBox();
           
        }
        

        public void ShowListBox()
        {
            //lb_settings.ItemsSource = Devices;
            lb_settings.ItemsSource = DevicesSettings;
            lb_settings.Items.Add("1");
            lb_settings.Items.Add("2");
        
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {

            lb_settings.Items.Refresh();

        }
    }
}
