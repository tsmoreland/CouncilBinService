using System;
using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using TSMoreland.ArdsBorough.Bins.Collections.Shared;
using DTO = TSMoreland.ArdsBorough.Api.DataTransferObjects;

namespace TSMoreland.ArdsBorough.Api.Infrastructure.Profiles;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BinType, DTO.BinType>()
            .ConvertUsingEnumMapping<BinType, DTO.BinType>(options =>  options.MapByName(false))
            .ReverseMap();

        CreateMap<(BinType Type, DateOnly Date), DTO.Response.BinCollectionSummary>()
            .ConstructUsing((source, context) => 
                new DTO.Response.BinCollectionSummary(
                    context.Mapper.Map<DTO.BinType>(source.Type), 
                    source.Date.ToDateTime(new TimeOnly(7, 0, 0))));
    }
}
