using ModelsLibrary.Models;
using ModelsLibrary.Models.DTO;
using ModelsLibrary.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace CLabManager_Web.Tests
{
    public static class CommonMethods
    {
        public static ClaimsPrincipal getDummyClaimsPrincipal(string role)
        {
            var identity = new ClaimsIdentity("yahya");
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            return new ClaimsPrincipal(identity);
        }
        public static Task<HttpResponseMessage> getDummyHttpResponseMessage()
        {
            var obj = new Lab
            {
                LabId = 45,
                RoomNo = 6,
                BuildingNo = 7,
                Status = LabStatus.Vacant,
                GridType = GridType.type2,
                ComputerList = new List<ComputerDTO>()

            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(obj));
            var message = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.Created,
                Content = content
            };
            return Task.FromResult(message);
        }
        public static Task<HttpResponseMessage> getDummyHttpResponseMessageWithError()
        {
            var obj = new Lab
            {
                LabId = 45,
                RoomNo = 6,
                BuildingNo = 7,
                Status = LabStatus.Vacant,
                GridType = GridType.type2,
                ComputerList = new List<ComputerDTO>()

            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(obj));
            var message = new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Content = content
            };
            return Task.FromResult(message);
        }
    }
}
