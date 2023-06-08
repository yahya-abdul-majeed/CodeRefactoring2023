using CLabManager_Web.Areas.Admin.Controllers;
using ModelsLibrary.Models.DTO;
using CLabManager_Web.Repos;
using Microsoft.AspNetCore.Mvc;
using ModelsLibrary.Models;
using ModelsLibrary.Models.ViewModels;
using ModelsLibrary.Utilities;
using Moq;
using NToastNotify;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CLabManager_Web.Tests
{
    public class IssuesControllerTests
    {
        private readonly Mock<IToastNotification> _mockToastNotif;
        private readonly Mock<IIssuesRepo> _mockIssuesRepo;
        private readonly SD _sd;
        private readonly Mock<ISD> _isdMock;
        private readonly IssuesController _controller;

        public IssuesControllerTests()
        {
            _isdMock = new Mock<ISD>();
            _mockToastNotif = new Mock<IToastNotification>();
            _mockIssuesRepo = new Mock<IIssuesRepo>();
            _sd = new SD();
            _controller = new IssuesController(_mockToastNotif.Object, _isdMock.Object, _mockIssuesRepo.Object);
        }

        [Fact]
        public async void Index_UnauthenticatedUser_ReturnsRedirectToActionResult()
        {
            //Arrange
            var SD = new SD();
            var controller = new IssuesController(_mockToastNotif.Object, SD,_mockIssuesRepo.Object);

            //Act
            var result = await controller.Index();

            //Assert
            Assert.IsType<RedirectToActionResult>(result);
        }
        [Fact]
        public async void Index_authenticatedAdminWithoutParams_ReturnsIssueIndexVM()
        {
            //Arrange
            _isdMock.Setup(r => r.getPrincipal()).Returns(CommonMethods.getDummyClaimsPrincipal("Admin"));
            _mockIssuesRepo.Setup(i => i.GetAllIssues()).Returns(getDummyIssues());
            //Act
            var result = await _controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IssueIndexVM>(viewResult.Model);
            Assert.Single(model.Issues);
        }
        [Fact]
        public async void Index_authenticatedAdminWithParams_ReturnsIssueIndexVM()
        {
            //Arrange
            _isdMock.Setup(r => r.getPrincipal()).Returns(CommonMethods.getDummyClaimsPrincipal("Admin"));
            _mockIssuesRepo.Setup(i => i.GetAllIssues()).Returns(getDummyIssues());
            //Act
            var result = await _controller.Index(4,5,"Urgent","Handled");

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IssueIndexVM>(viewResult.Model);
            var something = model.Issues.Count();
            Assert.Equal(1,model.Issues.Count());
        }
        [Fact]
        public async void IssueDetail_Admin_ReturnsIssueDetailVM()
        {
            
            _isdMock.Setup(r => r.getPrincipal()).Returns(CommonMethods.getDummyClaimsPrincipal("Admin"));
            _mockIssuesRepo.Setup(i => i.GetExactIssue(It.IsAny<int>())).Returns(getDummyIssue());
            //Act
            var result = await _controller.IssueDetail(5);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IssueDetailVM>(viewResult.Model);

        }
        [Fact]
        public async void IssueUpdate_Admin_ReturnsRedirectToActionResult()
        {
            
            _isdMock.Setup(r => r.getPrincipal()).Returns(CommonMethods.getDummyClaimsPrincipal("Admin"));
            _mockIssuesRepo.Setup(i => i.UpdateIssue(It.IsAny<IssueUpdateDTO>())).Returns(CommonMethods.getDummyHttpResponseMessage());
            //Act
            var result = await _controller.IssueUpdate(new IssueUpdateDTO());

            //Assert
            var Result = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("IssueDetail",Result.ActionName);

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

    }
}