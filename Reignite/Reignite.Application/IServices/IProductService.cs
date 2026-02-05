using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Core.Entities;


namespace Reignite.Application.IServices
{
    public interface IProductService:IService<Product,ProductResponse,CreateProductRequest,UpdateProductRequest,ProductQueryFilter,int>
    {
        Task<ProductResponse> UploadImageAsync(int productId, FileUploadRequest fileRequest);
        Task<bool> DeleteImageAsync(int productId);
        Task<List<ProductResponse>> GetBestSellingAsync(int count = 5);
    }
}
