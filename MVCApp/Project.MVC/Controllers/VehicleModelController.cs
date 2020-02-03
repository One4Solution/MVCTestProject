using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.MVC.HelpClasses;
using Project.MVC.Interfaces;
using Project.MVC.Models;
using Project.Service.DataContext;
using Project.Service.Interfaces;
using Project.Service.Models;
using static Project.MVC.HelpClasses.NotificationEnum;

namespace Project.MVC.Controllers
{
    public class VehicleModelController : VehicleBaseController
    {
        private static string _currentVehicleModelName;
        private static int _currentPageSize = 0;

        public VehicleModelController(IVehicleService service, IMapper mapper, SearchSortExClass searchSortParams) : base(service, mapper, searchSortParams) { }

        public IActionResult SetPageSize(int pageSize = 4)
        {
            _currentPageSize = pageSize;
            _searchSortParams.SlectionPageSize = _currentPageSize;
            return RedirectToAction("Index");
        }

        // method to list all vehicle models
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string clearSearch, int pageNumber = 1, int pageSize = 4)
        {
            var result = await _service.GetVehicleModelAsync();
            var vehicles = _mapper.Map<List<VehicleModelViewModel>>(result);
            if (vehicles == null)
                vehicles = new List<VehicleModelViewModel>();
            var paggedVehicles = VehiclePaginationAndSorting(sortOrder, searchString, clearSearch, pageNumber, pageSize, vehicles);

            return View(paggedVehicles);
        }

        // method to return pagged vehicle models
        public (PagedResult<VehicleModelViewModel> vehicleMake, SearchSortExClass param) VehiclePaginationAndSorting(string sortOrder, string searchString, string clearSearch, int pageNumber, int pageSize, List<VehicleModelViewModel> vehicleModels)
        {
            // define pageSize based on dropdown
            pageSize = _currentPageSize > 0 ? _currentPageSize : pageSize;

            // save parameters
            _searchSortParams.CurrentSortOrder = sortOrder;
            _searchSortParams.SearchFilter = searchString;
            _searchSortParams.BrandSortParm = String.IsNullOrEmpty(sortOrder) ? "brand_desc" : "";
            _searchSortParams.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            _searchSortParams.AbrvSortParm = String.IsNullOrEmpty(sortOrder) ? "abrv_desc" : "";
            _searchSortParams.SlectionPageSize = _currentPageSize > 0 ? _currentPageSize : pageSize;

            var excludeVehicles = (pageSize * pageNumber) - pageSize; // which data will not be included
            var totalVehicles = vehicleModels.Count();

            if (!String.IsNullOrEmpty(clearSearch))
            {
                searchString = null;
                _searchSortParams.SearchFilter = null;
                // ViewData["SearchFilter"] = null;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                vehicleModels = vehicleModels.Where(x => x.Name.ToLower().Contains(searchString.ToLower()) || x.Abbreviation.ToLower().Contains(searchString.ToLower())
                 || x.VehicleMake.Name.ToLower().Contains(searchString.ToLower())).ToList();
                totalVehicles = vehicleModels.Count();
            }


            switch (sortOrder)
            {
                case "name_desc":
                    vehicleModels = vehicleModels.OrderByDescending(x => x.Name).ToList();
                    break;

                case "abrv_desc":
                    vehicleModels = vehicleModels.OrderByDescending(x => x.Abbreviation).ToList();
                    break;

                case "brand_desc":
                    vehicleModels = vehicleModels.OrderByDescending(x => x.VehicleMake.Name).ToList();
                    break;

                default:
                    vehicleModels = vehicleModels.OrderBy(x => x.Name).ToList();
                    break;
            }


            var pagedVehicles = vehicleModels.Skip(excludeVehicles).Take(pageSize).ToList();

            var vehicleResult = new PagedResult<VehicleModelViewModel>
            {
                Data = pagedVehicles,
                TotalItems = totalVehicles,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return (vehicleResult, _searchSortParams);
        }


