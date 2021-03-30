using CinemaProj.Data;
using CinemaProj.Entityes;
using CinemaProj.ViewModels;
using CinemaProj.Windows;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CinemaProj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Server { get; set; }
        public static string DataBase { get; set; }

        public static DbContextOptions option = new DbContextOptionsBuilder<CinemaDB>()
                .UseSqlServer($"Server={Server};Database=Cinema;Trusted_Connection=True;ConnectRetryCount=0;").Options;

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private async Task InitializeDB(CinemaDB db)
        {
            if(await db.Database.EnsureCreatedAsync())
            {
                var sessions = Enumerable.Range(1, 10).
                Select(i => new CinemaSession
                {
                    Name = $"Movie{i}",
                    Begin = new DateTime(2021, 4, 1, i + 10, 0, 0),
                    Tickets = Enumerable.Range(1, 100)
                    .Select(i => new Ticket
                    {
                        Place = i,
                        Status = true,
                    }).ToArray()
                }).ToArray();
                var dopsession = sessions.Select(i => new CinemaSession
                {
                    Name = i.Name,
                    Begin = i.Begin.AddDays(1),
                    Tickets = Enumerable.Range(1, 100)
                            .Select(i => new Ticket
                            {
                                Place = i,
                                Status = true,
                            }).ToArray()
                }).ToArray();
                await db.Sessions.AddRangeAsync(sessions);
                await db.Sessions.AddRangeAsync(dopsession);
                await db.SaveChangesAsync();
            }
        }

        private async void ConnectBDAsync(object sender, RoutedEventArgs e)
        {
            DBWindow window = new DBWindow();
            window.Owner = this;
            if(window.ShowDialog() == true)
            {
                Server = window.Server;
                DataBase = window.DataBase;
                if (await DBIsConnectAsync(Server))
                {
                    MessageBox.Show("БД успешно подключена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                    CreateBD.IsEnabled = true;
                }
                else
                    MessageBox.Show("Неверное подключение!\nПопробуйте снова!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnGo_ClickAsync(object sender, RoutedEventArgs e)
        {
            var places = dgPlaces.SelectedItems;
            var truePlacesForUpdate = new List<Ticket>();

            if(dgPlaces.SelectedItems.Count != 0)
            {
                foreach (Ticket place in places)
                    if (place.Status == true)
                        truePlacesForUpdate.Add(new Ticket
                        {
                            CinemaSessionID = place.CinemaSessionID,
                            Place = place.Place,
                            Status = false,
                            TicketID = place.TicketID
                        });
                if (truePlacesForUpdate.Count != 0) 
                {
                    using (var db = new CinemaDB(option))
                    {
                        db.Tickets.UpdateRange(truePlacesForUpdate);
                        await db.SaveChangesAsync();

                        MainWindowViewModel.ClearTickets();

                        var tickets = await db.Tickets.Where(i => i.CinemaSessionID == truePlacesForUpdate[0].CinemaSessionID).OrderBy(t => t.Place).ToArrayAsync();
                        MainWindowViewModel.LoadTickets(tickets);

                        var movie = await db.Sessions.Where(i => i.ID == truePlacesForUpdate[0].CinemaSessionID).ToArrayAsync();
                        await db.Orders.AddAsync(new Order
                        {
                            CinemaSessionName = $"{movie[0].Name} в {movie[0].Begin}",
                            CinemaSessionID = movie[0].ID,
                            SaleTime = DateTime.Now,
                            Tickets = truePlacesForUpdate
                        });
                        await db.SaveChangesAsync();

                        var orderplaces = truePlacesForUpdate.Select(i => i.Place).ToArray();
                        string orderplacesstr = String.Empty;
                        foreach (var op in orderplaces)
                            orderplacesstr += op.ToString() + ";";
                        MessageBox.Show($"Заказ на киносеанс {movie[0].Name} в {movie[0].Begin}\n" +
                            $"Места: {orderplacesstr}\n" +
                            $"Сформирован", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                    MessageBox.Show($"Выбранные места уже заняты!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task<bool> DBIsConnectAsync(string server)
        {
            SqlConnection connection = new SqlConnection($"Server={server};Database=master;Trusted_Connection=True;");
            try
            {
                await connection.OpenAsync();
                await connection.CloseAsync();
                return true;
            }
            catch (SystemException ex)
            {
                if (connection.State == ConnectionState.Open)
                {
                    await connection.CloseAsync();
                }
                return false;
            }
        }

        private async void CreateBDAsync(object sender, RoutedEventArgs e)
        {
            using (var db = new CinemaDB(option))
                await InitializeDB(db);
            using (var db = new CinemaDB(option))
            {
                var movies = await db.Sessions.Select(i => i.Name).Distinct().ToArrayAsync();
                MainWindowViewModel.LoadMovies(movies);
            }
            btnGo.IsEnabled = true;
            btnOrders.IsEnabled = true;
            AddMovie.IsEnabled = true;
            DeleteMovie.IsEnabled = true;
            cbMovies.IsEnabled = true;
            cbTimes.IsEnabled = true;
            MessageBox.Show("БД успешно загружена!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void cbMovies_SelectionChangedAsync(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            var selectedItem = (CinemaSession)comboBox.SelectedItem;
            if(comboBox.SelectedItem != null)
            {
                using (var db = new CinemaDB(option))
                {
                    var movies = await db.Sessions.Where(i => i.Name == selectedItem.Name).ToArrayAsync();
                    var times = movies.Select(i => i.Begin).ToArray();
                    cbTimes.ItemsSource = times;
                }
                cbTimes.SelectedIndex = -1;
                MainWindowViewModel.ClearTickets();
            }
        }

        private async void cbTimes_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if(comboBox.SelectedItem != null)
            {
                var time = (DateTime)comboBox.SelectedItem;
                var movie = (CinemaSession)cbMovies.SelectedItem;
                MainWindowViewModel.ClearTickets();
                using (var db = new CinemaDB(option))
                {
                    var movieID = await db.Sessions.Where(i => i.Name == movie.Name && i.Begin == time).ToArrayAsync();
                    var tickets = await db.Tickets.Where(i => i.CinemaSessionID == movieID[0].ID).OrderBy(t => t.Place).ToArrayAsync();
                    MainWindowViewModel.LoadTickets(tickets);
                }
            }
            
        }

        private async void btnOrders_ClickAsync(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel.ClearOrders();
            using (var db = new CinemaDB(option))
            {
                var orders = await db.Orders.Include(o => o.Tickets).ToArrayAsync();
                MainWindowViewModel.LoadOrders(orders);
                OrdersWindow ordersWindow = new OrdersWindow();
                ordersWindow.Owner = this;
                ordersWindow.Show();
            }
        }

        private async void AddMovie_ClickAsync(object sender, RoutedEventArgs e)
        {
            AddCinemaSession session = new AddCinemaSession();
            session.Owner = this;
            session.Show();
        }

        private async void cbMovies_DropDownOpenedAsunc(object sender, EventArgs e)
        {
            MainWindowViewModel.ClearMovies();
            using (var db = new CinemaDB(option))
            {
                var movies = await db.Sessions.Select(i => i.Name).Distinct().ToArrayAsync();
                MainWindowViewModel.LoadMovies(movies);
            }
        }

        private void DeleteMovie_Click(object sender, RoutedEventArgs e)
        {
            RemoveCinemaSession session = new RemoveCinemaSession();
            session.Owner = this;
            session.Show();
        }
    }
}
