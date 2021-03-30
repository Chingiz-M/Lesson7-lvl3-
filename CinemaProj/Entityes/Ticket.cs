using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProj.Entityes
{
    public class Ticket
    {
        public int TicketID { get; set; }
        [Required]
        public int Place { get; set; }
        [Required]
        public bool Status { get; set; }
        public int CinemaSessionID { get; set; }
    }
}
