using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsersStudentsAPIApp.Models;
using UsersStudentsAPIApp.Services;

namespace UsersStudentsAPIApp.Controllers
{
    // [Produces("application/json")] default since it derives from ControllerBase
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController : ControllerBase
    {      
        public readonly IApplicationService _applicationService;

        protected BaseController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        private ApplicationUser? _appUser;

        protected ApplicationUser? AppUser
        {
            get
            {
                if (User != null && User.Claims != null && User.Claims.Any())
                {

                    var claimsTypes = User.Claims.Select(x => x.Type);
                    if (!claimsTypes.Contains(ClaimTypes.NameIdentifier))
                    {
                        return null;
                    }

                    var userClaimsId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    _ = int.TryParse(userClaimsId, out int id);

                    _appUser = new ApplicationUser
                    {
                        Id = id
                    };

                    var userClaimsName = User.FindFirst(ClaimTypes.Name)?.Value;

                    _appUser.Username = userClaimsName!;
                    _appUser.Email = User.FindFirst(ClaimTypes.Email)?.Value;
                    return _appUser;
                }
                return null;
            }
        }
    }
}
