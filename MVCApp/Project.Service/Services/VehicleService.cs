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

        // method to return list of vehicle makes
        public async Task<List<VehicleMake>> GetVehicleMakeAsync()
        {
            return await _dbCarContext.VehicleMake.ToListAsync();
        }


        // method to create vehicle make
        public async Task CreateVehicleMakeAsync(VehicleMake vehicleMake)
        {
            _dbCarContext.Add(vehicleMake);
            await _dbCarContext.SaveChangesAsync();
        }


        // method to get vehicle make by id
        public async Task<VehicleMake> GetVehicleMakeByIdAsync(int? id)
        {
            var vehicle = await _dbCarContext.VehicleMake.FindAsync(id);
            return vehicle;
        }



        // method to update current vehicle make
        public async Task UpdateVehicleMakeAsync(VehicleMake vehicleMake)
        {
            _dbCarContext.VehicleMake.Update(vehicleMake);
            await _dbCarContext.SaveChangesAsync();
        }

        // method to check if vehicle make by id exists
        public async Task<bool> CheckVehicleMakeAsync(int id)
        {
            return await _dbCarContext.VehicleMake.AnyAsync(x => x.Id == id);
        }


        // method to delete vehicle make
        public async Task DeleteVehicleMakeAsync(int id)
        {
            var vehicle = await GetVehicleMakeByIdAsync(id);
            _dbCarContext.VehicleMake.Remove(vehicle);
            await _dbCarContext.SaveChangesAsync();
        }












    }
}
