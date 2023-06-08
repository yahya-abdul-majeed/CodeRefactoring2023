using CLabManager_Web.Areas.Admin.Controllers;
using CLabManager_Web.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ModelsLibrary.Models;
using ModelsLibrary.Models.DTO;
using ModelsLibrary.Models.ViewModels;
using ModelsLibrary.Utilities;
using Moq;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLabManager_Web.Tests
{
    public class LabControllerTests
    {
        private readonly Mock<IToastNotification> _mockToastNotif;
        private readonly Mock<ISD> _isdMock;
        private readonly Mock<ILabRepo> _mockLabRepo;
        private readonly Mock<IComputerRepo> _mockComputerRepo;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly LabController _controller;
        public LabControllerTests()
        {
            _mockToastNotif = new Mock<IToastNotification>();
            _isdMock = new Mock<ISD>();
            _mockLabRepo = new Mock<ILabRepo>();
            _mockComputerRepo = new Mock<IComputerRepo>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _controller = new LabController(_mockToastNotif.Object, _isdMock.Object, _mockComputerRepo.Object, _mockLabRepo.Object,_mockHttpContextAccessor.Object);
        }
        [Fact]
        public async void Create_Unauthorized_ReturnsRedirectToActionResult()
        {

            _isdMock.Setup(r => r.getPrincipal()).Returns(CommonMethods.getDummyClaimsPrincipal("User"));
            var result = await _controller.Create(null);
            Assert.IsAssignableFrom<RedirectToActionResult>(result);

        }
        [Fact]
        public async void Create_authorized_ReturnsCreateLabVM()
        {

            _isdMock.Setup(r => r.getPrincipal()).Returns(CommonMethods.getDummyClaimsPrincipal("Admin"));
            _mockLabRepo.Setup(r => r.GetExactLab(It.IsAny<int>())).Returns(GetDummyLab());
            _mockLabRepo.Setup(r=>r.GetAllLabs()).Returns(GetDummySelectListItems());
            _mockComputerRepo.Setup(r => r.GetUnassignedComputers()).Returns(GetDummyComputerList());
            var result = await _controller.Create(It.IsAny<int>());
            var viewResult = Assert.IsAssignableFrom<ViewResult>(result);
            var model = Assert.IsAssignableFrom<CreateLabVM>(viewResult.Model);
            Assert.Equal(1,model.Labs.Count);
            Assert.Equal(1, model.UnassignedComputers.Count);
            Assert.IsType<Lab>(model.Lab as Lab);

        }
        [Fact]
        public async void CreateLab_authorized_ReturnsLabId45()  
        {
            _mockLabRepo.Setup(r=>r.PostLab(It.IsAny<object>())).Returns(CommonMethods.getDummyHttpResponseMessage());
            _isdMock.Setup(r => r.getPrincipal()).Returns(CommonMethods.getDummyClaimsPrincipal("Admin"));
            var obj = new LabCreationDTO
            {
                RoomNo = 1,
                BuildingNo = 2,
                GridType = GridType.type1,
                Status = LabStatus.Vacant
            };
            var result =await _controller.CreateLab(obj);
            var ActionResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
            Assert.Equal(ActionResult.RouteValues?.Values.ElementAt(0),45);
        }
        [Fact]
        public async void CreateLab_authorized_ReturnsRedirectToActionResult_WithNullRouteValues()  
        {
            _mockLabRepo.Setup(r=>r.PostLab(It.IsAny<object>())).Returns(CommonMethods.getDummyHttpResponseMessageWithError());
            _isdMock.Setup(r => r.getPrincipal()).Returns(CommonMethods.getDummyClaimsPrincipal("Admin"));
            var obj = new LabCreationDTO
            {
                RoomNo = 1,
                BuildingNo = 2,
                GridType = GridType.type1,
                Status = LabStatus.Vacant
            };
            var result =await _controller.CreateLab(obj);
            var ActionResult = Assert.IsAssignableFrom<RedirectToActionResult>(result);
            Assert.Null(ActionResult.RouteValues);
        }
        [Fact]
        public async void DeleteLab_LabId45_ReturnsRedirectResult()
        {
            _mockLabRepo.Setup(r => r.DeleteLab(It.IsAny<int>())).Returns(CommonMethods.getDummyHttpResponseMessageWithError());
            _mockHttpContextAccessor.Setup(r => r.HttpContext!.Request.Headers["Referer"]).Returns("randomUrl");
            var result = await _controller.DeleteLab(45);
            var ActionResult = Assert.IsAssignableFrom<RedirectResult>(result);
            Assert.Equal("randomUrl", ActionResult.Url);
        }

        private Task<Lab> GetDummyLab()
        {
            Lab? lab = new Lab();
            return Task.FromResult(lab);
        }
        private Task<List<SelectListItem>> GetDummySelectListItems()
        {
            return Task.FromResult(new List<SelectListItem> { new SelectListItem() });
        }
        private Task<List<Computer>> GetDummyComputerList()
        {
            return Task.FromResult(new List<Computer> { new Computer() });
        }
    }
}
