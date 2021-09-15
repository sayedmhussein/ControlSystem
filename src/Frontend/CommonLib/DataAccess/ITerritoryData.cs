using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using WeeControl.SharedKernel.DtosV1.Territory;

namespace WeeControl.Frontend.CommonLib.DataAccess
{
    [Headers("Authorization: Bearer")]
    public interface ITerritoryData
    {
        [Get("/Territory")]
        Task<List<TerritoryDto>> GetTerritories();
        
        
    }
}