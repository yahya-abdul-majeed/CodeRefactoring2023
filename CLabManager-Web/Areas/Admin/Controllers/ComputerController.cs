using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Utilities;
using NToastNotify;

namespace CLabManager_Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ComputerController : Controller
    {
        private readonly IToastNotification _toastNotification;
        private readonly IHttpContextAccessor _contextAccessor;
        public ComputerController(IToastNotification toastNotification, IHttpContextAccessor contextAccessor)
        {
            _toastNotification = toastNotification;
            _contextAccessor = contextAccessor;

        }
        [HttpPost]
        public async Task<IActionResult> DeleteComputer(int compId)
        {
            using(var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers
                    .AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext.Request.Cookies[SD.XAccessToken]);
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
