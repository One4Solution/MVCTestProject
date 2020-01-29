using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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

        public VehicleMakeController(IVehicleService service, IMapper mapper, CarContext carContext)
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

        // method to list all vehicle makes
        [HttpGet]
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, string clearSearch, int? pageNumber, int pageSize = 4)
        {
            pageSize = _currentPageSize > 0 ? _currentPageSize : pageSize;
            ViewData["SlectionPageSize"] = _currentPageSize > 0 ? _currentPageSize : pageSize;

            var result = await _service.GetVehicleMakeAsync();

            var vehicles = _mapper.Map<List<VehicleMakeViewModel>>(result);

            var finalView = PaginationAndSorting(sortOrder, currentFilter, searchString, clearSearch, vehicles, pageNumber, pageSize);

            return View(finalView);
        }

        // help method to return list of VehicleMakeViewModel with pagination
        public Pagination<VehicleMakeViewModel> PaginationAndSorting(string sortOrder, string currentFilter, string searchString, string clearSearch, List<VehicleMakeViewModel> vehicles, int? pageNumber, int pageSize)
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
                vehicles = vehicles.Where(x => x.Name.ToLower().Contains(searchString.ToLower()) || x.Abbreviation.ToLower().Contains(searchString.ToLower())).ToList();


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

            return Pagination<VehicleMakeViewModel>.Create(vehicles, pageNumber ?? 1, pageSize);
        }




        // method to show page to create vehicle
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // method to create new vehicle
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id, Name, Abbreviation")] VehicleMakeViewModel vehicleMakeViewModel)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ErrorMessage = ""; // display message depends on name existing
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleMakeByNameAsync(vehicleMakeViewModel.Name);
                if (alreadyExists != null)
                {
                    ViewBag.ErrorMessage = "Name " + vehicleMakeViewModel.Name.ToString() + " already exists";
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
                ViewBag.ErrorMessage = "";
                // check if model name already exists in db for vehicle brand (make)
                var alreadyExists = await _service.GetVehicleMakeByNameAsync(vehicleMakeViewModel.Name);
                if (alreadyExists != null && _currentVehicleMakeName != alreadyExists.Name)
                {
                    ViewBag.ErrorMessage = "Name " + vehicleMakeViewModel.Name.ToString() + " already exists";
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