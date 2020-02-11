using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentShare.API.Helpers;
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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => 
            u.LikerId == userId && u.LikeeId == recipientId); // returns null if none of the ID's match otherwise it return the like
        }

        public async Task<Photo> GetMainPhoto(int userId)
        {
            return await _context.Photos.Where(n => n.UserId == userId)
            .FirstOrDefaultAsync(p => p.MainPhoto); // returns the main photo based on the UserId
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

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        { // returns a list of the users as well as the users photos
            var users = _context.Users.Include(p => p.Photo).OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id)); // returns userLikers that match a user id in the users, as users
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id)); // returns userLikees that match a user id in the users, as users
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize); // returns the users as a paged list
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(x => x.Likers)
            .Include(x => x.Likees).FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId); // returns a list of likers of the current user
            }
            else 
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId); // returns a list of likees of the current user
            }
        }

        public async Task<bool> SaveAll()
        { // returns true or false for changes saved to the DB, more than 0 will return true, 0 returns false
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Message> GetMessage(int id)
        {
            return await  _context.Messages.FirstOrDefaultAsync(m => m.Id == id); // gets message from the db 
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender).ThenInclude( p=> p.Photo) // get sender and photo of sender from db
                .Include(u => u.Recipient).ThenInclude(p => p.Photo) // get recipient and photo of recipient from db
                .AsQueryable();

            switch (messageParams.MessageContainer) 
            {   // switch through the different types of messages
                case "Inbox":
                    messages = messages.Where(u => u.RecipientId 
                    == messageParams.UserId && u.RecipientDeleted == false); // inbox to show recieved messages where recipientId matches UserId and are not deleted by the recipient
                    break;
                    case "Outbox":
                    messages = messages.Where(u => u.SenderId 
                    == messageParams.UserId && u.SenderDeleted == false); // outbox to show sent messages where senderId matches UserId and are not deleted by the sender
                    break;
                    default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false 
                    && u.IsRead == false); // inbox to show recieved messages where recipientId matches UserId & the message hasnt been read and the recipient hasn't deleted
                    break;

            }

            messages = messages.OrderByDescending(d => d.MessageSent); // sort messages by most recent first
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize); // returns the paged list of messages
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages.Include(u => u.Sender).ThenInclude(p=> p.Photo) // get sender and photo of sender from db
                .Include(u => u.Recipient).ThenInclude(p => p.Photo) // get recipient and photo of recipient from db
                .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId 
                || m.RecipientId == recipientId && m.SenderId == userId && m.SenderDeleted == false) // gets complete conversation, to and from each user
                    .OrderByDescending(m => m.MessageSent) // gets back in order
                    .ToListAsync();

            return messages;
        }
    }
}