        // method to show page to create vehicle
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vehicleMake = await _service.GetVehicleMakeAsync();
            var vehicleModelViewModel = new VehicleModelViewModel();
            vehicleModelViewModel.vehicleMakeModels = _mapper.Map<List<VehicleMakeViewModel>>(vehicleMake.OrderBy(x => x.Name));

            return View(vehicleModelViewModel);
        }

        // method to create new vehicle
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name, Abbreviation, VehicleMakeId")] VehicleModelViewModel vehicleModelViewModel)
        {
            if (ModelState.IsValid)
            {
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleModelByNameAsync(vehicleModelViewModel.Name);
                if (alreadyExists != null)
                {
                    vehicleModelViewModel.ErrorMessage = "Name " + vehicleModelViewModel.Name.ToString() + " already exists";
                    var vehicleMake = await _service.GetVehicleMakeAsync();
                    vehicleModelViewModel.vehicleMakeModels = _mapper.Map<List<VehicleMakeViewModel>>(vehicleMake);
                    return View(vehicleModelViewModel);
                }

                var vehicle = _mapper.Map<VehicleModel>(vehicleModelViewModel);
                await _service.CreateVehicleModelAsync(vehicle);  

                ModelState.Clear(); // clear form inputs after successfully creating

                // get vehicle makes to return in view (dropdown selection)
                var vehicleMakes = await _service.GetVehicleMakeAsync();
                var makeModels = _mapper.Map<List<VehicleMakeViewModel>>(vehicleMakes.OrderBy(x => x.Name));

                var vehicleModel = new VehicleModelViewModel { vehicleMakeModels = makeModels };
     
                SweetAlert("Vehicle make was created!", NotificationType.success);
                Response.StatusCode = (int)HttpStatusCode.Created;
                return View(vehicleModel);
            }

            return View(vehicleModelViewModel);
        }




        // method to show edit page
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            var result = await _service.GetVehicleModelByIdAsync(id);
            var vehicle = _mapper.Map<VehicleModelViewModel>(result);

            if (vehicle == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            _currentVehicleModelName = vehicle.Name;

            return View(vehicle);
        }


        // method to edit selected vehicle
        [HttpPost]
        public async Task<IActionResult> Edit(int? id, [Bind("Id, Name, Abbreviation, VehicleMakeId")] VehicleModelViewModel vehicleModelViewModel)
        {
            if (id != vehicleModelViewModel.Id || id == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            if (ModelState.IsValid)
            {
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleModelByNameAsync(vehicleModelViewModel.Name);
                if (alreadyExists != null && _currentVehicleModelName != alreadyExists.Name)
                {
                    vehicleModelViewModel.ErrorMessage = "Name " + vehicleModelViewModel.Name.ToString() + " already exists";
                    return View(vehicleModelViewModel);
                }

                try
                {
                    var vehicle = _mapper.Map<VehicleModel>(vehicleModelViewModel);
                    await _service.UpdateVehicleModelAsync(vehicle);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _service.CheckVehicleModelAsync(vehicleModelViewModel.Id))
                        return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });
                    else
                        return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });
                }

                SweetAlert("Vehicle was successfully modified!", NotificationType.success);
                return View(vehicleModelViewModel);
            }

            return View(vehicleModelViewModel);
        }




        // method to get vehicle model to delete 
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            var result = await _service.GetVehicleModelByIdAsync(id);

            if (result == null)
                return RedirectToAction("ErrorResponse", "Error", new { statusCode = 404 });

            var vehicle = _mapper.Map<VehicleModelViewModel>(result);

            return View(vehicle);

        }

        // method to delete vehicle model
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteVehicleModelAsync(id);

            var vehicle = new VehicleModelViewModel { Name = ""};
            SweetAlert("Vehicle model was successfully deleted!", NotificationType.success);
            return View(vehicle);
        }





    }
}