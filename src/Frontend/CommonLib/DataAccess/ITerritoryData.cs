using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using WeeControl.SharedKernel.DtosV1.Territory;

namespace CommonLib.DataAccess
{
    public interface ITerritoryData
    {
        [Get("/Territory")]
        Task<List<TerritoryDto>> GetTerritories();
        
        
    }
}