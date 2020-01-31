using Project.MVC.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Models
{
    public class VehicleModelViewModel : IVehicleModelViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field cannot be empty")]
        [StringLength(50, ErrorMessage = "Name length must be between 2 and 50 characters", MinimumLength = 2)]
        public string Name { get; set; }


        [Required(ErrorMessage = "This field cannot be empty")]
        [StringLength(30, ErrorMessage = "Abbreviation length must be between 1 and 30 characters", MinimumLength = 1)]
        public string Abbreviation { get; set; }

        public string ErrorMessage { get; set; }

        public int VehicleMakeId { get; set; }
        public VehicleMakeViewModel VehicleMake { get; set; }
        public IEnumerable<VehicleMakeViewModel> vehicleMakeModels { get; set; }
    }
}
