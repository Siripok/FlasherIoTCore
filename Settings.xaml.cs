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
    /// 
    public class DeviceIOT
    {

        public string Type { get; set; }
        public string[] TypesDevicesSettings { get; set; }

        public int numPIN { get; set; }

        public int numButton { get; set; }

        public bool reverseBULB { get; set; }

        public bool dimBRIGHTNESS { get; set; }
        public bool SMOTH { get; set; }
        public bool IKsenor { get; set; }


    }

    public partial class Settings : Window
    {
        public List<DeviceIOT> Listedevice;

        string[] DevicesSettings = { "svet", "retrotop_up" , "none" };


        public Settings(List <FlashInfo> fi)
        {
            InitializeComponent();
            ShowListBox();            

        }



        public void ShowListBox()
        {

            Listedevice = new List<DeviceIOT>() {
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings} ,
            new DeviceIOT() { Type = "retrotop_up", TypesDevicesSettings=DevicesSettings},
            new DeviceIOT() {Type= "none",TypesDevicesSettings=DevicesSettings }
        };

            lb_settings.ItemsSource = Listedevice;
        }
       
        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            lb_settings.Items.Refresh();
        }

        private void cmd_Edit_Clicked(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is DeviceIOT)
            {
                DeviceIOT msg = (DeviceIOT)cmd.DataContext;

                switch (msg.Type)
                {

                    case "svet": 
                        svet sv = new svet(Listedevice);                        
                        sv.ShowDialog();                     
                        break;

                    case "retrotop_up":
                        retrotop rt = new retrotop();
                        rt.ShowDialog();                     
                        break;

                    case "none":
                        break;

                    default:
                        MessageBox.Show($"Непонятно(", "?", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
                ShowSetting();                
            }
        }
        public void ShowSetting()
        {

            lb_settings.Items.Refresh();
        }
    }
    

}