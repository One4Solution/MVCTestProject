using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Service.Models
{
    public class VehicleMake
    {
        public VehicleMake()
        {
            VehicleModels = new HashSet<VehicleModel>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public virtual ICollection<VehicleModel> VehicleModels { get; set; }
    }
}
