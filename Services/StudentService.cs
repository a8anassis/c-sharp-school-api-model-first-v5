using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UsersStudentsAPIApp.Data;
using UsersStudentsAPIApp.Models;
using UsersStudentsAPIApp.Repositories;

namespace UsersStudentsAPIApp.Services
{
    public class StudentService : IStudentService
    {

        private readonly IUnitOfWork? _unitOfWork;
        private readonly IMapper? _mapper;
        private readonly ILogger<UserService>? _logger;

        public StudentService(IUnitOfWork? unitOfWork, ILogger<UserService>? logger, IMapper? mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;            
        }



        public async Task<bool> DeleteStudentAsync(int id)
        {
            bool studentDeleted = false;
            try
            {
                studentDeleted = await _unitOfWork!.StudentRepository.DeleteAsync(id);
                _logger!.LogInformation("{Message}", "Student with id:  " + id + " deleted, success");
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return studentDeleted;
        }

        public async Task<IEnumerable<User>> GetAllStudentsAsync()
        {
            List<User> usersStudents = new();
            try
            {
                //students = await _unitOfWork!.StudentRepository.GetAllAsync();
                usersStudents = await _unitOfWork!.StudentRepository.GetAllUsersStudentsAsync();
                _logger!.LogInformation("{Message}", "All students returned with success");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return usersStudents;
        }

        public async Task<Student?> GetStudentAsync(int id)
        {
            Student? student = null;
            try
            {
                student = await _unitOfWork!.StudentRepository.GetAsync(id);
                _logger!.LogInformation("{Message}", "Student with id: " + id + " retrieved with success");
            }
            catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return student;
        }

        public async Task<int> GetStudentCountAsync()
        {
            int count = 0;
            try {
                count =  await _unitOfWork!.StudentRepository.GetCountAsync();
                _logger!.LogInformation("{Message}", "Student count retrieved with success");
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return count;
        }

        public async Task<List<Course>> GetStudentCoursesAsync(int id)
        {
            List<Course> courses = new();

            try
            {
                courses = await _unitOfWork!.StudentRepository.GetStudentCoursesAsync(id);
                _logger!.LogInformation("{Message}", "Student count retrieved with success");
            } catch (Exception e)
            {
                _logger!.LogError("{Message}{Exception}", e.Message, e.StackTrace);
            }
            return courses;
        }     
    }
}
