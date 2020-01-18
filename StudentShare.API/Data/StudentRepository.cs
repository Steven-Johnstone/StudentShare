using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentShare.API.Models;

namespace StudentShare.API.Data
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DataContext _context; //declaring the DB
        public StudentRepository(DataContext context)
        {
            _context = context;
        }

        public DataContext Context { get; }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity); // adds the entity to the DB
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity); // removes the entity
        }

        public async Task<Photo> GetMainPhoto(int userId)
        {
            return await _context.Photos.Where(n => n.UserId == userId).FirstOrDefaultAsync(p => p.MainPhoto); // returns the main photo based on the UserId
        }

        public async Task<Photo> GetPhoto(int id)
        {   // going to context to get first match of id being passed in
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo; // returning the photo
        }

        public async Task<User> GetUser(int id) 
        { // used to get a user and photos via the user ID, or return it as null
            var user = await _context.Users.Include(p => p.Photo).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        { // returns a list of the users as well as the users photos
            var users = await _context.Users.Include(p => p.Photo).ToListAsync();

            return users;
        }

        public async Task<bool> SaveAll()
        { // returns true or false for changes saved to the DB, more than 0 will return true, 0 returns false
            return await _context.SaveChangesAsync() > 0;
        }
    }
}