using AutoMapper;
using LocalChachaAdminApi.Core.Models;
using LocalChachaAdminApi.Core.Models.DTO;
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
            CreateMap<SuggestedItemResultModel, SuggestedItemResult>()
                .ForMember(dest => dest.SuggestedItems, opt => opt.MapFrom(src => src.SuggestedItemModels))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SuggestedItemResult, SuggestedItemResultModel>()
                .ForMember(dest => dest.SuggestedItemModels, opt => opt.MapFrom(src => src.SuggestedItems))
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SuggestedItemModel, SuggestedItem>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SuggestedItem, SuggestedItemModel>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<CsvSuggestedItemModel, SuggestedItem>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SuggestedItem, CsvSuggestedItemModel>()
              .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SearchFilterModel, SearchFilter>()
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SearchFilter, SearchFilterModel>()
              .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<EmailModel, Email>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}