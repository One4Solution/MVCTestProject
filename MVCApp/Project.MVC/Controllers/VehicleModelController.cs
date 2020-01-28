using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.MVC.HelpClasses;
using Project.MVC.Models;
using Project.Service.DataContext;
using Project.Service.Interfaces;
using Project.Service.Models;

namespace Project.MVC.Controllers
{
    public class VehicleModelController : Controller
    {
        private readonly IVehicleService _service;
        private readonly IMapper _mapper;
        private static string _currentVehicleModelName;
        private static int _currentPageSize = 0;

        // public VehicleMakeViewModel VehicleMakeSelected { get; set; }

        public VehicleModelController(IVehicleService service, IMapper mapper )
        {
            _service = service;
            _mapper = mapper;
        }

        public IActionResult SetPageSize(int pageSize = 4)
        {
            _currentPageSize = pageSize;
            ViewData["SlectionPageSize"] = _currentPageSize;
            return RedirectToAction("Index");
        }


        // method to list all vehicle models
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, string clearSearch, int? pageNumber, int pageSize = 4)
        {
            pageSize = _currentPageSize > 0 ? _currentPageSize : pageSize;
            ViewData["SlectionPageSize"] = _currentPageSize > 0 ? _currentPageSize : pageSize;

            var result = await _service.GetVehicleModelAsync();

            var vehicles = _mapper.Map<List<VehicleModelViewModel>>(result);

            var finalView = PaginationAndSorting(sortOrder, currentFilter, searchString, clearSearch, vehicles, pageNumber, pageSize);

            return View(finalView);
        }


        // help method to return list of VehicleModelViewModel with pagination
        public Pagination<VehicleModelViewModel> PaginationAndSorting(string sortOrder, string currentFilter, string searchString, string clearSearch, List<VehicleModelViewModel> vehicles, int? pageNumber, int pageSize)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AbrvSortParm"] = String.IsNullOrEmpty(sortOrder) ? "abrv_desc" : "";

            if (searchString != null)
                pageNumber = 1;
            else
                searchString = currentFilter;

            ViewData["SearchFilter"] = searchString;

            if (!String.IsNullOrEmpty(clearSearch))
            {
                searchString = null;
                ViewData["SearchFilter"] = null;
            }

            if (!String.IsNullOrEmpty(searchString))
                vehicles = vehicles.Where(x => x.Name.ToLower().Contains(searchString.ToLower()) || x.Abbreviation.ToLower().Contains(searchString.ToLower()) 
                || x.VehicleMake.Name.ToLower().Contains(searchString.ToLower())).ToList();


            switch (sortOrder)
            {
                case "name_desc":
                    vehicles = vehicles.OrderByDescending(x => x.Name).ToList();
                    break;

                case "abrv_desc":
                    vehicles = vehicles.OrderByDescending(x => x.Abbreviation).ToList();
                    break;

                default:
                    vehicles = vehicles.OrderBy(x => x.Name).ToList();
                    break;
            }

            return Pagination<VehicleModelViewModel>.Create(vehicles, pageNumber ?? 1, pageSize);
        }



        // method to show page to create vehicle
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vehicleMake = await _service.GetVehicleMakeAsync();
            var vehicleModelViewModel = new VehicleModelViewModel();
            vehicleModelViewModel.vehicleMakeModels = _mapper.Map<List<VehicleMakeViewModel>>(vehicleMake);

            return View(vehicleModelViewModel);
        }

        // method to create new vehicle
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Name, Abbreviation, VehicleMakeId")] VehicleModelViewModel vehicleModelViewModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ErrorMessage = ""; // display message depends on name existing
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleModelByNameAsync(vehicleModelViewModel.Name);
                if (alreadyExists != null)
                {
                    ViewBag.ErrorMessage = "Name " + vehicleModelViewModel.Name.ToString() + " already exists";
                    var vehicleMake = await _service.GetVehicleMakeAsync();
                    vehicleModelViewModel.vehicleMakeModels = _mapper.Map<List<VehicleMakeViewModel>>(vehicleMake);
                    return View(vehicleModelViewModel);
                }

                var vehicle = _mapper.Map<VehicleModel>(vehicleModelViewModel);
                await _service.CreateVehicleModelAsync(vehicle);

                return RedirectToAction("Index");
            }
            return View(vehicleModelViewModel);
        }




        // method to show edit page
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var result = await _service.GetVehicleModelByIdAsync(id);
            var vehicle = _mapper.Map<VehicleModelViewModel>(result);

            _currentVehicleModelName = vehicle.Name;

            return View(vehicle);
        }


        // method to edit selected vehicle
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Abbreviation, VehicleMakeId")] VehicleModelViewModel vehicleModelViewModel)
        {
            if (id != vehicleModelViewModel.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "";
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleModelByNameAsync(vehicleModelViewModel.Name);
                if (alreadyExists != null && _currentVehicleModelName != alreadyExists.Name)
                {
                    ViewBag.ErrorMessage = "Name " + vehicleModelViewModel.Name.ToString() + " already exists";
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
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction("Index");
            }

            return View(vehicleModelViewModel);
        }




        // method to get vehicle model to delete 
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var result = await _service.GetVehicleModelByIdAsync(id);

            if (result == null)
                return NotFound();

            var vehicle = _mapper.Map<VehicleModelViewModel>(result);

            return View(vehicle);

        }

        // method to delete vehicle model
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteVehicleModelAsync(id);
            return RedirectToAction("Index");
        }





    }
}