using Project.MVC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Models
{
    public class VehicleModelViewModel : IVehicleModelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public VehicleMakeViewModel VehicleMake { get; set; }
    }
}
