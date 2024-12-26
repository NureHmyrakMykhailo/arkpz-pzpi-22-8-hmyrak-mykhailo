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
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
            }
        }


        public void EnsureDatabaseWithCollation(string collation)
        {
            if (Database.EnsureCreated())
            {
                Database.ExecuteSqlRaw($"ALTER DATABASE [{Database.GetDbConnection().Database}] COLLATE {collation}");
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
                    .HasColumnName("isbn")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Lang)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("lang")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Pages).HasColumnName("pages");

                entity.Property(e => e.Publish)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("publish")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("title")
                    .HasComment("Назва")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Year)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("year")
                    .UseCollation("Cyrillic_General_CI_AI");

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
                    .HasColumnName("name")
                    .UseCollation("Cyrillic_General_CI_AI");
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
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.Wet).HasColumnName("wet");
            });

            modelBuilder.Entity<Hist>(entity =>
            {
                entity.ToTable("hist");

                entity.Property(e => e.HistId).HasColumnName("hist_id");

                entity.Property(e => e.Comment)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("comment")
                    .UseCollation("Cyrillic_General_CI_AI");

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
                    .HasColumnName("description")
                    .UseCollation("Cyrillic_General_CI_AI");

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
                    .HasColumnName("country")
                    .UseCollation("Cyrillic_General_CI_AI");

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
                    .HasColumnName("name")
                    .UseCollation("Cyrillic_General_CI_AI");
            });

            modelBuilder.Entity<Reader>(entity =>
            {
                entity.ToTable("readers");

                entity.Property(e => e.ReaderId).HasColumnName("reader_id");

                entity.Property(e => e.Address)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("address")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Birthday)
                    .HasColumnType("date")
                    .HasColumnName("birthday");

                entity.Property(e => e.Class).HasColumnName("class");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("email")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("name")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Phone)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("phone")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.StudentCard)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("student_card")
                    .UseCollation("Cyrillic_General_CI_AI");
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
                    .HasColumnName("name")
                    .UseCollation("Cyrillic_General_CI_AI");
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
                    .HasColumnName("name")
                    .UseCollation("Cyrillic_General_CI_AI");
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

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Login)
                    .HasMaxLength(50)
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(512)
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('User')")
                    .UseCollation("Cyrillic_General_CI_AI");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
