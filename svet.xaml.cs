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
        
        public svet(List<DeviceIOT> param)
        {
            InitializeComponent();
            param = new List<DeviceIOT>();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

            if (txt_numPIN.Text != "" && txt_numBUTTON.Text != "")
            {
                DeviceIOT t = new DeviceIOT();
                t.numPIN = Convert.ToInt32(txt_numPIN.Text);
                t.numButton = Convert.ToInt32(txt_numBUTTON.Text);

                if (chk_reverseBULB.IsChecked == true)
                    t.reverseBULB = true;
                else
                    t.reverseBULB = false;

                if (chk_dimBRIGHTNESS.IsChecked == true)
                    t.dimBRIGHTNESS = true;
                else t.dimBRIGHTNESS = false;

                if (chk_SMOTH.IsChecked == true)
                    t.SMOTH = true;
                else t.SMOTH = false;

                if (chk_IKsenor.IsChecked == true)
                    t.IKsenor = true;
                else t.IKsenor = false;


                this.Close();
            }
            else
                MessageBox.Show("Заполните поля!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
          
        }

      
    }
}
