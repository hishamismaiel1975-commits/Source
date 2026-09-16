using Grpc.Core;
using Identity.Core.Persistence.Entities;
using Identity.GRPC;
using Platform.Lib.Core.Persistence.Repositories;
using System.Linq.Expressions;

namespace Identity.API.GrpcServices;

public class IdentityGrpcService : IdentityService.IdentityServiceBase
{
    public IRepository<User> _userRepository { get; set; }

    public IdentityGrpcService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task<GetUserPermissionsResponse> GetUserPermissions(GetUserPermissionsRequest request, ServerCallContext context)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.UserId),
            new List<Expression<Func<User, object>>>
               {
                   x => x.Role
               }
            );

        return null;
    }


}
