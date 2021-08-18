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

        public int num { get; set; } //№ по списку
        public int numPIN { get; set; }
        public int numButton { get; set; }
        public bool reverseBULB { get; set; }
        public bool dimBRIGHTNESS { get; set; }
        public bool SMOTH { get; set; }
        public bool IKsenor { get; set; }
        public bool MechanicBtn { get; set; }
        public string Opisanie { get; set; }


    }

    public partial class Settings : Window
    {
        public List<DeviceIOT> Listdevice;

        string[] DevicesSettings = { "svet", "retrotop_up", "none" };

        
        public Settings(int elem)
        {
            InitializeComponent();
            ShowListBox();
          
        }

        public void ShowListBox()
        {

            Listdevice = new List<DeviceIOT>() {
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings} ,
            new DeviceIOT() {Type = "retrotop_up", TypesDevicesSettings=DevicesSettings},
            new DeviceIOT() {Type= "none",TypesDevicesSettings=DevicesSettings }
        };

            lb_settings.ItemsSource = Listdevice;
            for (int i = 0; i < 6; i++)
            {
                Listdevice[i].num = i+1;
            }

            lb_settings.Items.Refresh();
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {

        }
        int u;//текущий элемент
        private void cmd_Edit_Clicked(object sender, RoutedEventArgs e)
        {
            Button cmd = (Button)sender;
            if (cmd.DataContext is DeviceIOT)
            {
                DeviceIOT msg = (DeviceIOT)cmd.DataContext;
                u = msg.num - 1;
                switch (msg.Type)
                {

                    case "svet":
                        svet sv = new svet(Listdevice, u);
                        sv.ShowDialog();
                        ShowSettingSvet(u);
                        break;

                    case "retrotop_up":
                        retrotop rt = new retrotop(Listdevice, u);
                        rt.ShowDialog();
                        ShowSettingRetrotop(u);
                        break;

                    case "none":
                        break;

                    default:
                        MessageBox.Show($"Непонятно что за тип устр-ва :(", "?", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }

                lb_settings.Items.Refresh();
            }
        }
        public void ShowSettingSvet(int i)
        {

            //Listedevice[i].Opisanie = $"PIN {Listedevice[i].numPIN }, Button {Listedevice[i].numButton}, ";
            Listdevice[i].Opisanie = "Выбрано:\n";
            if (Listdevice[i].reverseBULB == true)
                Listdevice[i].Opisanie += "Инвертировать, ";


            if (Listdevice[i].dimBRIGHTNESS == true)
                Listdevice[i].Opisanie += "Регулировка яркости, ";


            if (Listdevice[i].SMOTH == true)
                Listdevice[i].Opisanie += "Плавное включение, ";


            if (Listdevice[i].IKsenor == true)
                Listdevice[i].Opisanie += "ИК сенсор ";

        }

        public void ShowSettingRetrotop(int i)
        {

            // Listedevice[i].Opisanie = $"PIN {Listedevice[i].numPIN }, Button {Listedevice[i].numButton}, ";
            Listdevice[i].Opisanie = "Выбрано: ";
            if (Listdevice[i].MechanicBtn == true)
                Listdevice[i].Opisanie += "Механическая кнопка";

        }

        private void btn_Post_Click(object sender, RoutedEventArgs e)
        {
            //Listdevice[u].Type=?
            this.Close();
        }
    }


}