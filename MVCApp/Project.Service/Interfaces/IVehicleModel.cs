using Project.Service.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Service.Interfaces
{
    public interface IVehicleModel
    {
        int Id { get; set; }
        string Name { get; set; }
        string Abbreviation { get; set; }

        int VehicleMakeId { get; set; }
        VehicleMake VehicleMake { get; set; }
    }
}
