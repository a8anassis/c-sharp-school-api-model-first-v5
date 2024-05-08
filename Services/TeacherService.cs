using AutoMapper;
using UsersStudentsAPIApp.Data;
using UsersStudentsAPIApp.Repositories;
using UsersStudentsAPIApp.Services;

namespace UsersStudentsAPIApp.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork? _unitOfWork;
        private readonly IMapper? _mapper;
        private readonly ILogger<UserService>? _logger;

        public TeacherService(IUnitOfWork? unitOfWork, ILogger<UserService>? logger, IMapper? mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<User>> GetAllUsersTeachersAsync()
        {
            List<User> usersTeachers = new();
            try
            {
                usersTeachers = await _unitOfWork!.TeacherRepository.GetAllUsersTeachersAsync();
                _logger!.LogInformation("{Message}", "All teachers returned with success");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return usersTeachers;
        }

        public async Task<List<User>> GetAllUsersTeachersAsync(int pageNumber, int pageSize)
        {
            List<User> usersTeachers = new();
            int skip = 1;
            try
            {
                // Start pagination from 1
                skip = pageNumber * pageSize;
                usersTeachers = await _unitOfWork!.TeacherRepository.GetAllUsersTeachersAsync(pageNumber, pageSize);
                _logger!.LogInformation("{Message}", "All teachers paginated returned with success");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return usersTeachers;
        }


        public async Task<User?> GetTeacherByUsernameAsync(string? username)
        {
            return await _unitOfWork!.TeacherRepository.GetTeacherByUsernameAync(username!);
        }

        public Task<int> GetTeacherCountAsync()
        {
            throw new NotImplementedException();
        }
    }
}
