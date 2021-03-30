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

namespace CinemaProj
{
    /// <summary>
    /// Логика взаимодействия для OrdersWindow.xaml
    /// </summary>
    public partial class OrdersWindow : Window
    {
        public OrdersWindow()
        {
            InitializeComponent();
        }

        private async void btnDelete_ClickAsync(object sender, RoutedEventArgs e)
        {
            var forDelete = dgOrders.SelectedItems;
            var ordersForDelete = new List<Order>();
            var updatetickets = new List<Ticket>();

            if (dgOrders.SelectedItems.Count != 0)
            {
                foreach(Order order in forDelete)
                {
                    foreach (var ticket in order.Tickets)
                        updatetickets.Add(new Ticket
                            {
                                TicketID = ticket.TicketID,
                                CinemaSessionID = ticket.CinemaSessionID,
                                Place = ticket.Place,
                                Status = true
                            });

                    ordersForDelete.Add(new Order
                    {
                        ID = order.ID,
                        StringTicketsInOrder = order.StringTicketsInOrder,
                        CinemaSessionName = order.CinemaSessionName,
                        SaleTime = order.SaleTime,
                        Tickets = updatetickets
                    });
                }
                    
                using (var db = new CinemaDB(MainWindow.option))
                {
                    db.Orders.RemoveRange(ordersForDelete);
                    db.Tickets.UpdateRange(updatetickets);
                    await db.SaveChangesAsync();
                }
                MainWindowViewModel.ClearOrders();

                using (var db = new CinemaDB(MainWindow.option))
                {
                    var orders = await db.Orders.Include(o => o.Tickets).ToArrayAsync();
                    MainWindowViewModel.LoadOrders(orders);
                }
            }
        }
    }
}
