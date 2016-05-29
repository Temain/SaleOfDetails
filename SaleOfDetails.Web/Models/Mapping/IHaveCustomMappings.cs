using AutoMapper;

namespace SaleOfDetails.Web.Models.Mapping
{
    public interface IHaveCustomMappings
    {
        void CreateMappings(IConfiguration configuration);
    }
}
