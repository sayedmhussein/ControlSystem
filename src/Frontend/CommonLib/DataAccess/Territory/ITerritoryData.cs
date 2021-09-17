using System.Collections.Generic;
using System.Threading.Tasks;
using WeeControl.SharedKernel.DtosV1.Territory;

namespace WeeControl.Frontend.CommonLib.DataAccess.Territory
{
    public interface ITerritoryData
    {
        Task<List<TerritoryDto>> GetTerritories();
        
        
    }
}