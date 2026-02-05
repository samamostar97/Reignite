using Mapster;
using Reignite.Application.DTOs.Response;
using Reignite.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Infrastructure.Mappings
{
    public static class MappingConfig
    {
        public static void Configure()
        {
            TypeAdapterConfig<Product, ProductResponse>.NewConfig()
                .Map(dest => dest.ProductCategoryName, src => src.ProductCategory.Name)
                .Map(dest => dest.SupplierName, src => src.Supplier.Name);

            TypeAdapterConfig<Project, ProjectResponse>.NewConfig()
                .Map(dest => dest.UserName, src => src.User.FirstName + " " + src.User.LastName)
                .Map(dest => dest.HobbyName, src => src.Hobby.Name)
                .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : null);
        }
    }
}
