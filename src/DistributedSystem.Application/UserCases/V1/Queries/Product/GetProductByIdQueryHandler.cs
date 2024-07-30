using AutoMapper;
using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Contract.Abstractions.Shared;
using DistributedSystem.Contract.Services.V1.Product;
using DistributedSystem.Domain.Abstractions.Repositories;
using DistributedSystem.Domain.Exceptions;
using DistributedSystem.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DistributedSystem.Application.UserCases.V1.Queries.Product;

public class GetProductByIdQueryHandler : IQueryHandler<Query.GetProductByIdQuery, Response.ProductResponse>
{
    private readonly IRepositoryBase<Domain.Entities.Product, Guid> _productRepository;
    private readonly IRepositoryBaseDbContext<ApplicationDbContext ,Domain.Entities.Product, Guid> _productRepositoryBase;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IRepositoryBase<Domain.Entities.Product, Guid> productRepository, IRepositoryBaseDbContext<ApplicationDbContext, Domain.Entities.Product, Guid> productRepositoryBase, IMapper mapper)
    {
        _productRepository = productRepository;
        _productRepositoryBase = productRepositoryBase;
        _mapper = mapper;
    }

    public async Task<Result<Response.ProductResponse>> Handle(Query.GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id) ?? throw new ProductException.ProductNotFoundException(request.Id);
        var productBase = await _productRepositoryBase.FindByIdAsync(request.Id) ?? throw new ProductException.ProductNotFoundException(request.Id);
        var result = _mapper.Map<Response.ProductResponse>(product??productBase);
        return Result.Success(result);
    }
}