using Microsoft.EntityFrameworkCore;
using Project.Service.DataContext;
using Project.Service.Interfaces;
using Project.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

        #region VehicleMake
        // method to return list of vehicle makes
        public async Task<List<VehicleMake>> GetVehicleMakeAsync()
        {
            return await _dbCarContext.VehicleMake.ToListAsync();
        }


        // method to create vehicle make
        public async Task CreateVehicleMakeAsync(VehicleMake vehicleMake)
        {
            _dbCarContext.VehicleMake.Add(vehicleMake);
            await _dbCarContext.SaveChangesAsync();
        }


        // method to get vehicle make by id
        public async Task<VehicleMake> GetVehicleMakeByIdAsync(int? id)
        {
            var vehicle = await _dbCarContext.VehicleMake.FindAsync(id);
            return vehicle;
        }


        // method to get vehicle make by name
        public async Task<VehicleMake> GetVehicleMakeByNameAsync(string name)
        {
            // return vehicle make model if exists, also with setup notracking _db context
            return await _dbCarContext.VehicleMake.AsNoTracking().Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
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

            // check if some model exists of that brand and also remove all models with brand
            var modelVehiclesExist = await CheckIfExistsVehicleModelByMakeIdAsync(id);
            if (modelVehiclesExist)
                _dbCarContext.VehicleModel.RemoveRange(_dbCarContext.VehicleModel.Where(x => x.VehicleMakeId == id));

            await _dbCarContext.SaveChangesAsync();
        }

        #endregion





        #region VehicleModel
        // method to return list of vehicle models
        public async Task<List<VehicleModel>> GetVehicleModelAsync()
        {
            var vehicleModels = await _dbCarContext.VehicleModel.ToListAsync();

            foreach (var vehicleModel in vehicleModels)
            {
                vehicleModel.VehicleMake = await GetVehicleMakeByIdAsync(vehicleModel.VehicleMakeId);
            }

            return vehicleModels;
        }


        // method to create vehicle model
        public async Task CreateVehicleModelAsync(VehicleModel vehicleModel)
        {
            _dbCarContext.VehicleModel.Add(vehicleModel);
            await _dbCarContext.SaveChangesAsync();
        }



        // method to get vehicle model by id
        public async Task<VehicleModel> GetVehicleModelByIdAsync(int? id)
        {
            var vehicle = await _dbCarContext.VehicleModel.FindAsync(id);
            return vehicle;
        }

        // method to get vehicle make by name
        public async Task<VehicleModel> GetVehicleModelByNameAsync(string name)
        {
            // return vehicle make model if exists, also with setup notracking _db context
            return await _dbCarContext.VehicleModel.AsNoTracking().Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefaultAsync();
        }


        // method to get vehicle model by vehicle make id ## used for deleting models in the same time with vehicle brand (make)
        public async Task<bool> CheckIfExistsVehicleModelByMakeIdAsync(int? vehicleMakeId)
        {
            var vehicle = await _dbCarContext.VehicleModel.Where(x => x.VehicleMakeId == vehicleMakeId).AnyAsync();
            return vehicle;
        }




        // method to update current vehicle make
        public async Task UpdateVehicleModelAsync(VehicleModel vehicleMake)
        {
            _dbCarContext.VehicleModel.Update(vehicleMake);
            await _dbCarContext.SaveChangesAsync();
        }

        // method to check if vehicle make by id exists
        public async Task<bool> CheckVehicleModelAsync(int id)
        {
            return await _dbCarContext.VehicleModel.AnyAsync(x => x.Id == id);
        }


        // method to delete vehicle make
        public async Task DeleteVehicleModelAsync(int id)
        {
            var vehicle = await GetVehicleModelByIdAsync(id);
            _dbCarContext.VehicleModel.Remove(vehicle);
            await _dbCarContext.SaveChangesAsync();
        }



        #endregion











    }
}
