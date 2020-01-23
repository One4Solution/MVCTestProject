using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Project.Service.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Project.Service.DataContext
{
    public class CarContext : DbContext
    {
        public CarContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<VehicleMake> VehicleMake { get; set; }
        public DbSet<VehicleModel> VehicleModel { get; set; }


        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CarContext>
        {
            public CarContext CreateDbContext(string[] args)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(@Directory.GetCurrentDirectory() + "/../Project.MVC/appsettings.json")
                    .Build();

                var builder = new DbContextOptionsBuilder<CarContext>();
                var connectionString = configuration.GetConnectionString("CarDB");
                builder.UseSqlServer(connectionString);
                return new CarContext(builder.Options);
                    
            }
        }


    }
}
