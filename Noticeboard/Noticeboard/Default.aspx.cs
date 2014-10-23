using Noticeboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Noticeboard
{
    public partial class _Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public IQueryable<Noticeboard.Models.Post> PostsList_GetData()
        {
            NoticeboardEntities context = new NoticeboardEntities();

            var posts = context.Posts;

            return posts.OrderByDescending(x => x.PostDate);
        }

        public IQueryable<Noticeboard.Models.Category> CategoriesList_GetData()
        {
            NoticeboardEntities context = new NoticeboardEntities();

            var categories = context.Categories;

            return categories;
        }
    }
}