using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly ApplicationDbContext _context;

        public DatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.Id != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                    {
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    }

                    default:
                    {
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                    }
                }
            }


            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(photo => photo.UserId == userId)
                .FirstOrDefaultAsync(photo => photo.IsMain);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(
                user => user.LikerId == userId && user.LikeeId == recipientId
            );
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(message => message.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                .Include(message => message.Sender).ThenInclude(user => user.Photos)
                .Include(message => message.Recipient).ThenInclude(user => user.Photos)
                .AsQueryable();

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                {
                    messages = messages.Where(
                        u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false
                    );
                    break;
                }

                case "Outbox":
                {
                    messages = messages.Where(
                        u => u.SenderId == messageParams.UserId && u.SenderDeleted == false
                    );
                    break;
                }

                default:
                {
                    messages = messages.Where(
                        u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false
                    );
                    break;
                }
            }

            messages = messages.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                .Include(message => message.Sender).ThenInclude(user => user.Photos)
                .Include(message => message.Recipient).ThenInclude(user => user.Photos)
                .Where(message =>
                    message.RecipientId == userId && message.RecipientDeleted == false && message.SenderId == recipientId ||
                    message.RecipientId == recipientId && message.SenderDeleted == false && message.SenderId == userId
                )
                .OrderByDescending(message => message.MessageSent)
                .ToListAsync();
            return messages;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
                .Include(u => u.Likers)
                .Include(u => u.Likees)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(u => u.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(u => u.LikeeId);
            }
        }
    }
}