using ModelsLibrary.Models;

namespace CLabManager_Web.Repos
{
    public interface IComputerRepo
    {
        Task<List<Computer>> GetUnassignedComputers();
    }
}
