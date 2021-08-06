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
            //rich.Document.Blocks.Clear();
        }

        private void SaveClose_Click(object sender, RoutedEventArgs e)
        {
           string RichText = new TextRange(rich.Document.ContentStart, rich.Document.ContentEnd).Text;
           string[] s = RichText.Split(' ');
          // MessageBox.Show(s[]);

            
            
            
           this.Close();
        }
    }
}
