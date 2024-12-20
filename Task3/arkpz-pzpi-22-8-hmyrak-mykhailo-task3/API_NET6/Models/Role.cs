using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class Role
    {
        public Role()
        {
            BooksPeople = new HashSet<BooksPerson>();
        }

        public int RoleId { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<BooksPerson> BooksPeople { get; set; }
    }
}
