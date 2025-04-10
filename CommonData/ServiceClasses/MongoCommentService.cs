using CommonData.MongoModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonData.ServiceClasses
{    
    public class MongoCommentService
    {
        private readonly IMongoCollection<CommentsMongo> _comments;

        public MongoCommentService(IOptions<MongoDbSettings> settings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _comments = database.GetCollection<CommentsMongo>(settings.Value.CommentsCollectionName);
        }

        public async Task CommentsAsync(CommentsMongo comments)
        {
            await _comments.InsertOneAsync(comments);
        }

        public async Task<List<CommentsMongo>> GetCommentsByBlogIdAsync(int blogId)
        {
            return await _comments.Find(c => c.BlogId == blogId).ToListAsync();
        }
    }
}
