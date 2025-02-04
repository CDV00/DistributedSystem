﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DistributedSystem.Domain.Abstractions.Repositories;

public interface IRepositoryBaseDbContext<TContext, TEntity, in TKey>
    where TContext : DbContext
    where TEntity : class //=> In imlementation should be Entity<TKey>
{
    Task<TEntity> FindByIdAsync(TKey id, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperty);
    Task<TEntity> FindSingleAsync(Expression<Func<TEntity, bool>>? predicate = null ,CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includeProperty);
    IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>>? predicate = null , params Expression<Func<TEntity, object>>[] includeProperty);
    void Add(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
    void RemoveMultiple(List<TEntity> entities);
}

