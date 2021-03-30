using CinemaProj.Data;
using CinemaProj.Entityes;
using CinemaProj.ViewModels;
using Microsoft.EntityFrameworkCore;
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

namespace CinemaProj.Windows
{
    /// <summary>
    /// Логика взаимодействия для RemoveCinemaSession.xaml
    /// </summary>
    public partial class RemoveCinemaSession : Window
    {
        public RemoveCinemaSession()
        {
            InitializeComponent();
        }

        private async void cbMovies_DropDownOpenedAsync(object sender, EventArgs e)
        {
            MainWindowViewModel.ClearMovies();
            using (var db = new CinemaDB(MainWindow.option))
            {
                var movies = await db.Sessions.Select(i => i.Name).Distinct().ToArrayAsync();
                MainWindowViewModel.LoadMovies(movies);
            }
        }

        private async void cbMovies_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            var selectedItem = (CinemaSession)comboBox.SelectedItem;
            if (comboBox.SelectedItem != null)
            {
                using (var db = new CinemaDB(MainWindow.option))
                {
                    var movies = await db.Sessions.Where(i => i.Name == selectedItem.Name).ToArrayAsync();
                    var times = movies.Select(i => i.Begin).ToArray();
                    cbTimes.ItemsSource = times;
                }
                cbTimes.SelectedIndex = -1;
                MainWindowViewModel.ClearTickets();
            }
        }

        private async void btnRemove_ClickAsync(object sender, RoutedEventArgs e)
        {
            if(cbMovies.SelectedItem != null && cbTimes.SelectedItem != null)
            {
                var selectedmovie = cbMovies.SelectedItem;
                CinemaSession session = (CinemaSession)selectedmovie;
                DateTime movieTime = (DateTime)cbTimes.SelectedItem;
                using (var db = new CinemaDB(MainWindow.option))
                {
                    var movie = await db.Sessions
                        .Where(i => i.Name ==  session.Name && i.Begin == movieTime).ToArrayAsync();
                    var orders = await db.Orders
                        .Include(o => o.Tickets)
                        .Where(o => o.CinemaSessionID == movie[0].ID).ToArrayAsync();

                    db.Orders.RemoveRange(orders);
                    db.Sessions.RemoveRange(movie);
                    await db.SaveChangesAsync();
                }
                MessageBox.Show($"Киносеанс {session.Name} в {movieTime} удален!",
                    "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
