﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GadgetFox
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Null our session variables
            Session["user"] = null;
            Session["userID"] = null;
            Session["userRole"] = null;

            // Go back to home
            Response.Redirect("~/Home.aspx");
        }
    }
}