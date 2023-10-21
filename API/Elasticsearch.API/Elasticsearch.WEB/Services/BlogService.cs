using Elasticsearch.WEB.Models;
using Elasticsearch.WEB.Repositories;
using Elasticsearch.WEB.ViewModel;

namespace Elasticsearch.WEB.Services
{
    public class BlogService
    {
        private readonly BlogRepository _blogRepository;
        public BlogService(BlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<bool> SaveAsync(BlogCreateViewModel model)
        {
            Blog newBlog = new Blog
            {
                UserId = Guid.NewGuid(),
                Title = model.Title,
                Content = model.Content,
                Tags = model.Tags.Split(',')
            };

            var isCreated = await _blogRepository.SaveAsync(newBlog);

            return isCreated != null;
        }
    }
}
