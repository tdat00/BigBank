using System;
using System.Collections.Generic;
using System.Linq;
using LeeVox.Sdk;
using LeeVox.Demo.BigBank.Core;
using LeeVox.Demo.BigBank.Data;
using LeeVox.Demo.BigBank.Model;
using Microsoft.Extensions.Logging;

namespace LeeVox.Demo.BigBank.Service
{
    public class UserService : IUserService
    {
        public IJwtService JwtService {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public IUserRepository UserRepository {get; set;}
        public IBankAccountService BankAccountService {get; set;}
        public ILogger<IUserService> Logger {get; set;}

        public UserService(IUnitOfWork unitOfWork, IJwtService jwtService, IUserRepository userRepository, IBankAccountService bankAccountService, ILogger<IUserService> logger)
        {
            this.UnitOfWork = unitOfWork;
            this.JwtService = jwtService;
            this.UserRepository = userRepository;
            this.BankAccountService = bankAccountService;
            this.Logger = logger;
        }

        public User Get(int id)
        {
            return UserRepository.ById(id);
        }

        public User Get(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }

            return UserRepository.ByEmail(email);
        }

        public IEnumerable<User> Get(string search = null, int skip = 0, int take = 10)
        {
            var result = UserRepository.All;

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
            => Create(user, null);
        public int Create(User user, BankAccount newBankAccount)
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

            UserRepository.Create(entity);
            UnitOfWork.SaveChanges();

            if (newBankAccount != null)
            {
                newBankAccount.User = entity;
                BankAccountService.Create(newBankAccount);
            }

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
                return JwtService.GenerateToken(user);
            }
            else
            {
                throw new BusinessException("Email or password is not correct.");
            }
        }

        public void Logout(string session)
        {
            JwtService.RemoveSession(session);
        }

        public void Delete(int id)
        {
            var entity = UserRepository.ById(id);

            if (entity == null)
            {
                throw new BusinessException("User Id does not exist.");
            }

            UserRepository.Delete(id);
            UnitOfWork.SaveChanges();
        }

        public void Delete(string email) => Delete(Get(email).Id);
    }
}
