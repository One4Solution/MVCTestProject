using Project.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Interfaces
{
    public interface IVehicleMakeViewModel
    {
        int Id { get; set; }
        string Name { get; set; }
        string Abbreviation { get; set; }
        string ErrorMessage { get; set; }

        // ICollection<VehicleModelViewModel> VehicleModels { get; set; }
    }
}
