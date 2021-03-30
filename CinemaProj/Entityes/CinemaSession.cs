using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaProj.Entityes
{
    public class CinemaSession
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Begin { get; set; }
        [MaxLength(100)]
        public ICollection<Ticket> Tickets { get; set; }
    }
}
