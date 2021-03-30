using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CinemaProj
{
    /// <summary>
    /// Логика взаимодействия для DBWindow.xaml
    /// </summary>
    public partial class DBWindow : Window
    {
        public string Server { get; set; }
        public string DataBase { get; set; }
        public DBWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(tbDB.Text) && !String.IsNullOrEmpty(tbServer.Text))
            {
                Server = tbServer.Text;
                DataBase = tbDB.Text;
                DialogResult = true;
            }
            else
                MessageBox.Show("Введите данные!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
