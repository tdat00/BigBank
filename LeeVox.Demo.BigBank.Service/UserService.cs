using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeVox.Sdk;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;

namespace LeeVox.Demo.BigBank.Service
{
    public class UserService : IUserService
    {
        public IUnitOfWork UnitOfWork {get; set;}
        public IUserRepository Repository {get; set;}

        public UserService(IUnitOfWork unitOfWork, IUserRepository customerRepository)
        {
            this.UnitOfWork = unitOfWork;
            this.Repository = customerRepository;
        }

        public User Get(int id)
        {
            return Repository.ById(id);
        }

        public User GetByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return Repository.All.FirstOrDefault(e => email.IsOrdinalEqual(e.Email));
        }

        public IEnumerable<User> Get(string search = null, int skip = 0, int take = 10)
        {
            var result = Repository.All;

            if (!string.IsNullOrWhiteSpace(search))
            {
                result = result.Where(
                    e => e.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                        || e.Email.Contains(search, StringComparison.InvariantCultureIgnoreCase)
                );
            }
            
            return result.OrderBy(e => e.Id).Skip(skip).Take(take);
        }

        public int Create(User user)
        {
            var entity = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = user.Password,

                //TODO: secure password
                PasswordHash = user.Password + "-hashed"
            };

            Repository.Create(entity);
            UnitOfWork.SaveChanges();

            return entity.Id;
        }

        public string Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(nameof(email));
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException(nameof(password));
            }

            //TODO: secure password
            var passwordHash = password + "-hashed";
            var found = Repository.All.FirstOrDefault(e => email.IsOrdinalEqual(e.Email) && passwordHash.IsOrdinalEqual(e.PasswordHash));

            if (found != null)
            {
                return "found";
            }
            else
            {
                return string.Empty;
            }
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            var entity = Repository.ById(id);

            if (entity == null)
            {
                throw new Exception("User Id does not exist.");
            }

            Repository.Delete(id);
            UnitOfWork.SaveChanges();
        }
    }
}
