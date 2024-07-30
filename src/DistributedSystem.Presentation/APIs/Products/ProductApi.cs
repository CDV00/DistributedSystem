using Carter;
using DistributedSystem.Presentation.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using CommandV1 = DistributedSystem.Contract.Services.V1.Product;

namespace DistributedSystem.Presentation.APIs.Products;
public class ProductApi : ApiEndpoint, ICarterModule
{
    private const string BaseUrl = "/api/v{version:apiVersion}/products";
    private const string grouptName = "products";
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group1 = app.NewVersionedApi(grouptName).MapGroup(BaseUrl).HasApiVersion(1).RequireAuthorization();
        group1.MapPost(string.Empty, CreateProductsV1);
        group1.MapGet(string.Empty, GetProductsV1);
        group1.MapGet("aaa/{productId}", GetProductsByIdV1);
        group1.MapDelete("{productId}", DeleteProductsByIdV1);
        group1.MapPut("{productId}", UpdateProductsV1);

        var group2 = app.NewVersionedApi(grouptName).MapGroup(BaseUrl).HasApiVersion(2);
        group2.MapPost(string.Empty, () => Test);
        group2.MapGet(string.Empty, () => "");

    }
    #region ====== version 1 ======
    public static async Task<IResult> CreateProductsV1(ISender sender, [FromBody] CommandV1.Command.CreateProductCommand CreateProduct)
    {
        var result = await sender.Send(CreateProduct);
        if (result.IsFailure)
            return HandlerFailure(result);
        return Results.Ok(result);
    }
    public static async Task<IResult> GetProductsV1(ISender sender)
    {
        var result = await sender.Send(new CommandV1.Query.GetProductsQuery());
        return Results.Ok(result);
    }
    public static async Task<IResult> GetProductsByIdV1(ISender sender, Guid productId)
    {
        var result = await sender.Send(new CommandV1.Query.GetProductByIdQuery(productId));
        return Results.Ok(result);
    }
    public static async Task<IResult> DeleteProductsByIdV1(ISender sender, Guid productId)
    {
        var result = await sender.Send(new CommandV1.Command.DeleteProductCommand(productId));
        return Results.Ok(result);
    }
    public static async Task<IResult> UpdateProductsV1(ISender sender, Guid productId, [FromBody] CommandV1.Command.UpdateProductCommand updateProduct)
    {
        var updateProductCommand = new CommandV1.Command.UpdateProductCommand(productId, updateProduct.Name, updateProduct.Price, updateProduct.Description);
        var result = await sender.Send(updateProductCommand);
        return Results.Ok(result);
    }

    #endregion ====== version 1 ======

    public IResult Test()
    {
        var result = 1;
        result += 1;
        return Results.Ok(result);
    }
}
