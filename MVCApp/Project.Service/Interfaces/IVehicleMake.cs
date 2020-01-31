using Project.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Service.Interfaces
{
    public interface IVehicleMake
    {
        int Id { get; set; }
        string Name { get; set; }
        string Abbreviation { get; set; }

        ICollection<VehicleModel> VehicleModels { get; set; }
    }
}
