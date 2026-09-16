using Grpc.Core;
using Identity.Core.Persistence.Entities;
using Identity.GRPC;
using Platform.Lib.Core.Persistence.Repositories;

namespace Identity.API.GrpcServices;

public class IdentityGrpcService : IdentityService.IdentityServiceBase
{
    public IRepository<User> _userRepository { get; set; }
    public ILogger<IdentityGrpcService> _logger { get; set; }

    public IdentityGrpcService(IRepository<User> userRepository, ILogger<IdentityGrpcService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public override async Task<GetUserPermissionsResponse> GetUserPermissions(GetUserPermissionsRequest request, ServerCallContext context)
    {

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogError("IdentityGrpcService.GetUserPermissions: Invalid User ID: {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid User ID"));
        }

        var permissions = await _userRepository.FirstOrDefaultAsync(x => x.Role!.RolePermissions.Select(p => p.Permission!.Name), x => x.Id == userId);

        if (permissions == null)
        {
            _logger.LogError("IdentityGrpcService.GetUserPermissions: User not found or have no permissions: {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.NotFound, "IdentityGrpcService.GetUserPermissions: User not found or have no permissions"));
        }

        return new GetUserPermissionsResponse { Permissions = { permissions } };
    }


}
