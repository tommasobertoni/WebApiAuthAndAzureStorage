using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bertoni.TestFinale.Domain
{
    public class ActivityDTO
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Desc { get; set; }

        public DateTime CreationDate { get; set; }

        public int Duration { get; set; }

        public int CategoryId { get; set; }

        public string CategoryDescription { get; set; }
    }
}
