using Grpc.Core;
using Identity.Core.Persistence.Entities;
using Identity.GRPC;
using Platform.Lib.Core.Persistence.Repositories;

namespace Identity.API.GrpcServices;

public class IdentityGrpcService : IdentityService.IdentityServiceBase
{
    public IRepository<User> _userRepository;
    public IRepository<RolePermission> _rolePermissionRepository { get; set; }

    public ILogger<IdentityGrpcService> _logger { get; set; }

    public IdentityGrpcService(IRepository<User> userRepository, IRepository<RolePermission> rolePermissionRepository, ILogger<IdentityGrpcService> logger)
    {
        _userRepository = userRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _logger = logger;
    }

    public override async Task<GetUserInfoResponse> GetUserInfo(GetUserInfoRequest request, ServerCallContext context)
    {

        if (!Guid.TryParse(request.UserId, out var userId))
        {
            _logger.LogError("IdentityGrpcService.GetUserInfo: Invalid User ID: {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid User ID"));
        }

        var user = await _userRepository.FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            _logger.LogError("IdentityGrpcService.GetUserInfo: User not found: {UserId}", request.UserId);
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        var permissions = await _rolePermissionRepository.GetAllAsync(x => x.Permission.Name, x => x.RoleId == user.RoleId);


        return new GetUserInfoResponse { NameEn = user.NameEn, NameAr = user.NameAr, UserType = user.UserType.ToString().ToLower(), IsActive = user.IsActive, Permissions = { permissions } };
    }


}
