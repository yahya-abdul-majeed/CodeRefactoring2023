﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsLibrary.Models;
using ModelsLibrary.Models.ViewModels;
using ModelsLibrary.Utilities;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace CLabManager_Web.Areas.User.Controllers
{
    [Area("User")]
    public class LabsController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;

        public LabsController(IHttpContextAccessor contextAccessor, IHttpClientFactory httpClientFactory)
        {
            _contextAccessor = contextAccessor;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(int buildingNo, int roomNo)
        {
            List<Lab> labs = new List<Lab>();
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies[SD.XAccessToken]);
            using (var response =await httpClient.GetAsync("https://localhost:7138/api/labs")) // for jwt, url should be https
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                labs = JsonConvert.DeserializeObject<List<Lab>>(apiResponse);
            }
            if (buildingNo != 0 && roomNo != 0)
            {
                labs = labs.Where(l => l.BuildingNo == buildingNo).ToList();
                labs = labs.Where(l => l.RoomNo == roomNo).ToList();
            }
            else if (roomNo == 0 && buildingNo != 0)
            {
                labs = labs.Where(l => l.BuildingNo == buildingNo).ToList();
            }
            else if (buildingNo == 0 && roomNo != 0)
            {
                labs = labs.Where(l => l.RoomNo == roomNo).ToList();
            }
            return View(labs);
        }

        public async Task<IActionResult> LabDetail(int id)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies[SD.XAccessToken]);
            LabDetailVM vm = new LabDetailVM();
            using(var response = await httpClient.GetAsync($"https://localhost:7138/api/Labs/{id}"))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                vm.Lab = JsonConvert.DeserializeObject<Lab>(apiResponse);
            }
            Array values = Enum.GetValues(typeof(IssuePriority));
            vm.priorities = new List<SelectListItem>();
            foreach(var i in values)
            {
                vm.priorities.Add(new SelectListItem
                {
                    Text = Enum.GetName(typeof(IssuePriority), i),
                    Value = i.ToString()
                });
            }
            return View(vm);
        }
        public void Redirecter(int buildingNo, int roomNo)
        {
            Response.Redirect($"https://localhost:7183/User/Labs?buildingNo={buildingNo}&roomNo={roomNo}");
        }

        public void Clearer()
        {
            Response.Redirect($"https://localhost:7183");
        }
    }

    
}
