﻿using AutoMapper;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Domain.Entities;

namespace DistributedSystem.Application.Mapper;
public class ServiceProfile : Profile {
    public ServiceProfile() {
        //v1
        CreateMap<Product, Response.ProductResponse>().ReverseMap();
        CreateMap<PagedResult<Product>, PagedResult<Response.ProductResponse>>().ReverseMap();
    }
}
