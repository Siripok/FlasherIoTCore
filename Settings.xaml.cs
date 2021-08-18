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
using System.IO;
using System.Drawing;
using System.Collections.ObjectModel;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    /// 
    public class DeviceIOT    {

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
    
    public class Images
    {
        public string imgName { get; set; }
        //public BitmapImage image { get; set; }
     
        public string ImagePath { get; set; }
        public List<DeviceIOT> settings { get; set; }
    }

 

    public partial class Settings : Window
    {
       // public List <DeviceIOT> Listdevice;
        public List <Images> Pictures;


        string[] DevicesSettings = { "svet", "retrotop_up", "none" };

   
        public Settings(int elem)
        {
            InitializeComponent();
           // ShowListBox();
      
            ShowPreview();
         
           
        }



        public void ShowListBox()
        {

            List<DeviceIOT> Listdevice = (lb_preview.SelectedItem as Images).settings;

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
        public void ShowPreview()
        {
            
            if (!Directory.Exists("img"))
                Directory.CreateDirectory("img");
             Pictures = new List<Images>();


            Pictures.Add(new Images {
                ImagePath = Environment.CurrentDirectory + "\\img\\smartkitchen_1.jpg" ,
                imgName = "SmartKitchen v1",
                settings= new List<DeviceIOT>() {
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=32,numButton=21,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=33,numButton=22,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=25,numButton=19,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=12,numButton=23,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type = "retrotop_up", TypesDevicesSettings=DevicesSettings,numPIN=27,numButton=18,numPIN2=26,numButton2=5},
            new DeviceIOT() {Type= "none",TypesDevicesSettings=DevicesSettings }
        }
        });

            Pictures.Add(new Images
            {
                ImagePath = Environment.CurrentDirectory + "\\img\\smartkitchen_2.jpg",
                imgName = "SmartKitchen v2",
                settings = new List<DeviceIOT>() {
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=111,numButton=21,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=33,numButton=22,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=25,numButton=19,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type= "svet", TypesDevicesSettings=DevicesSettings,numPIN=12,numButton=23,reverseBULB=true, dimBRIGHTNESS=true, SMOTH=true} ,
            new DeviceIOT() {Type = "retrotop_up", TypesDevicesSettings=DevicesSettings,numPIN=27,numButton=18,numPIN2=26,numButton2=5},
            new DeviceIOT() {Type= "none",TypesDevicesSettings=DevicesSettings }
        }
            });


            //string[] files = Directory.GetFiles("img");

            //if (files.Length > 0)
            //{
            //    for (int i = 0; i < files.Length; i++)
            //    {
            //        // files[i] = files[i].Trim().Replace("img\\","");
            //        files[i] =Environment.CurrentDirectory+"\\"+ files[i];
            //        MessageBox.Show(files[i]);
            //   //     Pictures.Add(new Images { ImagePath = files[i], imgName=files[i] });  //image = new BitmapImage(new Uri($"{files[i]}"))
            //        //lb_preview.Items.Add(Pictures[i]);
            //    }
            //}

            //System.Windows.Controls.Image im = new System.Windows.Controls.Image();
            //        im.Width = 100;
            //        im.Height = 100;
            //        im.Source = new BitmapImage(new Uri(files[i], UriKind.RelativeOrAbsolute));// UriKind.RelativeOrAbsolute
            //lb_preview.Items.Add(im);
            // lb_preview.Items.Add(files[i]);
            // foreach (Ima asas in Directory.GetDirectories("img"))
            // {
            //    Picture.Add(asas.Replace("img\\", ""));
            //  }
            //irectoryProject = Directory.GetDirectories("projects").;

            lb_preview.ItemsSource = Pictures;
                   lb_preview.Items.Refresh();

            lb_preview.SelectedIndex = 0;

           
        }

        private void lb_preview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowListBox();
        }
    }


}