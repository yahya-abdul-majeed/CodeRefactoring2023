using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsLibrary.Models;

namespace CLabManager_Web.Repos
{
    public interface ILabRepo
    {
        Task<Lab?> GetExactLab(int? LabId);
        Task<List<SelectListItem>> GetAllLabs();
        Task<HttpResponseMessage> PostLab(object obj);
        Task<HttpResponseMessage> DeleteLab(int? LabId);
    }
}
