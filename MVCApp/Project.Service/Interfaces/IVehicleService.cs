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
    }
}
