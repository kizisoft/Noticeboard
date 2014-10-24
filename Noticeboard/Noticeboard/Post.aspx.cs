using Error_Handler_Control;
using Noticeboard.Controls;
using Noticeboard.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Noticeboard
{
    using System.IO;
    using System.Web.UI.HtmlControls;

    public partial class Post : System.Web.UI.Page
    {
        private int postId;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.postId = Convert.ToInt32(Request.Params["id"]);

            using (NoticeboardEntities context = new NoticeboardEntities())
            {
                var post = (from p in context.Posts
                            where p.PostId == this.postId
                            select p).FirstOrDefault();

                if (post == null)
                {
                    Response.Redirect("/EditPost.aspx");
                }

                var imagePlaceholder = this.FormViewPost.FindControl("ImagePlaceholder");
                if (post.Files.Count != 0)
                {
                    foreach (var image in post.Files)
                    {
                        var imageControl = new Image
                        {
                            ImageUrl = "~/Uploaded_Files/" + Path.GetFileName(image.Path),
                            Width = 300
                        };

                        imagePlaceholder.Controls.Add(imageControl);
                    }

                }
            }
        }

        protected void ButtonInsertComment_Click(object sender, EventArgs e)
        {
            ListView listviewCommentsControl = (ListView)this.FormViewPost.FindControl("ListViewComments");
            listviewCommentsControl.InsertItemPosition = InsertItemPosition.LastItem;
            if (!this.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
        }

        protected void ListViewComments_ItemInserted(object sender, ListViewInsertedEventArgs e)
        {
            ListView listviewCommentsControl = (ListView)this.FormViewPost.FindControl("ListViewComments");
            listviewCommentsControl.InsertItemPosition = InsertItemPosition.None;
        }

        public object FormViewPost_GetItem([QueryString]int id)
        {
            try
            {
                var db = new NoticeboardEntities();
                var post = db.Posts.Find(id);
                return post;
            }
            catch (Exception ex)
            {
                ErrorSuccessNotifier.AddErrorMessage(ex);
                return null;
            }
        }

        public IQueryable<Comment> ListViewComments_GetData()
        {
            var db = new NoticeboardEntities();
            var post = db.Posts.Include("Comments").FirstOrDefault(p => p.PostId == this.postId);
            if (post.Comments == null)
            {
                return new List<Comment>().AsQueryable();
            }
            return post.Comments.OrderBy(c => c.CommentDate).AsQueryable();
        }

        public Control FindControlRecursive(Control control, string id)
        {
            if (control == null) return null;
            //try to find the control at the current level
            Control ctrl = control.FindControl(id);

            if (ctrl == null)
            {
                //search the children
                foreach (Control child in control.Controls)
                {
                    ctrl = FindControlRecursive(child, id);

                    if (ctrl != null) break;
                }
            }
            return ctrl;
        }

        protected void ButtonEditComment_Command(object sender, CommandEventArgs e)
        {
            var db = new NoticeboardEntities();
            int commentId = Convert.ToInt32(e.CommandArgument);
            if (!this.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else if (!(this.User.Identity.Name == db.Comments.Find(commentId).AspNetUser.UserName))
            {
                ErrorSuccessNotifier.AddInfoMessage("You don't have permission to edit this comment");
                Response.Redirect("Post.aspx?id=" + this.postId);
            }
        }

        public void ListViewComments_UpdateItem(int? CommentId)
        {
            try
            {
                var db = new NoticeboardEntities();
                Noticeboard.Models.Comment item = null;
                item = db.Comments.Find(CommentId);
                if (item == null)
                {
                    ModelState.AddModelError("", String.Format("Item with id {0} was not found", CommentId));
                    return;
                }
                TryUpdateModel(item);
                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                    ErrorSuccessNotifier.AddSuccessMessage("Comment edited sucessfully");
                }

                var uPanel = (UpdatePanel)FindControlRecursive(this, "UpdatePanelComments");
                uPanel.Update();
            }
            catch (Exception ex)
            {
                ErrorSuccessNotifier.AddErrorMessage(ex);
            }
        }

        public void ListViewComments_InsertItem()
        {
            try
            {
                var db = new NoticeboardEntities();
                var user = db.AspNetUsers.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
                var cont = ((TextBox)FindControlRecursive(this, "TextBoxComment")).Text;
                if (cont.Length >= 5000)
                {
                    Exception ex = new Exception("Comment must be less than 5000 symbols!");
                    ErrorSuccessNotifier.AddErrorMessage(ex);
                    return;
                }
                var comment = new Comment
                {
                    Content = cont,
                    PostId = this.postId,
                    AspNetUser = user,
                    CommentDate = DateTime.Now
                };
                db.Comments.Add(comment);
                try
                {
                    db.SaveChanges();
                    ErrorSuccessNotifier.AddSuccessMessage("Commment created successfully");
                }
                catch (Exception ex)
                {
                    ErrorSuccessNotifier.AddErrorMessage(ex.Message);
                }

                var uPanel = (UpdatePanel)FindControlRecursive(this, "UpdatePanelComments");
                uPanel.Update();
            }
            catch (Exception ex)
            {
                ErrorSuccessNotifier.AddErrorMessage(ex);
            }
        }

        protected void ButtonEditPost_Command(object sender, CommandEventArgs e)
        {
            var db = new NoticeboardEntities();
            if (!this.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else if (!(this.User.Identity.Name == db.Posts.Find(this.postId).AspNetUser.UserName))
            {
                ErrorSuccessNotifier.AddInfoMessage("You don't have permission to edit this post");
                Response.Redirect("Post.aspx?id=" + this.postId);
            }
            Response.Redirect("EditPost.aspx?id=" + this.postId);
        }

        public void FormViewPost_DeleteItem(int? PostId)
        {
            try
            {
                var db = new NoticeboardEntities();
                if (!this.User.Identity.IsAuthenticated)
                {
                    Response.Redirect("~/Account/Login.aspx");
                }
                else if (this.User.IsInRole("admin"))
                {
                    var post = db.Posts.Find(this.postId);
                    db.Comments.RemoveRange(post.Comments);
                    db.Posts.Remove(post);
                    db.SaveChanges();
                    ErrorSuccessNotifier.AddSuccessMessage("Post successfully deleted");
                }
                else
                {
                    ErrorSuccessNotifier.AddInfoMessage("You don't have permission to delete this post");
                    //Response.Redirect("Post.aspx?id=" + this.postId);
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorSuccessNotifier.AddErrorMessage(ex);
            }

            Response.Redirect("Default.aspx");
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public void ListViewComments_DeleteItem(int? CommentId)
        {
            var db = new NoticeboardEntities();
            if (!this.User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Account/Login.aspx");
            }
            else if (this.User.IsInRole("admin"))
            {
                try
                {
                    var comment = db.Comments.Find(CommentId);
                    db.Comments.Remove(comment);
                    db.SaveChanges();
                    ErrorSuccessNotifier.AddSuccessMessage("Comment succesfully deleted");
                }
                catch (Exception ex)
                {
                    ErrorSuccessNotifier.AddErrorMessage(ex.Message);
                }
            }
            else
            {
                ErrorSuccessNotifier.AddInfoMessage("You don't have permission to delete this comment");
                Response.Redirect("Post.aspx?id=" + this.postId);
            }
        }
    }
}