using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Model
{
    public class ComputerList
    {
        public IEnumerable<Computers> Computers { get; set; }   
        public ComputerFilter Filter { get; set; }
    }
}
