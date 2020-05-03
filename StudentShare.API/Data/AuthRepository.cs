using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentShare.API.Models;

namespace StudentShare.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context; //lets us view the DB data - hence read only

        public AuthRepository(DataContext context)
        {
            _context = context;

        }
        public async Task<User> Login(string username, string password) //async to make it possible for many users to do at the same time
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username); //find username that matches or return null

            if (user == null)
                return null;
            
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) 
                return null;

            return user;
        
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for(int i = 0; i < computedHash.Length; i++) // looping through hash to make sure they match
                {
                    if (computedHash[i] != passwordHash[i]) 
                    return false;
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password) //async to make it possible for many users to do at the same time
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password,out passwordHash,out passwordSalt); //when hash and salt are created, they are auto updated with the out's
 
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user); 
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string username, string email)
        {
            if (await _context.Users.AnyAsync(x => x.Username == username)) //checks db to see if username exists
                return true;
            if (await _context.Users.AnyAsync(x => x.Email == email)) //checks db to see if email exists
                return true;

            return false;
        }
    }
}