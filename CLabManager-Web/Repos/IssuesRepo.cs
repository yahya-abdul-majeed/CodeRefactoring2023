using ModelsLibrary.Models;
using ModelsLibrary.Models.DTO;
using ModelsLibrary.Utilities;
using Newtonsoft.Json;

namespace CLabManager_Web.Repos
{
    public class IssuesRepo : IIssuesRepo
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IssuesRepo(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public async Task<List<Issue>> GetAllIssues()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext?.Request.Cookies[SD.XAccessToken]);
                using (var response = await httpClient.GetAsync("https://localhost:7138/api/Issues"))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                     return JsonConvert.DeserializeObject<List<Issue>>(apiResponse);   
                }
            }
        }

        public async Task<Issue> GetExactIssue(int id)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext?.Request.Cookies[SD.XAccessToken]);
            using(var response = await httpClient.GetAsync($"https://localhost:7138/api/Issues/{id}"))
            {
                var apiResponse = response.Content.ReadAsStringAsync().GetAwaiter().GetResult(); 
                return JsonConvert.DeserializeObject<Issue>(apiResponse);
            }
        }

        public async Task<HttpResponseMessage > UpdateIssue(IssueUpdateDTO dto)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(dto));
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _contextAccessor.HttpContext?.Request.Cookies[SD.XAccessToken]);
            return await httpClient.PutAsync($"https://localhost:7138/api/issues/{dto.IssueId}", content);
        }
    }
}
