using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Noticeboard.Admin
{
    public partial class Admin : System.Web.UI.Page
    {
        public void Redirect_Command(object sender, CommandEventArgs e)
        {
            this.Response.Redirect(e.CommandArgument.ToString());
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}