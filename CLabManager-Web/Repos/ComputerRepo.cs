using ModelsLibrary.Models;
using ModelsLibrary.Utilities;
using Newtonsoft.Json;

namespace CLabManager_Web.Repos
{
    public class ComputerRepo : IComputerRepo
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public ComputerRepo(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public async Task<List<Computer>>  GetUnassignedComputers()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext?.Request.Cookies[SD.XAccessToken]);
            string url = "https://localhost:7138/api/Computers/unassigned";
            using (var response = await httpClient.GetAsync(url))
            {
                var apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return JsonConvert.DeserializeObject<List<Computer>>(apiResponse);
            }
        }
    }
}
