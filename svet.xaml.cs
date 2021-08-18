using System;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для svet.xaml
    /// </summary>
    public partial class svet : Window
    {
        DeviceIOT t;
    
        public svet(DeviceIOT param)
        {
            InitializeComponent();
         
            t = param;
            
            ShowElement();
          
        }

        public void ShowElement()
        {
            txt_numPIN.Clear();
            txt_numBUTTON.Clear();
            if (t.numButton.ToString() != ""&& t.numPIN.ToString()!="") //если заполнены были поля
            {

                txt_numPIN.Text = t.numPIN.ToString();
                txt_numBUTTON.Text = t.numButton.ToString();

                if (t.reverseBULB == true) chk_reverseBULB.IsChecked = true;
                if (t.dimBRIGHTNESS == true) chk_dimBRIGHTNESS.IsChecked = true;
                if (t.SMOTH == true) chk_SMOTH.IsChecked = true;
                if (t.IKsenor == true) chk_IKsenor.IsChecked = true;
            }
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

            if (txt_numPIN.Text != "" && txt_numBUTTON.Text != "")
            {

                t.numPIN = Convert.ToInt32(txt_numPIN.Text);
                t.numButton = Convert.ToInt32(txt_numBUTTON.Text);

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

