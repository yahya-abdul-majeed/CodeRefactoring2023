using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models.DTO;
using ModelsLibrary.Utilities;
using Newtonsoft.Json;
using NToastNotify;

namespace CLabManager_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserRoleController : Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly ISD _sd;

        public UserRoleController(IToastNotification toastNotification, ISD sd)
        {
            _toastNotification = toastNotification;
            _sd = sd;
        }

        public async Task<IActionResult> Index()
        {
            if (_sd.getPrincipal().Identity == null)
                return RedirectToAction("AccessDenied", "Authentication", new { Area = "User" });
            if (_sd.getPrincipal().IsInRole("User"))
                return RedirectToAction("AccessDenied", "Authentication", new { Area = "User" });
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies[SD.XAccessToken]);
            using(var response = await httpClient.GetAsync("https://localhost:7138/api/User"))
            {
                var apiResponse = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserWithRoleDTO>>(apiResponse);
                return View(users);
            }
            
        }

        public async Task<IActionResult> UpdateRole(string role, string user)
        {
            var httpClient = new HttpClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(new RoleUpdateDTO
            {
                Role = role,
                UserId = user
            }));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", HttpContext.Request.Cookies[SD.XAccessToken]);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            using (var response = await httpClient.PutAsync("https://localhost:7138/api/User", content))
            {
                if (response.IsSuccessStatusCode)
                {
                    _toastNotification.AddSuccessToastMessage("Role Updated");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _toastNotification.AddErrorToastMessage("Role Update Failed");
                    return RedirectToAction(nameof(Index));

                }

            }
        }
    }
}
