using AutoMapper;
using ProductsAPI.Application.DTOs;
using ProductsAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsAPI.Application.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap <CreateProductDto,ProductDto>();
           CreateMap<UpdateProductDto,ProductDto>();


            CreateMap<Item, ItemDto>();
            CreateMap<CreateItemDto, Item>();
                CreateMap<UpdateItemDto, Item>();

        }
    }
}
