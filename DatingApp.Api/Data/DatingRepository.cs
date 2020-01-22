using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Api.Helpers;
using DatingApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Api.Data
{
    public class DatingRepository : IDatingRepository
    {
        private DataContext _context;
        public DatingRepository(DataContext context)
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

        public async Task<User> GetUser(int id)
        {
            var user  = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u =>u.Id==id);
            return user;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo  = await _context.Photo.FirstOrDefaultAsync(p =>p.Id==id);
            return photo;
        }

        public async   Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p=> p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.UserName != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            if(userParams.MinAge !=18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Now.AddYears(-userParams.MaxAge);
                var maxDob = DateTime.Now.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if(! string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created":
                        users= users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;

                }

            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetActivePhotoForUser(int userId)
        {
            return await _context.Photo.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsActive);
        }
    }
}