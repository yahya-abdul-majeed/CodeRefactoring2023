using ModelsLibrary.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net;
using ModelsLibrary.Utilities;
using System.Text;

namespace CLabManager_Web.Repos
{
    public class LabRepo : ILabRepo
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public LabRepo(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public async Task<HttpResponseMessage> DeleteLab(int? LabId)
        {
            using(var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext.Request.Cookies[SD.XAccessToken]);

                using (var response = await httpClient.DeleteAsync($"https://localhost:7138/api/Labs/{LabId}"))
                {
                    return response;
                }
            }
        }

        public async Task<List<SelectListItem>>  GetAllLabs()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext?.Request.Cookies[SD.XAccessToken]);
            string url = "https://localhost:7138/api/Labs";
            var vmLabs = new List<SelectListItem>();
            using (var response = await httpClient.GetAsync(url))
            {
                var apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var labs = JsonConvert.DeserializeObject<List<Lab>>(apiResponse);
                labs = labs.OrderBy(p => p.BuildingNo).ThenBy(p => p.RoomNo).ToList();
                foreach (var i in labs)
                {
                    vmLabs.Add(new SelectListItem
                    {
                        Text = "Building " + i.BuildingNo.ToString() + " Room " + i.RoomNo.ToString(),
                        Value = i.LabId.ToString()
                    });
                }
            }
            return vmLabs;
        }

        public async Task<Lab?> GetExactLab(int? LabId)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext?.Request.Cookies[SD.XAccessToken]);
            string url = $"https://localhost:7138/api/Labs/{LabId}";
            if (LabId != null)
            {
                using (var response = await httpClient.GetAsync(url))
                {
                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        var res = await response.Content.ReadAsStringAsync();
                        var lab = JsonConvert.DeserializeObject<Lab>(res);
                        return lab;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        public async Task<HttpResponseMessage> PostLab(object obj)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            var url = "https://localhost:7138/api/Labs";
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext.Request.Cookies[SD.XAccessToken]);
                return await httpClient.PostAsync(url, content);
            }
        }
    }
}
