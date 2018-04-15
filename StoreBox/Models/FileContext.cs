using System;
using Microsoft.EntityFrameworkCore;

namespace StoreBox.Models
{
    public class FileContext : DbContext //gestion et interaction avec la base de données
    {
       public FileContext(DbContextOptions<FileContext> options)
                : base(options) { }

            public DbSet<File> File { get; set; }
            public DbSet<User> User { get; set; }


    }
}
