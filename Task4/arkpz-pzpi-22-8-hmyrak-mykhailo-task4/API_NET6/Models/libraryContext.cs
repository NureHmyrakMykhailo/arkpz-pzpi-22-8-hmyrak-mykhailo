using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API_NET6.Models
{
    public partial class libraryContext : DbContext
    {
        public libraryContext()
        {
        }

        public libraryContext(DbContextOptions<libraryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; } = null!;
        public virtual DbSet<BooksPerson> BooksPersons { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<ClimatMonitor> ClimatMonitors { get; set; } = null!;
        public virtual DbSet<Hist> Hists { get; set; } = null!;
        public virtual DbSet<Item> Items { get; set; } = null!;
        public virtual DbSet<Param> Params { get; set; } = null!;
        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<Reader> Readers { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=localhost;Integrated Security=False;User ID=sa;Password=12345;Database=library");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("books");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Class).HasColumnName("class");

                entity.Property(e => e.Isbn)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("isbn");

                entity.Property(e => e.Lang)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lang");

                entity.Property(e => e.Pages).HasColumnName("pages");

                entity.Property(e => e.Publish)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("publish");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("title")
                    .HasComment("Назва");

                entity.Property(e => e.Year)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("year");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Books)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_books_category_id");
            });

            modelBuilder.Entity<BooksPerson>(entity =>
            {
                entity.HasKey(e => new { e.BookId, e.PersonId, e.RoleId })
                    .HasName("PK_books_authors");

                entity.ToTable("books_persons");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.BooksPeople)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK_books_authors_book_id");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.BooksPeople)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("FK_books_persons_person_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.BooksPeople)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_books_persons_role_id");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.Property(e => e.CategoryId)
                    .ValueGeneratedNever()
                    .HasColumnName("category_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<ClimatMonitor>(entity =>
            {
                entity.ToTable("climat_monitor");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Pressure).HasColumnName("pressure");

                entity.Property(e => e.Temperature).HasColumnName("temperature");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasColumnName("time")
                    .HasComputedColumnSql("(getdate())", false);

                entity.Property(e => e.Wet).HasColumnName("wet");
            });

            modelBuilder.Entity<Hist>(entity =>
            {
                entity.ToTable("hist");

                entity.Property(e => e.HistId).HasColumnName("hist_id");

                entity.Property(e => e.Comment)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("comment");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.ReaderId).HasColumnName("reader_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.Time)
                    .HasColumnType("datetime")
                    .HasColumnName("time")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Hists)
                    .HasForeignKey(d => d.ItemId)
                    .HasConstraintName("FK_hist_item_id");

                entity.HasOne(d => d.Reader)
                    .WithMany(p => p.Hists)
                    .HasForeignKey(d => d.ReaderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_hist_reader_id");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Hists)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_hist_status_id");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("items");

                entity.Property(e => e.ItemId).HasColumnName("item_id");

                entity.Property(e => e.Available)
                    .IsRequired()
                    .HasColumnName("available")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.BookId).HasColumnName("book_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("description");

                entity.Property(e => e.ReaderId).HasColumnName("reader_id");

                entity.HasOne(d => d.Book)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.BookId)
                    .HasConstraintName("FK_items_book_id");

                entity.HasOne(d => d.Reader)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.ReaderId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_items_reader_id");
            });

            modelBuilder.Entity<Param>(entity =>
            {
                entity.ToTable("param");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.TempMax).HasColumnName("temp_max");

                entity.Property(e => e.TempMin).HasColumnName("temp_min");

                entity.Property(e => e.WetMax).HasColumnName("wet_max");

                entity.Property(e => e.WetMin).HasColumnName("wet_min");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("persons");

                entity.Property(e => e.PersonId).HasColumnName("person_id");

                entity.Property(e => e.Country)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("country");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date")
                    .HasColumnName("date_of_birth");

                entity.Property(e => e.DateOfDeath)
                    .HasColumnType("date")
                    .HasColumnName("date_of_death");

                entity.Property(e => e.IsReal)
                    .IsRequired()
                    .HasColumnName("is_real")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Reader>(entity =>
            {
                entity.ToTable("readers");

                entity.Property(e => e.ReaderId).HasColumnName("reader_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address");

                entity.Property(e => e.Birthday)
                    .HasColumnType("date")
                    .HasColumnName("birthday");

                entity.Property(e => e.Class).HasColumnName("class");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("phone");

                entity.Property(e => e.StudentCard)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("student_card");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.Property(e => e.RoleId)
                    .ValueGeneratedNever()
                    .HasColumnName("role_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("statuses");

                entity.Property(e => e.StatusId)
                    .ValueGeneratedNever()
                    .HasColumnName("status_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Login, "UQ__Users__5E55825B864D4759")
                    .IsUnique();

                entity.HasIndex(e => e.Email, "UQ__Users__A9D10534684C648C")
                    .IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.Login).HasMaxLength(50);

                entity.Property(e => e.PasswordHash).HasMaxLength(512);

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('User')");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
