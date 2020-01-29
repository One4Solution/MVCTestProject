using Project.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Interfaces
{
    public interface IVehicleModelViewModel
    {
        int Id { get; set; }

        string Name { get; set; }
        string Abbreviation { get; set; }

        string ErrorMessage { get; set; }
        int VehicleMakeId { get; set; }
        VehicleMakeViewModel VehicleMake { get; set; }
        IEnumerable<VehicleMakeViewModel> vehicleMakeModels { get; set; }
    }
}
