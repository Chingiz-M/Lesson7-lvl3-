using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProj.Entityes
{
    public class Order
    {
        [Key]
        public int ID { get; set; }
        public string CinemaSessionName { get; set; }
        public DateTime SaleTime { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        [NotMapped]
        public string StringTicketsInOrder { get; set; }
        public int CinemaSessionID { get; set; }
    }
}
