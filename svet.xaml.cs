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
        int i;
        public svet(List<DeviceIOT> param,int num)
        {
            InitializeComponent();
         
            t = param;
            i = num;
            ShowElement();
          
        }

        public void ShowElement()
        {
            txt_numPIN.Clear();
            txt_numBUTTON.Clear();
            if (t[i].numButton.ToString() != ""&& t[i].numPIN.ToString()!="") //если заполнены были поля
            {

                txt_numPIN.Text = t[i].numPIN.ToString();
                txt_numBUTTON.Text = t[i].numButton.ToString();

                if (t[i].reverseBULB == true) chk_reverseBULB.IsChecked = true;
                if (t[i].dimBRIGHTNESS == true) chk_dimBRIGHTNESS.IsChecked = true;
                if (t[i].SMOTH == true) chk_SMOTH.IsChecked = true;
                if (t[i].IKsenor == true) chk_IKsenor.IsChecked = true;
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

            if (txt_numPIN.Text != "" && txt_numBUTTON.Text != "")
            {

                t[i].numPIN = Convert.ToInt32(txt_numPIN.Text);
                t[i].numButton = Convert.ToInt32(txt_numBUTTON.Text);

                 if (chk_reverseBULB.IsChecked == true)
                    t[i].reverseBULB = true;
                 else
                    t[i].reverseBULB = false;


                 if (chk_dimBRIGHTNESS.IsChecked == true)
                    t[i].dimBRIGHTNESS = true;
                else t[i].dimBRIGHTNESS = false;


                 if (chk_SMOTH.IsChecked == true)
                    t[i].SMOTH = true;
                else t[i].SMOTH = false;


                 if (chk_IKsenor.IsChecked == true)
                    t[i].IKsenor = true;
                 else t[i].IKsenor = false;

                this.Close();
            }
            else
                MessageBox.Show("Заполните PIN и укажите кнопку!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);


        }


    }


}

