using AutoMapper;
using Project.MVC.Models;
using Project.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.MVC.ModelMapper
{
    public class VehicleMapperProfile : Profile
    {
        public VehicleMapperProfile()
        {
            CreateMap<VehicleMake, VehicleMakeViewModel>().ReverseMap();
            CreateMap<VehicleModel, VehicleModelViewModel>().ReverseMap();
        }
    }
}
