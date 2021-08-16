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
    /// Логика взаимодействия для retrotop.xaml
    /// </summary>
    public partial class retrotop : Window
    {
        List<DeviceIOT> t;
        int i;
        public retrotop(List<DeviceIOT> Listedevice, int num)
        {
            InitializeComponent();
            t = Listedevice;
            i = num;
            ShowElement();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

            if (txt_numPIN.Text != "" && txt_numBUTTON.Text != "")
            {
               

                t[i].numPIN = Convert.ToInt32(txt_numPIN.Text);
                t[i].numButton = Convert.ToInt32(txt_numBUTTON.Text);

                if (chk_mechanicBtn.IsChecked == true)
                    t[i].MechanicBtn = true;
                else
                    t[i].MechanicBtn = false;

                this.Close();
            }
            else
                MessageBox.Show("Заполните PIN и укажите кнопку!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        public void ShowElement()
        {
            txt_numPIN.Clear();
            txt_numBUTTON.Clear();
            if (t[i].numButton.ToString() != "" && t[i].numPIN.ToString() != "") //если заполнены были поля
            {

                txt_numPIN.Text = t[i].numPIN.ToString();
                txt_numBUTTON.Text = t[i].numButton.ToString();

                if (t[i].MechanicBtn == true) chk_mechanicBtn.IsChecked = true;
                
            }
        }
    }
}
