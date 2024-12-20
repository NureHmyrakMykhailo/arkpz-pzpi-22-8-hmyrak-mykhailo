using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class Person
    {
        public Person()
        {
            BooksPeople = new HashSet<BooksPerson>();
        }

        public int PersonId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string? Country { get; set; }
        public bool? IsReal { get; set; }

        public virtual ICollection<BooksPerson> BooksPeople { get; set; }
    }
}
