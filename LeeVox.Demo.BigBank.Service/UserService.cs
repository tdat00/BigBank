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
        public IBankAccountService BankAccountService {get; set;}
        public IUserRepository UserRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IUserService> Logger {get; set;}

        public UserService(
            IJwtService jwtService,
            IBankAccountService bankAccountService,

            IUserRepository userRepository,

            IUnitOfWork unitOfWork,
            ILogger<IUserService> logger)
        {
            this.JwtService = jwtService;
            this.BankAccountService = bankAccountService;
            
            this.UserRepository = userRepository;

            this.UnitOfWork = unitOfWork;
            this.Logger = logger;
        }

        public User Get(int id)
        {
            var entity = UserRepository.ById(id);
            entity.EnsureNotNull("User");

            return entity;
        }

        public User Get(string email)
        {
            email.EnsureNotNullOrWhiteSpace(nameof(email));

            var entity = UserRepository.ByEmail(email);
            entity.EnsureNotNull("Email");

            return entity;
        }

        public IEnumerable<BankAccount> GetBankAccounts(string email)
        {
            email = email.EnsureNotNullOrWhiteSpace(nameof(email));
            
            var user = Get(email);
            user.EnsureNotNull("Email");

            return user.Accounts;
        }

        public int Create(string email, string password, string role, string firstName, string lastName)
            => Create(email, password, role, firstName, lastName, null, null);

        public int Create(string email, string password, string role, string firstName, string lastName, string bankAccount, string bankAccountCurrency)
        {
            password.EnsureNotNullOrWhiteSpace(nameof(password));
            email.EnsureNotNullOrWhiteSpace(nameof(email));
            firstName.EnsureNotNullOrWhiteSpace(nameof(firstName));
            lastName.EnsureNotNullOrWhiteSpace(nameof(lastName));
            
            if (UserRepository.ByEmail(email) != null)
            {
                throw new BusinessException("Email is taken.");
            }

            if (!Enum.TryParse(role, out UserRole userRole))
                userRole = UserRole.Customer;

            var random = new CryptoRandom();
            var hasher = new CryptoHash();
            var salt = random.RandomBytes(16).GetHexaString();
            //TODO: use slow hash instead of SHA
            var passwordHash = hasher.Sha256(salt + password).GetHexaString();

            var entity = new User
            {
                Email = email,
                Role = userRole,
                FirstName = firstName,
                LastName = lastName,
                
                PasswordSalt = salt,
                PasswordHash = passwordHash
            };

            UserRepository.Create(entity);
            UnitOfWork.SaveChanges();
            //TODO: using transaction in real db.
            if (!string.IsNullOrWhiteSpace(bankAccount) && !string.IsNullOrWhiteSpace(bankAccountCurrency))
            {
                BankAccountService.Create(bankAccount, bankAccountCurrency, entity.Id);
            }

            return entity.Id;
        }

        public string Login(string email, string password)
        {
            password = password.EnsureNotNullOrWhiteSpace(nameof(password));

            var user = Get(email);
            var hasher = new CryptoHash();
            //TODO: use slow hash instead of SHA
            var passwordHash = hasher.Sha256(user.PasswordSalt + password).GetHexaString();

            if (passwordHash.IsOrdinalEqual(user.PasswordHash, true))
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
    }
}
