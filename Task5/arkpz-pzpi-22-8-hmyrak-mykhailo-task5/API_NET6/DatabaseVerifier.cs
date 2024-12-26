using API_NET6.Models;
using Microsoft.EntityFrameworkCore;


namespace API_NET6
{
    public static class DatabaseVerifier
    {
        public static void VerifyDatabase(libraryContext db)
        {

            // Check if there is at least one record in the Param table
            bool hasParams = db.Params.Any();
            if (hasParams)
            {
                Console.WriteLine("Database verification completed.");
            }
            else
            {
                Console.WriteLine("The Param table does not contain any records. Inserting a new record.");

                //db.Database.ExecuteSqlRaw("ALTER DATABASE CURRENT COLLATE Cyrillic_General_CI_AI");


                // Insert a new record with specified values
                var newParam = new Param
                {
                    TempMax = 40,
                    TempMin = 10,
                    WetMax = 80,
                    WetMin = 5
                };

                db.Params.Add(newParam);
                db.SaveChanges();

                Console.WriteLine("A new record has been inserted into the Param table.");

                // Insert a new admin user
                var adminUser = new User
                {
                    Login = "admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345"),
                    Role = "Admin"
                };

                db.Users.Add(adminUser);
                db.SaveChanges();

                Console.WriteLine("Admin user has been inserted into the Users table.");

                // Insert statuses
                var statuses = new List<Status>
                    {
                        new Status { StatusId = 1, Name = "Поступила" },
                        new Status { StatusId = 2, Name = "Видана" },
                        new Status { StatusId = 3, Name = "Повернута" },
                        new Status { StatusId = 4, Name = "До архиву" },
                        new Status { StatusId = 5, Name = "З архиву" },
                        new Status { StatusId = 6, Name = "Коментар" },
                        new Status { StatusId = 7, Name = "Списана" },
                        new Status { StatusId = 8, Name = "Резерв" }
                    };

                db.Statuses.AddRange(statuses);
                db.SaveChanges();

                Console.WriteLine("Statuses have been inserted into the Status table.");
            }
        }
    }
}
