using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeeVox.Sdk;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class UserService : IUserService
    {
        public ILoginInfoService LoginInfoService {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public IUserRepository Repository {get; set;}
        public ILogger<IUserService> Logger {get; set;}

        public UserService(IUnitOfWork unitOfWork, ILoginInfoService loginInfoService, IUserRepository customerRepository, ILogger<IUserService> logger)
        {
            this.UnitOfWork = unitOfWork;
            this.LoginInfoService = loginInfoService;
            this.Repository = customerRepository;
            this.Logger = logger;
        }

        public User Get(int id)
        {
            return Repository.ById(id);
        }

        public User Get(string email)
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
            if (string.IsNullOrWhiteSpace(user.Email) || Get(user.Email) != null)
            {
                throw new ArgumentException("Email is missing or already exists.");
            }

            var random = new CryptoRandom();
            var hasher = new CryptoHash();
            var salt = random.RandomBytes(16).GetHexaString();
            //TODO: use slow hash instead of SHA
            var passwordHash = hasher.Sha256(salt + user.Password).GetHexaString();

            var entity = new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                
                PasswordSalt = salt,
                PasswordHash = passwordHash
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

            Logger.LogInformation("Count: " + Repository.All_IncludeDeleted.Count());

            var user = Get(email);
            var found = user != null;

            if (found)
            {
                var hasher = new CryptoHash();
                //TODO: use slow hash instead of SHA
                var passwordHash = hasher.Sha256(user.PasswordSalt + password).GetHexaString();
                found = passwordHash.IsOrdinalEqual(user.PasswordHash, true);
            }

            if (found)
            {
                //TODO: use JWT instead of random token
                return LoginInfoService.AddLoginInfo(user);
            }
            else
            {
                throw new BusinessException("Email or password is not correct.");
            }
        }

        public void Logout(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(nameof(email));
            }

            LoginInfoService.RemoveLoginInfo(new User {Email = email});
        }

        public void Delete(int id)
        {
            var entity = Repository.ById(id);

            if (entity == null)
            {
                throw new BusinessException("User Id does not exist.");
            }

            Repository.Delete(id);
            UnitOfWork.SaveChanges();
        }

        public void Delete(string email) => Delete(Get(email).Id);
    }
}
