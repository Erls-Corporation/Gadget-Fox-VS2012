﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.SessionState;

namespace GadgetFox
{
    public partial class EmployeeHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["userID"] == null)
            {
                // Redirect user to login before doing anything else
                Response.Redirect("~/Login.aspx?redirect=EmployeeHome.aspx");
            }
            else if (Session["userID"] != null && Session["userRole"].Equals("1"))
            {
                // Redirect customers away from the employee home
                Response.Redirect("~/Home.aspx");
            }            
        }
    }
}