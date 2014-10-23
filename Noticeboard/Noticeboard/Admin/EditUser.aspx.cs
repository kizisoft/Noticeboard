namespace Noticeboard.Admin
{
    using System;
    using System.Linq;
    using System.Web.UI.WebControls;
    using Error_Handler_Control;
    using Noticeboard.Models;

    public partial class EditUser : System.Web.UI.Page
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            var userId = this.Request.Params["userId"];

            using (var context = new NoticeboardEntities())
            {
                try
                {
                    var user = context.AspNetUsers.Find(userId);
                    this.TextBoxUsername.Text = user.UserName;
                    this.CheckBoxIsAdmin.Checked = user.AspNetRoles.FirstOrDefault(r => r.Name == "admin") != null;
                }
                catch (Exception ex)
                {
                    ErrorSuccessNotifier.AddErrorMessage(ex);
                }
            }
        }

        protected void ButtonSave_Click(object sender, EventArgs e)
        {
            using (var context = new NoticeboardEntities())
            {
                var userId = this.Request.Params["userId"];

                try
                {
                    var user = context.AspNetUsers.Find(userId);
                    user.UserName = this.TextBoxUsername.Text;

                    var adminRole = context.AspNetRoles.FirstOrDefault(r => r.Name == "admin");
                    if (this.CheckBoxIsAdmin.Checked && user.AspNetRoles.FirstOrDefault(r => r.Name == "admin") == null)
                    {
                        user.AspNetRoles.Add(adminRole);
                    }
                    else if (!this.CheckBoxIsAdmin.Checked && user.AspNetRoles.FirstOrDefault(r => r.Name == "admin") != null)
                    {
                        user.AspNetRoles.Remove(adminRole);
                    }

                    context.SaveChanges();

                    ErrorSuccessNotifier.AddInfoMessage("User successfully edited.");
                    ErrorSuccessNotifier.ShowAfterRedirect = true;
                    this.Response.Redirect("Users.aspx", false);
                }
                catch (Exception ex)
                {
                    ErrorSuccessNotifier.AddErrorMessage(ex);
                }
            }
        }

        protected void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("Users.aspx");
        }
    }
}