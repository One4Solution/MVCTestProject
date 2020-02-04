using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.MVC.HelpClasses;
using Project.MVC.Interfaces;
using Project.MVC.Models;
using Project.Service.DataContext;
using Project.Service.Interfaces;
using Project.Service.Models;
using Project.Service.Services;
using static Project.MVC.HelpClasses.NotificationEnum;

namespace Project.MVC.Controllers
{
    public class VehicleMakeController : VehicleBaseController
    {
        private static string _currentVehicleMakeName;
        private static int _currentPageSize = 0;

        public VehicleMakeController(IVehicleService service, IMapper mapper, SearchSortExClass searchSortParams) : base(service, mapper, searchSortParams) { }

        // method to set page size based on dropdown selection in the view
        public IActionResult SetPageSize(int pageSize = 4)
        {
            _currentPageSize = pageSize;
            _searchSortParams.SlectionPageSize = _currentPageSize;
            return RedirectToAction("Index");
        }


        // method to list all vehicle makes
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string clearSearch, int pageNumber = 1, int pageSize = 4)
        {
            // define pageSize based on dropdown
            pageSize = _currentPageSize > 0 ? _currentPageSize : pageSize;

            var totalVehicles = await _service.GetVehicleMakeTotalAsync(searchString); // get total number of vehicles based on search
            var result = await _service.GetSortedPaggedVehicleMake(sortOrder, searchString, clearSearch, pageNumber, pageSize);

            var vehicleMakes = _mapper.Map<List<VehicleMakeViewModel>>(result);

            var paggedVehicles = VehiclePaginationAndSorting(sortOrder, searchString, clearSearch, pageNumber, pageSize, vehicleMakes, totalVehicles);

            return View(paggedVehicles);
        }



        // method to return pagged vehicle makes
        public (PagedResult<VehicleMakeViewModel> vehicleMake, SearchSortExClass param) VehiclePaginationAndSorting(string sortOrder, string searchString, string clearSearch, int pageNumber, int pageSize, List<VehicleMakeViewModel> vehicleMakes, int totalVehicles)
        {
            // save parameters
            _searchSortParams.CurrentSortOrder = sortOrder;
            _searchSortParams.SearchFilter = searchString;
            _searchSortParams.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            _searchSortParams.AbrvSortParm = String.IsNullOrEmpty(sortOrder) ? "abrv_desc" : "";
            _searchSortParams.SlectionPageSize = _currentPageSize > 0 ? _currentPageSize : pageSize;

            // clear serach input
            if (!String.IsNullOrEmpty(clearSearch))
            {
                searchString = null;
                _searchSortParams.SearchFilter = null;
            }

            var vehicleResult = new PagedResult<VehicleMakeViewModel>
            {
                Data = vehicleMakes,
                TotalItems = totalVehicles,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return (vehicleResult, _searchSortParams);
        }


        // method to show page to create vehicle
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // method to create new vehicle
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name, Abbreviation")] VehicleMakeViewModel vehicleMakeViewModel)
        {
            if (ModelState.IsValid)
            {
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleMakeByNameAsync(vehicleMakeViewModel.Name);
                if (alreadyExists != null)
                {
                    vehicleMakeViewModel.ErrorMessage = "Name " + vehicleMakeViewModel.Name.ToString() + " already exists";
                    return View(vehicleMakeViewModel);
                }

                var vehicle = _mapper.Map<VehicleMake>(vehicleMakeViewModel);
                await _service.CreateVehicleMakeAsync(vehicle);

                ModelState.Clear(); // clear form inputs after successfully creating

                SweetAlert("Vehicle make was created!", NotificationType.success);
                Response.StatusCode = (int)HttpStatusCode.Created;
                return View();
            }

            return View(vehicleMakeViewModel);

        }


        // method to show edit page
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            var result = await _service.GetVehicleMakeByIdAsync(id);
            var vehicle = _mapper.Map<VehicleMakeViewModel>(result);

            if (vehicle == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            _currentVehicleMakeName = vehicle.Name; // save current vehicle name on edit 

            return View(vehicle);
        }


        // method to edit selected vehicle
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, [Bind("Id, Name, Abbreviation")] VehicleMakeViewModel vehicleMakeViewModel)
        {
            if (id != vehicleMakeViewModel.Id || id == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            if (ModelState.IsValid)
            {
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleMakeByNameAsync(vehicleMakeViewModel.Name);
                if (alreadyExists != null && _currentVehicleMakeName != alreadyExists.Name)
                {
                    vehicleMakeViewModel.ErrorMessage = "Name " + vehicleMakeViewModel.Name.ToString() + " already exists";
                    return View(vehicleMakeViewModel);
                }

                try
                {
                    var vehicle = _mapper.Map<VehicleMake>(vehicleMakeViewModel);
                    await _service.UpdateVehicleMakeAsync(vehicle);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _service.CheckVehicleMakeAsync(vehicleMakeViewModel.Id))
                        return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });
                    else
                        return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });
                }

                SweetAlert("Vehicle was successfully modified!", NotificationType.success);
                return View(vehicleMakeViewModel);
            }

            return View(vehicleMakeViewModel);
        }


        // method to get vehicle make to delete 
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            var result = await _service.GetVehicleMakeByIdAsync(id);

            if (result == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            var vehicle = _mapper.Map<VehicleMakeViewModel>(result);

            return View(vehicle);

        }

        // method to delete vehicle make
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteVehicleMakeAsync(id);

            var vehicle = new VehicleMakeViewModel { Name = "" };
            SweetAlert("Vehicle brand was successfully deleted!", NotificationType.success);
            return View(vehicle);
        }






    }
}