using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Project.MVC.Interfaces;
using Project.MVC.Models;
using Project.Service.Interfaces;
using static Project.MVC.HelpClasses.NotificationEnum;

namespace Project.MVC.Controllers
{
    public class VehicleBaseController : Controller
    {
        protected readonly IVehicleService _service;
        protected readonly IMapper _mapper;
        protected SearchSortExClass _searchSortParams;

        public VehicleBaseController(IVehicleService service, IMapper mapper, SearchSortExClass searchSortParams)
        {
            _service = service;
            _mapper = mapper;
            _searchSortParams = searchSortParams;
        }

        public void SweetAlert(string message, NotificationType notificationType)
        {
            var alert = "<script language='javascript'>swal('" + notificationType.ToString() + "', '" + message + "','" + notificationType + "')" + "</script>";
            TempData["NotificationAlert"] = alert;
        }

    }
}