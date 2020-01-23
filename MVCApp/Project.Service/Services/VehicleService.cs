using Microsoft.EntityFrameworkCore;
using Project.Service.DataContext;
using Project.Service.Interfaces;
using Project.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly CarContext _dbCarContext;
        public VehicleService(CarContext dbCarContext)
        {
            _dbCarContext = dbCarContext;
        }


        public async Task<List<VehicleMake>> GetVehicleMakeAsync()
        {
            return await _dbCarContext.VehicleMake.ToListAsync();
        }


        public async Task CreateVehicleMakeAsync(VehicleMake vehicleMake)
        {
            _dbCarContext.Add(vehicleMake);
            await _dbCarContext.SaveChangesAsync();
        }
    }
}
