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
                .Map(dest => dest.ProductCategoryName, src => src.ProductCategory != null ? src.ProductCategory.Name : string.Empty)
                .Map(dest => dest.SupplierName, src => src.Supplier != null ? src.Supplier.Name : string.Empty);

            TypeAdapterConfig<ProjectReview, ProjectReviewResponse>.NewConfig()
                .Map(dest => dest.UserName, src => src.User != null
                    ? src.User.FirstName + " " + src.User.LastName
                    : string.Empty);

            TypeAdapterConfig<ProductReview, ProductReviewResponse>.NewConfig()
                .Map(dest => dest.UserName, src => src.User != null
                    ? src.User.FirstName + " " + src.User.LastName
                    : "Nepoznat korisnik")
                .Map(dest => dest.UserProfileImageUrl, src => src.User != null ? src.User.ProfileImageUrl : null)
                .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : "Nepoznat proizvod");

            TypeAdapterConfig<Project, ProjectResponse>.NewConfig()
                .Map(dest => dest.UserName, src => src.User != null ? src.User.FirstName + " " + src.User.LastName : string.Empty)
                .Map(dest => dest.HobbyName, src => src.Hobby != null ? src.Hobby.Name : string.Empty)
                .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : null)
                .Map(dest => dest.AverageRating, src => src.Reviews != null && src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0)
                .Map(dest => dest.ReviewCount, src => src.Reviews != null ? src.Reviews.Count : 0);
            TypeAdapterConfig<User, UserResponse>.NewConfig()
                .Map(dest => dest.OrderCount, src => src.Orders != null ? src.Orders.Count() : 0)
                .Map(dest => dest.ProjectCount, src => src.Projects != null ? src.Projects.Count() : 0);
        }
    }
}
