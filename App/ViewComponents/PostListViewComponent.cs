using Microsoft.AspNetCore.Mvc;
using MinTwitterApp.DTO;
using MinTwitterApp.Common;


namespace MinTwitterApp.ViewComponents
{
    public class PostListViewComponent : ViewComponent
    {
        private readonly LoginUser _loginUser;

        public PostListViewComponent(LoginUser loginUser)
        {
            _loginUser = loginUser;
        }

        public IViewComponentResult Invoke(IEnumerable<PostPageDTO> posts, int currentUserId)
        {
            ViewBag.currentUserId = currentUserId;
            ViewBag.LoginUserId = _loginUser.GetUserId();
            return View(posts);
        }
    }
}