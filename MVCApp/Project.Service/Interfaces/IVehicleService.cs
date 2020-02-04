using Project.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Project.Service.Interfaces
{
    public interface IVehicleService
    {
        // Make
        Task<List<VehicleMake>> GetVehicleMakeAsync();

        Task<int> GetVehicleMakeTotalAsync(string searchString);

        Task<List<VehicleMake>> GetSortedPaggedVehicleMake(string sortOrder, string searchString, string clearSearch, int pageNumber, int pageSize);

        Task CreateVehicleMakeAsync(VehicleMake vehicleMake);

        Task<VehicleMake> GetVehicleMakeByIdAsync(int? id);

        Task<VehicleMake> GetVehicleMakeByNameAsync(string name);

        Task UpdateVehicleMakeAsync(VehicleMake vehicleMake);

        Task<bool> CheckVehicleMakeAsync(int id);

        Task DeleteVehicleMakeAsync(int id);




        //Model
        Task<List<VehicleModel>> GetVehicleModelAsync();

        Task<int> GetVehicleModelTotalAsync(string searchString);

        Task<List<VehicleModel>> GetSortedPaggedVehicleModel(string sortOrder, string searchString, string clearSearch, int pageNumber, int pageSize);

        Task CreateVehicleModelAsync(VehicleModel vehicleModel);

        Task<VehicleModel> GetVehicleModelByIdAsync(int? id);

        Task<VehicleModel> GetVehicleModelByNameAsync(string name);

        Task UpdateVehicleModelAsync(VehicleModel vehicleMake);

        Task<bool> CheckVehicleModelAsync(int id);

        Task DeleteVehicleModelAsync(int id);



    }
}
