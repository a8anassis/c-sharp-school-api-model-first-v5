using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UsersStudentsAPIApp.Data;
using UsersStudentsAPIApp.DTO;
using UsersStudentsAPIApp.Models;
using UsersStudentsAPIApp.Repositories;
using UsersStudentsAPIApp.Security;
using UsersStudentsAPIApp.Services.Exceptions;
namespace UsersStudentsAPIApp.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork? _unitOfWork;       
        private readonly ILogger<UserService>? _logger;
        private readonly IMapper? _mapper;

        public UserService(IUnitOfWork? unitOfWork, ILogger<UserService>? logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;           
            _logger = logger;  
            _mapper = mapper;
        }

        public async Task<User?> SignUpUserAsync(UserSignupDTO signupDTO)
        {            
            Student? student;
            Teacher? teacher;
            User? user;

            try
            {
                user = ExtractUser(signupDTO);
                User? existingUser = await _unitOfWork!.UserRepository
                    .GetByUsernameAsync(user.Username!);

                if ( existingUser != null)
                {
                    throw new UserAlreadyExistsException("UserExists: " + existingUser.Username);
                }

                user.Password = EncryptionUtil.Encrypt(user.Password!);
                await _unitOfWork!.UserRepository.AddAsync(user);

                if (user!.UserRole!.Value.ToString().Equals("Student"))
                {
                    student = ExtractStudent(signupDTO);
                    if (await _unitOfWork!.StudentRepository.GetByPhoneNumber(student.PhoneNumber) is not null)
                    {
                        throw new StudentAlreadyExistsException("StudentExists");
                    }
                    await _unitOfWork!.StudentRepository.AddAsync(student);
                    user.Student = student;
                    // student.User = user; Since entities are attached EF manages the other end of the relationship
                }
                else if (user!.UserRole!.Value.ToString().Equals("Teacher"))
                {
                    teacher = ExtractTeacher(signupDTO);
                    if (await _unitOfWork!.TeacherRepository.GetByPhoneNumber(teacher.PhoneNumber) is not null)
                    {
                        throw new TeacherAlreadyExistsException("TeacherExists");
                    }
                    await _unitOfWork!.TeacherRepository.AddAsync(teacher);
                    user.Teacher = teacher;
                    // teacher.User = user;   // EF manages the other end since both entities are attached 
                }
                else
                {
                    throw new InvalidRoleException("InvalidRole");                 
                }                
                await _unitOfWork!.SaveAsync();
                _logger!.LogInformation("{Message}", "User: " + user + " signup success");
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
            return user;
        }

        public async Task<User?> VerifyAndGetUserAsync(UserLoginDTO request)
        {
            User? user = null;

            try
            {
                user = await _unitOfWork!.UserRepository.GetUserAsync(request.Username!, request.Password!);
                _logger!.LogInformation("{Message}", "User: " + user + " found and returned");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }           
            return user;
        }

        public async Task<User?> UpdateUserAsync(int userId, UserDTO userDTO)
        {
            User? existingUser;
            User? user;
            try
            {
                existingUser = await _unitOfWork!.UserRepository.GetAsync(userId);
                if (existingUser == null) return null;

                var userToUpdate = _mapper!.Map<User>(userDTO);

                user = await _unitOfWork.UserRepository.UpdateUserAsync(userId, userToUpdate);
                await _unitOfWork.SaveAsync();
                _logger!.LogInformation("{Message}", "User: " + user + " updated successfully");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
            return user;
        }

        public async Task<User?> UpdateUserPatchAsync(int userId, UserPatchDTO userPatchDTO)
        {
            User? existingUser;
            User? user = null;
            try
            {
                existingUser = await _unitOfWork!.UserRepository.GetAsync(userId);
                if (existingUser == null) return null;

                existingUser.Username = userPatchDTO.Username;
                existingUser.Email = userPatchDTO.Email;
                existingUser.Password = EncryptionUtil.Encrypt(userPatchDTO.Password!);

                // No need for Update command. Since existing user is attached
                // EF will detect changes and provide an update SQL statement.
                // The update statement will include only the modified columns.
                await _unitOfWork.SaveAsync();
                _logger!.LogInformation("{Message}", "User: " + user + " updated successfully");
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            User? user;
            try
            {
                user = await _unitOfWork!.UserRepository.GetByUsernameAsync(username);
                _logger!.LogInformation("{Message}", "User: " + user + " found and returned" );
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
            return user;
        }

        public async Task<List<User>> GetAllUsersFiltered(int pageNumber, int pageSize, 
            UserFiltersDTO userFiltersDTO)
        {
            List<User> users = new();
            List<Func<User, bool>> predicates = new();

            try
            {
                // Add individual predicates for filtering conditions
                if (!string.IsNullOrEmpty(userFiltersDTO.Username))
                {
                    predicates.Add(u => u.Username == userFiltersDTO.Username);
                }
                if (!string.IsNullOrEmpty(userFiltersDTO.Email))
                {
                    predicates.Add(u => u.Email == userFiltersDTO.Email);
                }
                if (!string.IsNullOrEmpty(userFiltersDTO.UserRole.ToString()))
                {
                    predicates.Add(u => u.UserRole == userFiltersDTO.UserRole);
                }
                users = await _unitOfWork!.UserRepository.GetAllUsersFilteredAsync(pageNumber, pageSize,
                    predicates);
                _logger!.LogInformation("{Message}", "User Filtered Returned Success");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
            return users;
        }


        public string CreateUserToken(int userId, string? username, string? email, UserRole? userRole, 
            string? appSecurityKey)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSecurityKey!));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsInfo = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username!),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email!),
                new Claim(ClaimTypes.Role, userRole.ToString()!)  
            };

            var jwtSecurityToken = new JwtSecurityToken(null, null, claimsInfo, DateTime.UtcNow, 
                DateTime.UtcNow.AddHours(3), signingCredentials);

            // Serialize the token
            var userToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            //return "Bearer " + userToken;
            return userToken;
        }

        public async Task<UserTeacherReadOnlyDTO?> GetUserTeacherByUsername(string? username)
        {
            UserTeacherReadOnlyDTO? dto;
            try
            {
                dto = await _unitOfWork!.UserRepository.GetUserTeacherInfoAync(username!);
                _logger!.LogInformation("{Message}", "User with username: " + username + " Success");
                return dto;
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
        }

        public async Task<User?> GetUserById(int id)
        {
            User? user;
            try
            {
                user = await _unitOfWork!.UserRepository.GetAsync(id);
                _logger!.LogInformation("{Message}", "User with id: " + id + " Success");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            bool deleted;
            try
            {
                deleted = await _unitOfWork!.UserRepository.DeleteAsync(id);
                if (!deleted)
                {
                    throw new UserNotFoundException("UserNotFound");
                }
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
                throw;
            }
        }

        private User ExtractUser(UserSignupDTO signupDTO)
        {
            return new User()
            {
                Username = signupDTO.Username,
                Password = signupDTO.Password,
                Email = signupDTO.Email,
                Firstname = signupDTO.Firstname,
                Lastname = signupDTO.Lastname,
                UserRole = signupDTO.UserRole
            };
        }

        private Student ExtractStudent(UserSignupDTO? signupDTO)
        {
            return new Student()
            {
                PhoneNumber = signupDTO!.PhoneNumber!,
                Institution = signupDTO!.Institution,
            };
        }

        private Teacher ExtractTeacher(UserSignupDTO? signupDTO)
        {
            return new Teacher()
            {
                PhoneNumber = signupDTO!.PhoneNumber!,
                Institution = signupDTO!.Institution,
            };
        }
    }
}
