﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace GadgetFox
{
    public partial class EditDefaultCard : System.Web.UI.Page
    {
        private int addressId = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userID"] != null)
                {
                   String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                    SqlConnection myConnection = new SqlConnection(myConnectionString);
         
                    try
                    {
                        myConnection.Open();
                        SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[Addresses], [GadgetFox].[dbo].[ZipCodes] where Addresses.Zip = ZipCodes.Zip and EmailID=@EmailID and IsProfileAddress=@IsProfileAddress", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        cmd.Parameters.AddWithValue("@IsProfileAddress", true);
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            addressId = Int16.Parse(dr["AddressID"].ToString());
                            address1TB.Text = dr["Address Line1"].ToString();
                            address2TB.Text = dr["Address Line2"].ToString();
                            cityTB.Text = dr["City"].ToString();
                            countryDL.Text = dr["Country"].ToString();
                            stateDL.Text = dr["State"].ToString();
                            if (dr["Zip"] != null)
                                zipcodeTB.Text = dr["Zip"].ToString();
                        }
                    }
                    catch (SqlException ex)
                    {
                        Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }
        }

        protected void saveButton_Clicked(object sender, EventArgs e)
        {
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                // Is there a credit card in the database
                myConnection.Open();
                SqlCommand cmd1 = new SqlCommand("Select COUNT(*) from CCDetails where EmailID=@EmailID", myConnection);
                cmd1.Parameters.AddWithValue("@EmailID", Session["userID"]);
                int idRows = (int)cmd1.ExecuteScalar();
                if (idRows == 0)
                {
                    // Insert new address into database
                    SqlCommand cmd2 = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[ZipCodes] ([Zip],[City],[State],[Country])" +
                        " VALUES(@Zip,@City,@State,@Country)", myConnection);
                    cmd2.Parameters.AddWithValue("@City", cityTB.Text);
                    cmd2.Parameters.AddWithValue("@State", stateDL.Text);
                    cmd2.Parameters.AddWithValue("@Country", countryDL.Text);
                    cmd2.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                    int rc = cmd2.ExecuteNonQuery();
                    if (rc > 0)
                    {
                        returnLabel.Text = "Your credit card was successfully saved";
                        saveButton.Visible = false;
                        cancelButton.Text = "Close";
                    }
                    else
                    {
                        returnLabel.Text = "Failed to save your credit card. Please try again later!";
                    }
                }
                else
                {
                    // Update credit card in database
                    SqlCommand cmd = new SqlCommand("Update Addresses set [Address Line1]=@AddressLine1, [Address Line2]=@AddressLine2, Zip=@Zip where " +
                        "EmailID=@EmailID", myConnection);
                    cmd.Parameters.AddWithValue("@AddressLine1", address1TB.Text);
                    cmd.Parameters.AddWithValue("@AddressLine2", address2TB.Text);
                    cmd.Parameters.AddWithValue("@Zip", zipcodeTB.Text);
                    cmd.Parameters.AddWithValue("@EmailID", Session["userID"]);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 1)
                    {
                        Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Information Saved successfully')</SCRIPT>");
                        Response.Redirect("~/Home.aspx");
                    }
                }
            }
            catch (SqlException ex)
            {
                Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
            }
            finally
            {
                myConnection.Close();
            }
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /**
         * Generate the next credit card Id
         */
        public int getNextCardId()
        {
            int nextId = 0;
            int id;
            String conStr = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection con = new SqlConnection(conStr);

            SqlCommand cmd = new SqlCommand("Select CCID from [GadgetFox].[dbo].[CCDetails]", con);

            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                id = Convert.ToInt16(dr["CCID"].ToString());
                if (id > nextId)
                {
                    nextId = id;
                }
            }
            con.Close();

            return nextId + 1;
        }
    }
}