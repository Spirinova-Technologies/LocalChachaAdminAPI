using AutoMapper;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Models;

namespace LocalChachaAdminApi.AutoMapperConfiguration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<MerchantRequestModel, MerchantViewModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<MerchantViewModel, MerchantRequestModel>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}