using ModelsLibrary.Models;

namespace CLabManager_Web.Repos
{
    public interface IIssuesRepo
    {
        public Task<List<Issue>> GetAllIssues();
        public Task<Issue > GetExactIssue(int id);
        public HttpResponseMessage UpdateIssue(int id);
    }
}
