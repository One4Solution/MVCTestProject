using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project.MVC.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.Models
{
    public class VehicleMakeViewModel : IVehicleMakeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This field cannot be empty")]
        [StringLength(50, ErrorMessage = "Name length must be between 2 and 50 characters", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field cannot be empty")]
        [StringLength(10, ErrorMessage = "Abbreviation length must be between 2 and 10 characters", MinimumLength = 2)]
        public string Abbreviation { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; }

        public ICollection<VehicleModelViewModel> VehicleModels { get; set; }

    }
}
