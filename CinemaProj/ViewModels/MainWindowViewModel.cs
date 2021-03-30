using CinemaProj.Entityes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CinemaProj.ViewModels
{
    public static class MainWindowViewModel 
    {
        public static ObservableCollection<CinemaSession> Movies { get;} = new ObservableCollection<CinemaSession>();

        public static ObservableCollection<Ticket> Tickets { get; } = new ObservableCollection<Ticket>();
        public static ObservableCollection<Order> Orders { get; } = new ObservableCollection<Order>();

        public static void LoadMovies(string[] sessions)
        {
            foreach (var movie in sessions)
                Movies.Add(new CinemaSession { Name = movie });
            Movies.OrderBy(m => m.Name).ToArray();
        }

        public static void LoadTickets(ICollection<Ticket> tickets)
        {
            foreach (var ticket in tickets)
                Tickets.Add(new Ticket
                {
                    CinemaSessionID = ticket.CinemaSessionID,
                    Place = ticket.Place,
                    Status = ticket.Status,
                    TicketID = ticket.TicketID
                }) ;
        }
        public static void LoadOrders(ICollection<Order> orders)
        {
            foreach (var order in orders)
            {
                string result = String.Empty;
                foreach (var ticket in order.Tickets)
                    result += ticket.Place + ";";
                Orders.Add(new Order
                {
                    ID = order.ID,
                    StringTicketsInOrder = result,
                    CinemaSessionID = order.CinemaSessionID,
                    CinemaSessionName = order.CinemaSessionName,
                    SaleTime = order.SaleTime,
                    Tickets = order.Tickets
                });
            }
                
        }

        public static void ClearTickets() => Tickets.Clear();
        public static void ClearOrders() => Orders.Clear();
        public static void ClearMovies() => Movies.Clear();

    }
}
