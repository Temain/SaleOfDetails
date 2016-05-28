using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using SaleOfDetails.Domain.DataAccess.Interfaces;
using SaleOfDetails.Domain.Models;
using SaleOfDetails.Web.Models;

namespace SaleOfDetails.Web.Controllers
{
    [Authorize]
    public class MeController : BaseApiController
    {
        public MeController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        //public MeController(ApplicationUserManager userManager)
        //{
        //    UserManager = userManager;
        //}

        // GET api/Me
        public GetViewModel Get()
        {
            //var hometown = UserProfile.Hometown;
            var user = UserManager.FindById(User.Identity.GetUserId());
            var employee = UnitOfWork.Repository<Person>()
                .Get()
                .FirstOrDefault();

            return new GetViewModel() { LastName = employee.LastName };
        }
    }
}