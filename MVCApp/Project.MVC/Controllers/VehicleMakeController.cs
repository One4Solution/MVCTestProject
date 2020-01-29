using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using cloudscribe.Pagination.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project.MVC.HelpClasses;
using Project.MVC.Models;
using Project.Service.DataContext;
using Project.Service.Interfaces;
using Project.Service.Models;
using Project.Service.Services;

namespace Project.MVC.Controllers
{
    public class VehicleMakeController : Controller
    {
        private readonly IVehicleService _service;
        private readonly IMapper _mapper;
        private static string _currentVehicleMakeName;
        private static int _currentPageSize = 0;
        SearchSortExClass searchSortParams = new SearchSortExClass();

        public VehicleMakeController(IVehicleService service, IMapper mapper, CarContext carContext)
        {
            _service = service;
            _mapper = mapper;
        }

        // method to set page size based on dropdown selection in the view
        public IActionResult SetPageSize(int pageSize = 4)
        {
            _currentPageSize = pageSize;
            searchSortParams.SlectionPageSize = _currentPageSize;
            return RedirectToAction("Index");
        }


        // method to list all vehicle makes
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string searchString, string clearSearch, int pageNumber = 1, int pageSize = 4)
        {
            var result = await _service.GetVehicleMakeAsync();
            var vehicles = _mapper.Map<List<VehicleMakeViewModel>>(result);
            var paggedVehicles = VehiclePaginationAndSorting(sortOrder, searchString, clearSearch, pageNumber, pageSize, vehicles);

            return View(paggedVehicles);
        }

        // method to return pagged vehicle makes
        public (PagedResult<VehicleMakeViewModel> vehicleMake, SearchSortExClass param) VehiclePaginationAndSorting(string sortOrder, string searchString, string clearSearch, int pageNumber, int pageSize, List<VehicleMakeViewModel> vehicleMakes)
        {
            // define pageSize based on dropdown
            pageSize = _currentPageSize > 0 ? _currentPageSize : pageSize;

            // save parameters
            searchSortParams.CurrentSortOrder = sortOrder;
            searchSortParams.SearchFilter = searchString;
            searchSortParams.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            searchSortParams.AbrvSortParm = String.IsNullOrEmpty(sortOrder) ? "abrv_desc" : "";
            searchSortParams.SlectionPageSize = _currentPageSize > 0 ? _currentPageSize : pageSize;

            var excludeRecords = (pageSize * pageNumber) - pageSize; // which data will not be included
            var totalVehicles = vehicleMakes.Count();


            // clear serach input
            if (!String.IsNullOrEmpty(clearSearch))
            {
                searchString = null;
                searchSortParams.SearchFilter = null;
            }

            // check if search field is not empty
            if (!String.IsNullOrEmpty(searchString))
            {
                vehicleMakes = vehicleMakes.Where(x => x.Name.ToLower().Contains(searchString.ToLower())
               || x.Abbreviation.ToLower().Contains(searchString.ToLower())).ToList();
                totalVehicles = vehicleMakes.Count();
            }

            // sorting
            switch (sortOrder)
            {
                case "name_desc":
                    vehicleMakes = vehicleMakes.OrderByDescending(x => x.Name).ToList();
                    break;

                case "abrv_desc":
                    vehicleMakes = vehicleMakes.OrderByDescending(x => x.Abbreviation).ToList();
                    break;

                default:
                    vehicleMakes = vehicleMakes.OrderBy(x => x.Name).ToList();
                    break;
            }

            var pagedVehicles = vehicleMakes.Skip(excludeRecords).Take(pageSize).ToList();
            
            var vehicleResult = new PagedResult<VehicleMakeViewModel>
            {
                Data = pagedVehicles,
                TotalItems = totalVehicles,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            
            return (vehicleResult, searchSortParams);
        }


        // method to show page to create vehicle
        [HttpGet]
        public IActionResult Create()
        {
            var vehicleMakeModel = new VehicleMakeViewModel { ErrorMessage = "" };
            return View(vehicleMakeModel);
        }

        // method to create new vehicle
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Name, Abbreviation")] VehicleMakeViewModel vehicleMakeViewModel)
        {
            if (ModelState.IsValid)
            {
                vehicleMakeViewModel.ErrorMessage = ""; // display message depends on name existing
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleMakeByNameAsync(vehicleMakeViewModel.Name);
                if (alreadyExists != null)
                {
                    vehicleMakeViewModel.ErrorMessage = "Name " + vehicleMakeViewModel.Name.ToString() + " already exists";
                    return View(vehicleMakeViewModel);
                }

                var vehicle = _mapper.Map<VehicleMake>(vehicleMakeViewModel);
                await _service.CreateVehicleMakeAsync(vehicle);
                return RedirectToAction("Index");
            }
            return View(vehicleMakeViewModel);

        }


        // method to show edit page
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var result = await _service.GetVehicleMakeByIdAsync(id);
            var vehicle = _mapper.Map<VehicleMakeViewModel>(result);

            _currentVehicleMakeName = vehicle.Name; // save current vehicle name on edit 

            return View(vehicle);
        }


        // method to edit selected vehicle
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Abbreviation")] VehicleMakeViewModel vehicleMakeViewModel)
        {
            if (id != vehicleMakeViewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                vehicleMakeViewModel.ErrorMessage = "";
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
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction("Index");
            }

            return View(vehicleMakeViewModel);
        }


        // method to get vehicle make to delete 
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var result = await _service.GetVehicleMakeByIdAsync(id);

            if (result == null)
                return NotFound();

            var vehicle = _mapper.Map<VehicleMakeViewModel>(result);

            return View(vehicle);

        }

        // method to delete vehicle make
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteVehicleMakeAsync(id);
            return RedirectToAction("Index");

        }






    }
}