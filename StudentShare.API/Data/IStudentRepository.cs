using System.Collections.Generic;
using System.Threading.Tasks;
using StudentShare.API.Models;

namespace StudentShare.API.Data
{
    public interface IStudentRepository
    {
         void Add<T>(T entity) where T: class; // generic method to add a type of user/photo where just classes
         void Delete<T>(T entity) where T: class; // generic method to delete a type of user/photo
         Task<bool> SaveAll(); // method to save changes to the DB
         Task<IEnumerable<User>> GetUsers(); // method to return a list of users
         Task<User> GetUser(int id); // method to get a specific user
    }
}