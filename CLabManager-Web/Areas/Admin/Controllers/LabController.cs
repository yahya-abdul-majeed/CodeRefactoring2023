using CLabManager_Web.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsLibrary.Models;
using ModelsLibrary.Models.DTO;
using ModelsLibrary.Models.ViewModels;
using ModelsLibrary.Utilities;
using Newtonsoft.Json;
using NToastNotify;
using System.Net;
using System.Text;

namespace CLabManager_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LabController : Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly ISD _sd;
        private readonly ILabRepo _labRepo;
        private readonly IComputerRepo _computerRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        public LabController(IToastNotification toastNotification, ISD sd, IComputerRepo computerRepo,ILabRepo labRepo,IHttpContextAccessor contextAccessor)
        {
            _toastNotification = toastNotification;
            _sd = sd;
            _computerRepo = computerRepo;
            _labRepo = labRepo;
            _contextAccessor = contextAccessor;
        }
        public void Redirector(int? LabId)
        {
            Response.Redirect($"https://localhost:7183/Admin/Lab/Create?LabId={LabId}");
        }
        public async Task<IActionResult> Create(int? LabId)
        {
            if (_sd.getPrincipal().Identity == null || _sd.getPrincipal().IsInRole("User") )
                return RedirectToAction("AccessDenied", "Authentication", new { Area = "User" });
            CreateLabVM vm = new CreateLabVM();
            vm.Lab = await _labRepo.GetExactLab(LabId);
            vm.UnassignedComputers = await _computerRepo.GetUnassignedComputers();
            //lab select
            vm.Labs = await _labRepo.GetAllLabs();
            //gridType select
            Array values = Enum.GetValues(typeof(GridType));
            vm.items = new List<SelectListItem>();
            foreach (var i in values)
            {
                vm.items.Add(new SelectListItem
                {
                    Text = Enum.GetName(typeof(GridType), i),
                    Value = i.ToString()
                });
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLab(LabCreationDTO dto)
        {
            if (_sd.getPrincipal().IsInRole("User"))
                return RedirectToAction("AccessDenied", "Authentication", new { Area = "User" });
            Lab lab = new Lab();
            var requestData = new
            {
                roomNo = dto.RoomNo,
                buildingNo = dto.BuildingNo,
                gridType = dto.GridType,
                status = dto.Status
            };
            var response = await _labRepo.PostLab(requestData);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                var apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                lab = JsonConvert.DeserializeObject<Lab>(apiResponse);
                _toastNotification.AddSuccessToastMessage("Lab Created");
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Lab Creation failed");
                return RedirectToAction("Create");
            }
            return RedirectToAction("Create", new { LabId = lab.LabId });
        }

        public async Task<IActionResult> DeleteLab(int? LabId)
        {
            if (LabId == 0 || LabId ==null)
            {
                return RedirectToAction(nameof(Create), new { LabId = (int?)null });
            }
            var response = await _labRepo.DeleteLab(LabId);
            if(response.IsSuccessStatusCode)
            {
                _toastNotification.AddSuccessToastMessage("Lab deleted");
                return RedirectToAction(nameof(Create), new {LabId = (int?)null});
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Lab delete failed");
                
                return Redirect(_contextAccessor.HttpContext.Request.Headers["Referer"].ToString());
            }
        }

        public async Task<IActionResult> DeleteComputer(int compId)
        {
            using(var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers
                    .AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies[SD.XAccessToken]);
                using(var response = await httpClient.DeleteAsync($"https://localhost:7138/api/computers/{compId}"))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        _toastNotification.AddErrorToastMessage("Delete failed");
                    }
                    return Redirect(_contextAccessor.HttpContext.Request.Headers["Referer"].ToString());
                }
            }
        }
    }
}
