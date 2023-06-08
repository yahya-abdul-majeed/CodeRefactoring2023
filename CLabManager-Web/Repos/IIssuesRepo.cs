using ModelsLibrary.Models;
using ModelsLibrary.Models.DTO;

namespace CLabManager_Web.Repos
{
    public interface IIssuesRepo
    {
        public Task<List<Issue>> GetAllIssues();
        public Task<Issue > GetExactIssue(int id);
        public Task<HttpResponseMessage > UpdateIssue(IssueUpdateDTO dto);
    }
}
