using FreeMediator;
using Identity.Application.Auth.Commands;
using Identity.Core.Persistence.Entities;
using Platform.Lib.Core.Exceptions;
using Platform.Lib.Core.Persistence.Repositories;
using Platform.Lib.Core.Services.Identity.Enums;
using Platform.Lib.Core.Services.Security;

namespace Identity.Application.Auth.Handlers
{
    public class RegisterCustomerHandler : IRequestHandler<RegisterCustomerCommand, bool>
    {
        private readonly IHashService _hashService;
        public IRepository<User> _userRepository { get; set; }

        public RegisterCustomerHandler(IHashService hashService, IRepository<User> userRepository)
        {

            _userRepository = userRepository;
            _hashService = hashService;
        }

        public async Task<bool> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
        {

            var user = await _userRepository.FirstOrDefaultAsync(x => x.UserName == request.UserName);
            if (user != null) { AppException.Throw("UserAlreadyExist"); }

            var newUser = new User
            {
                UserName = request.UserName,
                PasswordHash = _hashService.Hash(request.Password),
                NameAr = request.FullNameAr,
                NameEn = request.FullNameEn,
                IsActive = true,
                IsBuiltIn = false,
                RoleId = null,
                UserType = UserTypes.Customer
            };

            await _userRepository.CreateAsync(newUser);



            return true;
        }
    }
}
