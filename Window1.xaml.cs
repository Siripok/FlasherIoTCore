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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            Fresh();
        }
        public void Fresh()
        {
            if (Properties.Settings.Default.prj != "" & Properties.Settings.Default.prj != null)
            {
                string g = Properties.Settings.Default.prj;
                string[] s = g.Split("\r" + "\n");

                for (int i = 0; i < s.Length; i++)
                {
                    if (!String.IsNullOrWhiteSpace(s[i]))
                        rich.AppendText(s[i] + "\r" + "\n");

                }


            }
        }

        private void SaveClose_Click(object sender, RoutedEventArgs e)
        {
            string g = rich.Text;
            string[] s = g.Split("\r" + "\n");

            Properties.Settings.Default.prj = g;           
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
