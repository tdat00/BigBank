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
        public ICryptoHash CryptoHash {get; set;}
        public IJwtService JwtService {get; set;}
        public IBankAccountService BankAccountService {get; set;}
        public IUserRepository UserRepository {get; set;}
        public IUnitOfWork UnitOfWork {get; set;}
        public ILogger<IUserService> Logger {get; set;}

        public UserService(
            ICryptoHash cryptoHash,

            IJwtService jwtService,
            IBankAccountService bankAccountService,

            IUserRepository userRepository,

            IUnitOfWork unitOfWork,
            ILogger<IUserService> logger)
        {
            this.CryptoHash = cryptoHash;

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
            => BankAccountService.GetByUser(email);

        public (int userId, int? bankAccountId) Create(string email, string password, string role, string firstName, string lastName, string bankAccount, string bankAccountCurrency)
        {
            password.EnsureNotNullOrWhiteSpace(nameof(password));
            email.EnsureNotNullOrWhiteSpace(nameof(email));
            firstName.EnsureNotNullOrWhiteSpace(nameof(firstName));
            lastName.EnsureNotNullOrWhiteSpace(nameof(lastName));

            var createBankAccount = !string.IsNullOrWhiteSpace(bankAccount) && !string.IsNullOrWhiteSpace(bankAccountCurrency);
            
            if (UserRepository.ByEmail(email) != null)
            {
                throw new BusinessException("Email is taken.");
            }

            if (createBankAccount && BankAccountService.Exists(bankAccount))
            {
                throw new BusinessException("Bank account name is taken.");
            }

            if (!Enum.TryParse(role, out UserRole userRole))
                userRole = UserRole.Customer;

            var (hash, salt) = CryptoHash.Pbkdf2Hash(password);

            var entity = new User
            {
                Email = email,
                Role = userRole,
                FirstName = firstName,
                LastName = lastName,
                
                PasswordSalt = salt.GetBase64String(),
                PasswordHash = hash.GetBase64String()
            };

            UserRepository.Create(entity);
            UnitOfWork.SaveChanges();

            int? bankAccountId = !createBankAccount ? (int?)null : BankAccountService.Create(bankAccount, bankAccountCurrency, entity.Id);
            return (entity.Id, bankAccountId);
        }

        public string Login(string email, string password)
        {
            password = password.EnsureNotNullOrWhiteSpace(nameof(password));

            var user = Get(email);
            var passwordHash = CryptoHash.Pbkdf2Hash(password, user.PasswordSalt.ParseBase64String()).GetBase64String();

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
