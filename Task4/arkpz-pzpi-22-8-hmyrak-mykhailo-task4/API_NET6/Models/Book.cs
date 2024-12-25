using System;
using System.Collections.Generic;

namespace API_NET6.Models
{
    public partial class Book
    {
        public Book()
        {
            BooksPeople = new HashSet<BooksPerson>();
            Items = new HashSet<Item>();
        }

        public int BookId { get; set; }
        /// <summary>
        /// Назва
        /// </summary>
        public string Title { get; set; } = null!;
        public string? Isbn { get; set; }
        public int? Pages { get; set; }
        public string? Publish { get; set; }
        public int? CategoryId { get; set; }
        public int? Class { get; set; }
        public string? Lang { get; set; }
        public string? Year { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<BooksPerson> BooksPeople { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
}
