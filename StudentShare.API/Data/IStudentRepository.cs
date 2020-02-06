using System.Collections.Generic;
using System.Threading.Tasks;
using StudentShare.API.Helpers;
using StudentShare.API.Models;

namespace StudentShare.API.Data
{
    public interface IStudentRepository
    {
         void Add<T>(T entity) where T: class; // generic method to add a type of user/photo where just classes
         void Delete<T>(T entity) where T: class; // generic method to delete a type of user/photo
         Task<bool> SaveAll(); // method to save changes to the DB
         Task<PagedList<User>> GetUsers(UserParams userParams); // method to return a list of users
         Task<User> GetUser(int id); // method to get a specific user
         Task<Photo> GetPhoto(int id); // method to get an id from photo
         Task<Photo> GetMainPhoto(int userId); // method to get main photo
         Task<Like> GetLike(int userId, int recipientId); // used to check if a like already exists 
         Task<Message> GetMessage(int id); // used to get a single message from the db
         Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams); // gets a list of messages for a user
         Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId); // method to get the conversation between 2 users

    }
}