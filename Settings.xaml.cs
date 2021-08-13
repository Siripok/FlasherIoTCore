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
        public string Opisanie {get;set;}


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
            for (int i = 0; i < 6; i++)
            {
                Listedevice[i].num = i;
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
                        svet sv = new svet(Listedevice,msg.num);                        
                        sv.ShowDialog();
                        ShowSettingSvet(msg.num);
                        break;

                    case "retrotop_up":
                        retrotop rt = new retrotop();
                        rt.ShowDialog();
                        ShowSettingRetrotop(msg.num);
                        break;

                    case "none":
                        break;

                    default:
                        MessageBox.Show($"Непонятно(", "?", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                }
              
                lb_settings.Items.Refresh();
            }
        }
        public void ShowSettingSvet(int i)
        {
            
                //Listedevice[i].Opisanie = $"PIN {Listedevice[i].numPIN }, Button {Listedevice[i].numButton}, ";

                if (Listedevice[i].reverseBULB == true)
                    Listedevice[i].Opisanie = "Инвертировать, ";
               

                if(Listedevice[i].dimBRIGHTNESS == true)
                    Listedevice[i].Opisanie += "Регилировка яркости, ";
                

               if(Listedevice[i].SMOTH == true)
                    Listedevice[i].Opisanie += "Плавное включение, ";
             

               if(Listedevice[i].IKsenor == true)
                    Listedevice[i].Opisanie += "ИК сенсор ";
              
            
          
        }

        public void ShowSettingRetrotop(int i)
        {
           
               // Listedevice[i].Opisanie = $"PIN {Listedevice[i].numPIN }, Button {Listedevice[i].numButton}, ";

                if (Listedevice[i].MechanicBtn == true)
                    Listedevice[i].Opisanie = "Механическая кнопка";
                          
            
           
        }
    }
    

}