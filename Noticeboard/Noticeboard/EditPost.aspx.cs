using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Noticeboard.Models;
using System.Web.ModelBinding;
using System.IO;
using System.Text;
using System.Web.UI.HtmlControls;
using Error_Handler_Control;

namespace Noticeboard
{
    public partial class EditPost : System.Web.UI.Page
    {
        private const string ALLOWED_FILENAME_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const int FILENAME_LENGTH = 16;

        private const string CHANGE_POST_HEADLINE = "Edit Post";

        private const string ADD_NEW_POST_HEADLINE = "Create New Post";

        private HashSet<string> allowedFileExtensions =
            new HashSet<string> { "jpg", "jpeg", "png", "gif" };

        private int postId;

        private bool isNew;

        private Random randomizer = new Random();

        private string GenerateRandomFileName()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < FILENAME_LENGTH; i++)
            {
                sb.Append(ALLOWED_FILENAME_CHARS[randomizer.Next(0, ALLOWED_FILENAME_CHARS.Length)]);
            }

            return sb.ToString();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!this.Page.IsPostBack)
            //{
            //    var db = new NoticeboardEntities();
            //    var dropDown = (DropDownList)FindControlRecursive(this, "DropDownListCategories");
            //    dropDown.DataSource = db.Categories.ToList();
            //    dropDown.DataBind();
            //}
            this.postId = Convert.ToInt32(Request.Params["id"]);

            this.isNew = (this.postId == 0);

            Control headline = this.FormViewEditPost.FindControl("PostHeadline");

            var headlineControl = headline as HtmlGenericControl;
            if (headlineControl != null)
            {
                if (this.isNew)
                {
                    headlineControl.InnerText = Server.HtmlEncode(ADD_NEW_POST_HEADLINE);
                }
                else
                {
                    headlineControl.InnerText = Server.HtmlEncode(CHANGE_POST_HEADLINE);
                }
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            var db = new NoticeboardEntities();
            var dropDown = (DropDownList)FindControlRecursive(this, "DropDownListCategories");
            dropDown.DataSource = db.Categories.ToList();
            dropDown.DataBind();
        }

        public Noticeboard.Models.Post FormViewEditPost_GetItem([QueryString]int? id)
        {
            var db = new NoticeboardEntities();
            Noticeboard.Models.Post post = new Models.Post();
            if (!this.isNew)
            {
                post = db.Posts.Find(this.postId);
            }
            else
            {
                post.CategoryId = db.Categories.FirstOrDefault().CategoryId;
            }
            return post;
        }

        public void FormViewEditPost_UpdateItem(int PostId)
        {
            var db = new NoticeboardEntities();

            if (!this.isNew)
            {
                Noticeboard.Models.Post post = db.Posts.Find(PostId);
                if (post == null)
                {
                    ModelState.AddModelError("", String.Format(
                        "Post with id {0} was not found", PostId));
                    return;
                }

                int categoryId = Convert.ToInt32(
                    ((DropDownList)FindControlRecursive(this, "DropDownListCategories"))
                    .SelectedValue);
                post.CategoryId = categoryId;

                var fileUploadControl = (FileUpload)this.FormViewEditPost.FindControl("FileUploadControl");
                if (fileUploadControl.HasFile)
                {
                    string fileExtension = Path.GetExtension(fileUploadControl.FileName).Substring(1);
                    if (!allowedFileExtensions.Contains(fileExtension.ToLowerInvariant()))
                    {
                        throw new ArgumentException("File type not allowed!");
                    }

                    string filename =
                        this.GenerateRandomFileName() + '.' + fileExtension;
                    string fullPath = Server.MapPath("~/Uploaded_Files/") + filename;

                    fileUploadControl.SaveAs(fullPath);

                    Noticeboard.Models.File uploadedFile = new Noticeboard.Models.File
                    {
                        Path = fullPath,
                        Post = post
                    };

                    post.Files.Add(uploadedFile);
                    db.Files.Add(uploadedFile);
                    //try
                    db.SaveChanges();
                }

                TryUpdateModel(post);
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SaveChanges();
                        ErrorSuccessNotifier.AddSuccessMessage("Post edited succesfully");
                    }
                    catch (Exception ex)
                    {
                        ErrorSuccessNotifier.AddErrorMessage(ex);
                    }
                    
                    Response.Redirect("Default.aspx");
                }
            }
            else
            {
                string title = ((TextBox)FindControlRecursive(this, "TextBoxPostTitle")).Text;
                string content = ((TextBox)FindControlRecursive(this, "TextBoxPostContent")).Text;
                AspNetUser user = db.AspNetUsers.FirstOrDefault(u => u.UserName == this.User.Identity.Name);
                int categoryId = Convert.ToInt32(
                    ((DropDownList)FindControlRecursive(this, "DropDownListCategories"))
                    .SelectedValue);

                var post = new Noticeboard.Models.Post
                {
                    PostDate = DateTime.Now,
                    CategoryId = categoryId,
                    Title = title,
                    Content = content,
                    AspNetUser = user
                };

                var fileUploadControl = (FileUpload)this.FormViewEditPost.FindControl("FileUploadControl");
                if (fileUploadControl.HasFile)
                {
                    string fileExtension = Path.GetExtension(fileUploadControl.FileName).Substring(1);
                    if (!this.allowedFileExtensions.Contains(fileExtension.ToLower()))
                    {
                        throw new ArgumentException("File type not allowed!");
                    }

                    string filename = this.GenerateRandomFileName() + '.' + fileExtension;
                    string fullPath = Server.MapPath("~/Uploaded_Files/") + filename;
                    fileUploadControl.SaveAs(fullPath);

                    Noticeboard.Models.File uploadedFile = new Noticeboard.Models.File
                    {
                        Path = fullPath,
                        Post = post
                    };

                    post.Files.Add(uploadedFile);
                    db.Files.Add(uploadedFile);
                }

                db.Posts.Add(post);
                try
                {
                    db.SaveChanges();
                    ErrorSuccessNotifier.AddSuccessMessage("Post added successfully");
                  
                }
                catch (Exception ex)
                {
                    ErrorSuccessNotifier.AddErrorMessage(ex);
                }

               // Response.Redirect("Post.aspx?id=" + post.PostId);
                Response.Redirect("Default.aspx");

            }
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        public Control FindControlRecursive(Control control, string id)
        {
            if (control == null) return null;
            Control ctrl = control.FindControl(id);

            if (ctrl == null)
            {
                foreach (Control child in control.Controls)
                {
                    ctrl = FindControlRecursive(child, id);

                    if (ctrl != null) break;
                }
            }
            return ctrl;
        }
    }
}