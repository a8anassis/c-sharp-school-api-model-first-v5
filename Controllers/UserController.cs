using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersStudentsAPIApp.Data;
using UsersStudentsAPIApp.DTO;
using UsersStudentsAPIApp.Services;
using UsersStudentsAPIApp.Services.Exceptions;

namespace UsersStudentsAPIApp.Controllers
{

    public class UserController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserController(IApplicationService applicationService, IConfiguration configuration, 
            IMapper mapper) 
            : base(applicationService)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<UserReadOnlyDTO>> SignupUserAsync(UserSignupDTO? userSignupDTO)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is not valid, build a custom response
                var errors = ModelState
                    .Where(e => e.Value!.Errors.Any())
                    .Select(e => new
                    {
                        Field = e.Key,
                        Errors = e.Value!.Errors.Select(error => error.ErrorMessage).ToArray()
                    });

                // instead of return BadRequest(new { Errors = errors });
                throw new InvalidRegistrationException("ErrorsInRegistation: " + errors.ToString());
            }
            if (_applicationService == null)
            {
                throw new ServerGenericException("ApplicationServiceNull");
            }
            User? user = await _applicationService.UserService.GetUserByUsernameAsync(userSignupDTO!.Username!);
            if (user is not null)
            {
                throw new UserAlreadyExistsException("UserExists: " + user.Username);
            }
            User? returnedUser = await _applicationService.UserService.SignUpUserAsync(userSignupDTO);         
            if (returnedUser is null)
            {
                throw new InvalidRegistrationException("InvalidRegistration");
            }
            var returnedUserDTO = _mapper.Map<UserReadOnlyDTO>(returnedUser);
            return CreatedAtAction(nameof(GetUserById), new { id = returnedUserDTO.Id }, returnedUserDTO);
        }

        [HttpGet]
        public async Task<ActionResult<UserReadOnlyDTO>> GetUserById(int id)
        {
            var user = await _applicationService.UserService.GetUserById(id);
            if (user is null)
            {
                throw new UserNotFoundException("UserNotFound");
            }

            var returnedUser = _mapper.Map<UserReadOnlyDTO>(user);
            return Ok(returnedUser);
        }

        [HttpGet]
        public async Task<ActionResult<UserTeacherReadOnlyDTO>> GetUserTeacherByUsername(string? username)
        {
            var returnedUserTeacherDTO = await _applicationService.UserService.GetUserTeacherByUsername(username);
            if (returnedUserTeacherDTO is null)
            {
                throw new UserNotFoundException("UserNotFound");
            }
            
            return Ok(returnedUserTeacherDTO);
        }

        [HttpPost]
        public async Task<ActionResult<JwtTokenDTO>> LoginUserAsync(UserLoginDTO credentials)
        {
            var user = await _applicationService.UserService.VerifyAndGetUserAsync(credentials);
            if (user == null)
            {
                throw new UnauthorizedAccessException("BadCredentials");
            }

            var userToken = _applicationService.UserService.CreateUserToken(user.Id, user.Username!, user.Email!, 
                user.UserRole, _configuration["Authentication:SecretKey"]!);
            
            JwtTokenDTO token = new()
            {
                Token = userToken
            };

            return Ok(token);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> UpdateUserPatch(UserPatchDTO patchDTO)
        {
            var userId = AppUser!.Id;
            var user = await _applicationService.UserService.UpdateUserPatchAsync(userId, patchDTO);
            var userDTO = _mapper.Map<UserDTO>(user);
            return Ok(userDTO);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDTO>> UpdateUserAccount(UserDTO? userDTO)
        {
            var userId = AppUser!.Id;
            var user = await _applicationService.UserService.UpdateUserAsync(userId, userDTO!);
            var returnedUserDTO = _mapper.Map<UserDTO>(user);
            return Ok(returnedUserDTO);
        }
    }
}
