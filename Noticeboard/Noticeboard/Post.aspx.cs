namespace Noticeboard
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.ModelBinding;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using Error_Handler_Control;

    using Noticeboard.Models;

    public partial class Post : Page
    {
        private int postId;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.postId = Convert.ToInt32(this.Request.Params["id"]);

            using (var context = new NoticeboardEntities())
            {
                var post = (from p in context.Posts where p.PostId == this.postId select p).FirstOrDefault();

                if (post == null)
                {
                    this.Response.Redirect("/EditPost.aspx");
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
            var listviewCommentsControl = (ListView)this.FormViewPost.FindControl("ListViewComments");
            listviewCommentsControl.InsertItemPosition = InsertItemPosition.LastItem;
            if (!this.User.Identity.IsAuthenticated)
            {
                this.Response.Redirect("~/Account/Login.aspx");
            }
        }

        protected void ListViewComments_ItemInserted(object sender, ListViewInsertedEventArgs e)
        {
            var listviewCommentsControl = (ListView)this.FormViewPost.FindControl("ListViewComments");
            listviewCommentsControl.InsertItemPosition = InsertItemPosition.None;
        }

        public object FormViewPost_GetItem([QueryString] int id)
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
            if (control == null)
            {
                return null;
            }

            var ctrl = control.FindControl(id);

            if (ctrl == null)
            {
                // search the children
                foreach (Control child in control.Controls)
                {
                    ctrl = this.FindControlRecursive(child, id);

                    if (ctrl != null)
                    {
                        break;
                    }
                }
            }

            return ctrl;
        }

        protected void ButtonEditComment_Command(object sender, CommandEventArgs e)
        {
            var db = new NoticeboardEntities();
            var commentId = Convert.ToInt32(e.CommandArgument);
            if (!this.User.Identity.IsAuthenticated)
            {
                this.Response.Redirect("~/Account/Login.aspx");
            }
            else if (this.User.Identity.Name != db.Comments.Find(commentId).AspNetUser.UserName)
            {
                ErrorSuccessNotifier.AddInfoMessage("You don't have permission to edit this comment");
                this.Response.Redirect("Post.aspx?id=" + this.postId);
            }
        }

        public void ListViewComments_UpdateItem(int? CommentId)
        {
            try
            {
                var db = new NoticeboardEntities();
                Comment item = null;
                item = db.Comments.Find(CommentId);
                if (item == null)
                {
                    this.ModelState.AddModelError(string.Empty, string.Format("Item with id {0} was not found", CommentId));
                    return;
                }

                this.TryUpdateModel(item);
                if (this.ModelState.IsValid)
                {
                    db.SaveChanges();
                    ErrorSuccessNotifier.AddSuccessMessage("Comment edited successfully");
                }

                var uPanel = (UpdatePanel)this.FindControlRecursive(this, "UpdatePanelComments");
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
                var cont = ((TextBox)this.FindControlRecursive(this, "TextBoxComment")).Text;
                if (cont.Length >= 5000)
                {
                    var ex = new Exception("Comment must be less than 5000 symbols!");
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
                    ErrorSuccessNotifier.AddSuccessMessage("Comment created successfully");
                }
                catch (Exception ex)
                {
                    ErrorSuccessNotifier.AddErrorMessage(ex.Message);
                }

                var uPanel = (UpdatePanel)this.FindControlRecursive(this, "UpdatePanelComments");
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
                this.Response.Redirect("~/Account/Login.aspx");
            }
            else if (!(this.User.Identity.Name == db.Posts.Find(this.postId).AspNetUser.UserName))
            {
                ErrorSuccessNotifier.AddInfoMessage("You don't have permission to edit this post");
                this.Response.Redirect("Post.aspx?id=" + this.postId);
            }

            this.Response.Redirect("EditPost.aspx?id=" + this.postId);
        }

        public void FormViewPost_DeleteItem(int? PostId)
        {
            try
            {
                var db = new NoticeboardEntities();
                if (!this.User.Identity.IsAuthenticated)
                {
                    this.Response.Redirect("~/Account/Login.aspx");
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
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorSuccessNotifier.AddErrorMessage(ex);
            }

            this.Response.Redirect("Default.aspx");
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public void ListViewComments_DeleteItem(int? CommentId)
        {
            var db = new NoticeboardEntities();
            if (!this.User.Identity.IsAuthenticated)
            {
                this.Response.Redirect("~/Account/Login.aspx");
            }
            else if (this.User.IsInRole("admin"))
            {
                try
                {
                    var comment = db.Comments.Find(CommentId);
                    db.Comments.Remove(comment);
                    db.SaveChanges();
                    ErrorSuccessNotifier.AddSuccessMessage("Comment successfully deleted");
                }
                catch (Exception ex)
                {
                    ErrorSuccessNotifier.AddErrorMessage(ex.Message);
                }
            }
            else
            {
                ErrorSuccessNotifier.AddInfoMessage("You don't have permission to delete this comment");
                this.Response.Redirect("Post.aspx?id=" + this.postId);
            }
        }
    }
}