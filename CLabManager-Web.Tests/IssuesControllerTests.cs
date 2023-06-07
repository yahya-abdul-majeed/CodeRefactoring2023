using CLabManager_Web.Areas.Admin.Controllers;
using CLabManager_Web.Repos;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;
using ModelsLibrary.Models.ViewModels;
using ModelsLibrary.Utilities;
using Moq;
using NToastNotify;
using System.Security.Claims;

namespace CLabManager_Web.Tests
{
    public class IssuesControllerTests
    {
        [Fact]
        public async void Index_UnauthenticatedUser_ReturnsRedirectToActionResult()
        {
            //Arrange
            var mockToastNotif = new Mock<IToastNotification>();
            var SD = new SD();
            var issueRepoMock= new Mock<IIssuesRepo>();

            var controller = new IssuesController(mockToastNotif.Object, SD,issueRepoMock.Object);

            //Act
            var result = await controller.Index();

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
        [Fact]
        public async void Index_authenticatedAdminWithoutParams_ReturnsIssueIndexVM()
        {
            //Arrange
            var mockToasNotif = new Mock<IToastNotification>();
            var SDMock = new Mock<ISD>();
            SDMock.Setup(r => r.getPrincipal()).Returns(getDummyClaimsPrincipal());
            var issueRepoMock = new Mock<IIssuesRepo>();
            issueRepoMock.Setup(i => i.GetAllIssues()).Returns(getDummyIssues());
            var controller = new IssuesController(mockToasNotif.Object, SDMock.Object,issueRepoMock.Object);
            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IssueIndexVM>(viewResult.Model);
            Assert.Single(model.Issues);
        }
        [Fact]
        public async void Index_authenticatedAdminWithParams_ReturnsIssueIndexVM()
        {
            //Arrange
            var mockToasNotif = new Mock<IToastNotification>();
            var SDMock = new Mock<ISD>();
            SDMock.Setup(r => r.getPrincipal()).Returns(getDummyClaimsPrincipal());
            var issueRepoMock = new Mock<IIssuesRepo>();
            issueRepoMock.Setup(i => i.GetAllIssues()).Returns(getDummyIssues());
            var controller = new IssuesController(mockToasNotif.Object, SDMock.Object,issueRepoMock.Object);
            //Act
            var result = await controller.Index(4,5,"Urgent","Handled");

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IssueIndexVM>(viewResult.Model);
            var something = model.Issues.Count();
            Assert.Equal(1,model.Issues.Count());
        }
        [Fact]
        public async void IssueDetail_Admin_ReturnsIssueDetailVM()
        {
            
            var mockToasNotif = new Mock<IToastNotification>();
            var SDMock = new Mock<ISD>();
            SDMock.Setup(r => r.getPrincipal()).Returns(getDummyClaimsPrincipal());
            var issueRepoMock = new Mock<IIssuesRepo>();
            issueRepoMock.Setup(i => i.GetExactIssue(It.IsAny<int>())).Returns(getDummyIssue());
            var controller = new IssuesController(mockToasNotif.Object, SDMock.Object,issueRepoMock.Object);
            //Act
            var result = await controller.IssueDetail(5);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IssueDetailVM>(viewResult.Model);

        }
        private Task<List<Issue>> getDummyIssues()
        {
            var lab = new Lab
            {
                LabId = 81,
                BuildingNo = 5,
                RoomNo = 4
            };
            var issues = new List<Issue> { 
                new Issue
                {
                    Lab = lab,
                    LabId = lab.LabId,
                    Priority = IssuePriority.Urgent,
                    State = IssueState.Handled
                }
            };
            return Task.FromResult(issues);
        }
        private Task<Issue> getDummyIssue()
        {
            var lab = new Lab
            {
                LabId = 1,
                BuildingNo = 5,
                RoomNo = 4
            };
            var issue = new Issue { 
                    Lab = lab,
                    LabId = lab.LabId,
                    Priority = IssuePriority.Urgent,
                    State = IssueState.Handled
            };
            return Task.FromResult(issue);
        }

        private ClaimsPrincipal getDummyClaimsPrincipal()
        {
            var identity = new ClaimsIdentity("yahya");
            identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
            return new ClaimsPrincipal(identity);
        }
    }
}