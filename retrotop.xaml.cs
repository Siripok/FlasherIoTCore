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
        DeviceIOT t;
        
        public retrotop(DeviceIOT Listedevice)
        {
            InitializeComponent();
            t = Listedevice;
         
            ShowElement();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {

            if (txt_numPIN.Text != "" && txt_numBUTTON.Text != "")
            {
               

                t.numPIN = Convert.ToInt32(txt_numPIN.Text);
                t.numButton = Convert.ToInt32(txt_numBUTTON.Text);

                if (chk_mechanicBtn.IsChecked == true)
                    t.MechanicBtn = true;
                else
                    t.MechanicBtn = false;

                this.Close();
            }
            else
                MessageBox.Show("Заполните PIN и укажите кнопку!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);

        }

        public void ShowElement()
        {
            txt_numPIN.Clear();
            txt_numBUTTON.Clear();
            if (t.numButton.ToString() != "" && t.numPIN.ToString() != "") //если заполнены были поля
            {

                txt_numPIN.Text = t.numPIN.ToString();
                txt_numBUTTON.Text = t.numButton.ToString();

                if (t.MechanicBtn == true) chk_mechanicBtn.IsChecked = true;
                
            }
        }
    }
}
