using DistributedSystem.Infrastructure.Consumer.Abstractions;
using DistributedSystem.Infrastructure.Consumer.Abstractions.Repositories;
using DistributedSystem.Infrastructure.Consumer.Attributes;
using DistributedSystem.Infrastructure.DependencyInjection.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace DistributedSystem.Infrastructure.Consumer.Repositoties;

public class MongoRepository<TDocument> : IMongoRepository<TDocument>
    where TDocument : IDocument
{
    private readonly IMongoCollection<TDocument> _collection;

    public MongoRepository(IMongoDbSettings settings)
    {
        var database = new MongoClient(settings.ConnectionString).GetDatabase(settings.DatabaseName);
        _collection = database.GetCollection<TDocument>(GetCollectionName(typeof(TDocument)));
    }

    private protected string GetCollectionName(Type doccumrntType)
        => ((BsonCollectionAttribute)doccumrntType.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault())?.CollectionName;

    public IQueryable<TDocument> AsQueryable()
        => _collection.AsQueryable();
    public IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filterExpression)
        => _collection.Find(filterExpression).ToEnumerable();

    public IEnumerable<TProjectd> FilterBy<TProjectd>(Expression<Func<TDocument, bool>> filterExpression, Expression<Func<TDocument, TProjectd>> projectionExpression)
        => _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
    public TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
        =>_collection.Find(filterExpression).FirstOrDefault();

    public Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
        => Task.Run(()=>_collection.Find(filterExpression).FirstOrDefaultAsync());

    //public virtual IEnumerable<TProjected> FilterBy<TProjected>(
    //    Expression<Func<TDocument, bool>> filterExpression,
    //    Expression<Func<TDocument, TProjected>> projectionExpression)
    //{
    //    return _collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
    //}

    //public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
    //{
    //    return _collection.Find(filterExpression).FirstOrDefault();
    //}

    //public virtual Task<TDocument> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    //{
    //    return Task.Run(() => _collection.Find(filterExpression).FirstOrDefaultAsync());
    //}

    public virtual TDocument FindById(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        return _collection.Find(filter).SingleOrDefault();
    }

    public virtual Task<TDocument> FindByIdAsync(string id)
    {
        return Task.Run(() =>
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            return _collection.Find(filter).SingleOrDefaultAsync();
        });
    }


    public virtual void InsertOne(TDocument document)
    {
        _collection.InsertOne(document);
    }

    public virtual Task InsertOneAsync(TDocument document)
    {
        return Task.Run(() => _collection.InsertOneAsync(document));
    }

    public void InsertMany(ICollection<TDocument> documents)
    {
        _collection.InsertMany(documents);
    }


    public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
    {
        await _collection.InsertManyAsync(documents);
    }

    public void ReplaceOne(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        _collection.FindOneAndReplace(filter, document);
    }

    public virtual async Task ReplaceOneAsync(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        await _collection.FindOneAndReplaceAsync(filter, document);
    }

    public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        _collection.FindOneAndDelete(filterExpression);
    }

    public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() => _collection.FindOneAndDeleteAsync(filterExpression));
    }

    public void DeleteById(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
        _collection.FindOneAndDelete(filter);
    }

    public Task DeleteByIdAsync(string id)
    {
        return Task.Run(() =>
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, objectId);
            _collection.FindOneAndDeleteAsync(filter);
        });
    }

    public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
    {
        _collection.DeleteMany(filterExpression);
    }

    public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() => _collection.DeleteManyAsync(filterExpression));
    }
}
