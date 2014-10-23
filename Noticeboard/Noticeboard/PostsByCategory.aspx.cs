using Noticeboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Noticeboard
{
    public partial class PostsByCategory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public IQueryable<Noticeboard.Models.Post> PostsList_GetData()
        {
            NoticeboardEntities context = new NoticeboardEntities();
            int id = Convert.ToInt32(this.Request.Params["id"]);
            if (id == 0)
            {
                this.Response.Redirect("~/Default.aspx");
            }

            var posts = context.Posts.Where(x => x.CategoryId == id);
            this.LabelCategoryTitle.Text = this.Server.HtmlEncode(context.Categories.Find(id).Title);

            return posts;
        }
    }
}