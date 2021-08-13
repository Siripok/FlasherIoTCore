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
        int elem;
        public svet(List<DeviceIOT> param,int num)
        {
            InitializeComponent();
            t = param;
            elem = num;
            ShowElement();
          
        }

        public void ShowElement()
        {
            if (t[elem].numButton.ToString() != ""&& t[elem].numPIN.ToString()!="") //если заполены были поля
            {

                txt_numPIN.Text = t[elem].numPIN.ToString();
                txt_numBUTTON.Text = t[elem].numButton.ToString();

                if (t[elem].reverseBULB == true) chk_reverseBULB.IsChecked = true;
                if (t[elem].dimBRIGHTNESS == true) chk_dimBRIGHTNESS.IsChecked = true;
                if (t[elem].SMOTH == true) chk_SMOTH.IsChecked = true;
                if (t[elem].IKsenor == true) chk_IKsenor.IsChecked = true;
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

            if (txt_numPIN.Text != "" && txt_numBUTTON.Text != "")
            {

                t[elem].numPIN = Convert.ToInt32(txt_numPIN.Text);
                t[elem].numButton = Convert.ToInt32(txt_numBUTTON.Text);

                 if (chk_reverseBULB.IsChecked == true)
                    t[elem].reverseBULB = true;
                 else
                    t[elem].reverseBULB = false;


                 if (chk_dimBRIGHTNESS.IsChecked == true)
                    t[elem].dimBRIGHTNESS = true;
                else t[elem].dimBRIGHTNESS = false;


                 if (chk_SMOTH.IsChecked == true)
                    t[elem].SMOTH = true;
                else t[elem].SMOTH = false;


                 if (chk_IKsenor.IsChecked == true)
                    t[elem].IKsenor = true;
                 else t[elem].IKsenor = false;

                this.Close();
            }
            else
                MessageBox.Show("Заполните PIN и укажите кнопку!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);


        }


    }


}

