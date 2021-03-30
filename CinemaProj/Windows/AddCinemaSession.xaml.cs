using CinemaProj.Data;
using CinemaProj.Entityes;
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
    /// Логика взаимодействия для AddCinemaSession.xaml
    /// </summary>
    public partial class AddCinemaSession : Window
    {
        public AddCinemaSession()
        {
            InitializeComponent();
        }

        private async void btnAdd_ClickAsync(object sender, RoutedEventArgs e)
        {
            string movieName = MovieName.Text;
            DateTime movieTime = (DateTime)MovieTime.Value;
            int countTickets = int.Parse(CountTickets.Text);
            using (var db = new CinemaDB(MainWindow.option))
            {
                await db.Sessions.AddAsync(new CinemaSession
                    {
                        Name = movieName,
                        Begin = movieTime,
                        Tickets = Enumerable.Range(1, countTickets)
                                    .Select(i => new Ticket
                                    {
                                        Place = i,
                                        Status = true,
                                    }).ToArray()
                    });
                await db.SaveChangesAsync();
            }
            MessageBox.Show($"Киносеанс {movieName} в {movieTime}\n" +
                $" в количестве {countTickets} билетов\n" +
                $"добавлен успешно!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
