using WeeControl.SharedKernel.Essential.DataTransferObjects;
using WeeControl.SharedKernel.Interfaces;

namespace WeeControl.SharedKernel.Essential;

public interface IAdminOperation
{
    Task<IResponseDto> GetListOfTerritories();
    Task<IResponseDto> AddTerritory(TerritoryDto territory);
    Task<IResponseDto> EditTerritory(TerritoryDto territory);
    
    Task<IResponseDto<IEnumerable<UserDto>>> GetListOfUsers();
    Task<IResponseDto<UserDto>> GetUserDetails(string username);
    Task<IResponseDto> UpdateUser(UserDto userDto);
}