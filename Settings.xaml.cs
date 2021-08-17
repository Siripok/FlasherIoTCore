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
        public int numPIN2 { get; set; }
        public int numButton2 { get; set; }
        public bool reverseBULB { get; set; }
        public bool dimBRIGHTNESS { get; set; }
        public bool SMOTH { get; set; }
        public bool IKsenor { get; set; }
        public bool MechanicBtn { get; set; }
        public string Opisanie { get; set; }


        public void ShowSettingSvet()
        {

            //Listedevice[i].Opisanie = $"PIN {Listedevice[i].numPIN }, Button {Listedevice[i].numButton}, ";
            Opisanie = "Выбрано:\n";

            switch (Type)
            {
                case "svet":
                    if (reverseBULB == true)
                        Opisanie += "Инвертировать, ";


                    if (dimBRIGHTNESS == true)
                        Opisanie += "Регулировка яркости, ";


                    if (SMOTH == true)
                        Opisanie += "Плавное включение, ";


                    if (IKsenor == true)
                        Opisanie += "ИК сенсор ";
                    break;
                case "retrotop_up":
                    if (MechanicBtn == true)
                        Opisanie += "Механическая кнопка";
                    break;
            }
            Opisanie= Opisanie == "Выбрано:\n" ?  "" :  Opisanie ;
            Opisanie = Opisanie.Trim();
        }

      


    }

    public partial class Settings : Window
    {
        public List<DeviceIOT> Listdevice;

        string[] DevicesSettings = { "svet", "retrotop_up", "none" };

        int c;
        public Settings(int elem)
        {
            InitializeComponent();
            ShowListBox();
            c = elem;

        }



        public void ShowListBox()
        {

            Listdevice = new List<DeviceIOT>() {
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=32,numButton=21,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=33,numButton=22,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=25,numButton=19,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=12,numButton=23,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type = "retrotop_up", TypesDevicesSettings=DevicesSettings,numPIN=27,numButton=18,numPIN2=26,numButton2=5},
            new DeviceIOT() {Type= "none",TypesDevicesSettings=DevicesSettings }
        };

            lb_settings.ItemsSource = Listdevice;
            for (int i = 0; i < Listdevice.Count; i++)
            {
                Listdevice[i].num = i+1;

                Listdevice[i].ShowSettingSvet();
            }

            lb_settings.Items.Refresh();
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {

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
                        svet sv = new svet(msg);
                        sv.ShowDialog();
                        msg.ShowSettingSvet();
                        break;

                    case "retrotop_up":
                        retrotop rt = new retrotop(msg);
                        rt.ShowDialog();
                        msg.ShowSettingSvet();
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
     

        private void btn_Post_Click(object sender, RoutedEventArgs e)
        {
            //Listdevice[u].Type=?
            this.Close();
        }
    }


}