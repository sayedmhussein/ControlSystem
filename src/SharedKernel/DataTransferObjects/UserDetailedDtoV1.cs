using WeeControl.SharedKernel.DataTransferObjects.Admin;

namespace WeeControl.SharedKernel.DataTransferObjects;

public class UserDetailedDtoV1 : UserDtoV1
{
    public IEnumerable<ClaimDto> Claims { get; set; }
    public IEnumerable<SessionDto> Sessions { get; set; }
    
}