using luizalabs.desafio_tecnico.Interfaces;
using luizalabs.desafio_tecnico.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace luizalabs.desafio_tecnico.test.LocalMock.Data
{
    internal class UserData : IUserData
    {
        private List<User> Users { get; set; }

        public UserData() 
        {
            Users = new List<User>();
        }

        public async Task<bool> DeleteAsync(User user)
        {
            Users.Remove(user);
            return true;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return Users.FirstOrDefault(u => u.user_id == id);
        }

        public async Task<User?> GetByIdLegacyAsync(int id)
        {
            return Users.FirstOrDefault(u => u.legacy_user_id == id);
        }

        public async Task<List<User>> GetListAsync()
        {
            return Users;
        }

        public async Task<User> LoadAsync(User user)
        {
            return user;
        }

        public async Task<User> SaveAsync(User user)
        {
            if (!Users.Contains(user)) 
            { 
                user.user_id = Guid.NewGuid();
                Users.Add(user);
            }
            return user;
        }

        public async Task<List<User>> GetListByRequestIdAsync(Guid id, bool load)
        {
            return Users;
        }

        public Task<List<User>> GetListByRequestIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
