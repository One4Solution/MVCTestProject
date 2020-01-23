using Project.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Interfaces
{
    public interface IVehicleService
    {
        Task<List<VehicleMake>> GetVehicleMakeAsync();

        Task CreateVehicleMakeAsync(VehicleMake vehicleMake);

        Task<VehicleMake> GetVehicleMakeByIdAsync(int? id);

        Task UpdateVehicleMakeAsync(VehicleMake vehicleMake);

        Task<bool> CheckVehicleMakeAsync(int id);

        Task DeleteVehicleMakeAsync(int id);
    }
}
