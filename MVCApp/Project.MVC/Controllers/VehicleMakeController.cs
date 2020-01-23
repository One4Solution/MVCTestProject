﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.MVC.Models;
using Project.Service.Interfaces;
using Project.Service.Models;
using Project.Service.Services;

namespace Project.MVC.Controllers
{
    public class VehicleMakeController : Controller
    {
        private readonly IVehicleService _service;
        private readonly IMapper _mapper;

        public VehicleMakeController(IVehicleService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        // method to list all vehicles
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var result = await _service.GetVehicleMakeAsync();
            var vehicles = _mapper.Map<List<VehicleMakeViewModel>>(result);

            return View(vehicles);
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
                var vehicle = _mapper.Map<VehicleMake>(vehicleMakeViewModel);
                await _service.CreateVehicleMakeAsync(vehicle);
                return RedirectToAction("Index");
            }
            return View(vehicleMakeViewModel);

        }



        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var result = await _service.GetVehicleMakeByIdAsync(id);
            var vehicle = _mapper.Map<VehicleMakeViewModel>(result);

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