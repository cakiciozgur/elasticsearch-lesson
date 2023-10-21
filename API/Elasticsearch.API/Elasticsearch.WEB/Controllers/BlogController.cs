using Elasticsearch.WEB.Services;
using Elasticsearch.WEB.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.WEB.Controllers
{
    public class BlogController : Controller
    {
        private BlogService _blogService;

        public BlogController(BlogService blogService)
        {
            _blogService = blogService;
        }

        public IActionResult Save()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Save(BlogCreateViewModel model)
        {
            var isSuccess = await _blogService.SaveAsync(model);
            if (!isSuccess)
            {
                TempData["result"] = "Başarısız";
                return RedirectToAction(nameof(BlogController.Save));
            }

            TempData["result"] = "Başarılı";
            return RedirectToAction(nameof(BlogController.Save));
        }
    }
}
