using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Service.Models
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }

        public virtual int VehicleMakeId { get; set; }
        public virtual VehicleMake VehicleMake { get; set; }
    }
}
