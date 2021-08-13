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
    /// Логика взаимодействия для svet.xaml
    /// </summary>
    public partial class svet : Window
    {
        List<DeviceIOT> t;
        public svet(List<DeviceIOT> param)
        {
            InitializeComponent();
            t = param;
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

            if (txt_numPIN.Text != "" && txt_numBUTTON.Text != "")
            {/*
                Button cmd = (Button)sender;
                if (cmd.DataContext is DeviceIOT)
                    DeviceIOT msg = (DeviceIOT)cmd.DataContext;
                { 



                msg.numPIN = Convert.ToInt32(txt_numPIN.Text);
                msg.numButton = Convert.ToInt32(txt_numBUTTON.Text);

                if (chk_reverseBULB.IsChecked == true)
                    msg.reverseBULB = true;
                else
                    msg.reverseBULB = false;


                if (chk_dimBRIGHTNESS.IsChecked == true)
                    msg.dimBRIGHTNESS = true;
                else msg.dimBRIGHTNESS = false;


                if (chk_SMOTH.IsChecked == true)
                    msg.SMOTH = true;
                else msg.SMOTH = false;


                if (chk_IKsenor.IsChecked == true)
                    msg.IKsenor = true;
                else msg.IKsenor = false; 
            }*/


                this.Close();
            }
            else
                MessageBox.Show("Заполните поля!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
          
        }

      
    }
}
