using CLabManager_Web.Repos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsLibrary.Models;
using ModelsLibrary.Models.DTO;
using ModelsLibrary.Models.ViewModels;
using ModelsLibrary.Utilities;
using Newtonsoft.Json;
using NToastNotify;

namespace CLabManager_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class IssuesController : Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly ISD _sd;
        private readonly IIssuesRepo _issueRepo;
        public IssuesController(IToastNotification toastNotification, ISD sd, IIssuesRepo issueRepo)
        {
            _issueRepo = issueRepo;
            _toastNotification = toastNotification;
            _sd = sd;
        }
        public async Task<IActionResult> Index(IndexParamObject parameters)
        {
            if (_sd.getPrincipal().Identity == null || _sd.getPrincipal().IsInRole("User") )
                return RedirectToAction("AccessDenied", "Authentication", new { Area = "User" });
            IssueIndexVM vm = new IssueIndexVM();
            vm.Issues = await _issueRepo.GetAllIssues();
            vm.Issues = configureVMIssues(parameters, vm.Issues);
            vm.Items = configureVMItems();
            vm.PItems = configureVMPItems(); 
            return View(vm);
        }
        public List<Issue> configureVMIssues(IndexParamObject parameters,List<Issue> issueList)
        {
            var issues = issueList;
            if (parameters.buildingNo != 0 && parameters.roomNo != 0)
            {
                issues = issues.Where(l => l.Lab.BuildingNo == parameters.buildingNo).ToList();
                issues  = issues.Where(l => l.Lab.RoomNo == parameters.roomNo).ToList();
            }
            else if (parameters.roomNo == 0 && parameters.buildingNo != 0)
            {
                issues = issues.Where(l => l.Lab.BuildingNo == parameters.buildingNo).ToList();
            }
            else if (parameters.buildingNo == 0 && parameters.roomNo != 0)
            {
                issues = issues.Where(l => l.Lab.RoomNo == parameters.roomNo).ToList();
            }
            //checking for state and priority
            if(parameters.priority != "All" && parameters.priority != null)
            {
                issues = issues.Where(l=>l.Priority.ToString() ==parameters.priority).ToList();  
            }
            if(parameters.state != "All" && parameters.state != null)
            {
                issues = issues.Where(l=>l.State.ToString() == parameters.state).ToList();
            }
            return issues;

        }
        public List<SelectListItem> configureVMItems()
        {
            Array values = Enum.GetValues(typeof(IssueState));
            var Items = new List<SelectListItem>();
            foreach (var i in values)
            {
                Items.Add(new SelectListItem
                {
                    Text = Enum.GetName(typeof(IssueState), i),
                    Value = i.ToString()
                });
            }
            return Items;
        }
        public List<SelectListItem> configureVMPItems()
        {
            Array values2 = Enum.GetValues(typeof(IssuePriority));
            var PItems = new List<SelectListItem>();
            foreach(var i in values2)
            {
                PItems.Add(new SelectListItem
                {
                    Text = Enum.GetName(typeof(IssuePriority), i),
                    Value = i.ToString()
                });
            }
            return PItems;
        }

        public void Redirecter(int? roomNo = 0, int? buildingNo = 0, string? priority = null, string? state = null)
        {
            Response.Redirect($"https://localhost:7183/Admin/Issues?roomNo={roomNo}&buildingNo={buildingNo}&priority={priority}&state={state}");
        }
        public void Clearer()
        {
            Response.Redirect($"https://localhost:7183/Admin/Issues");
        }

        public async Task<IActionResult> IssueDetail(int id)    
        {
            if (_sd.getPrincipal().IsInRole("User"))
                return RedirectToAction("AccessDenied", "Authentication", new { Area = "User" });
            IssueDetailVM vm = new IssueDetailVM();
            vm.Issue = await _issueRepo.GetExactIssue(id);
            vm.Items = configureVMItems();
            vm.PItems = configureVMPItems(); 
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> IssueUpdate(IssueUpdateDTO dto)
        {
            var response = await _issueRepo.UpdateIssue(dto);
            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                _toastNotification.AddSuccessToastMessage("Issue Updated");
            }
            else
            {
                _toastNotification.AddErrorToastMessage("Issue Update Failed");
            }

            var r = RedirectToAction("IssueDetail", new { id = dto.IssueId});
            return r;
        }
    }
}
