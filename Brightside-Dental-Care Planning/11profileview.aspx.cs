﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Brightside_Dental_Care_Planning
{
    public partial class WebForm6 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Retrieve the Patient_Id from session
                if (Session["Patient_Id"] != null)
                {
                    int patientId = Convert.ToInt32(Session["Patient_Id"]);
                    LoadProfileData(patientId);
                }
                else
                {
                    // If no Patient_Id found in session, redirect to login
                    Response.Redirect("1login.aspx");
                }
            }
        }

        private void LoadProfileData(int patientId)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            string query = @"
                SELECT p.first_name, p.last_name, p.phone_number, p.Date_of_Birth, p.Gender, 
                       a.Street, a.City, a.Province
                FROM Profile p
                LEFT JOIN Address a ON p.Patient_Id = a.Patient_Id
                WHERE p.Patient_Id = @PatientId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientId", patientId);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            // Populate the labels with the retrieved profile data
                            FirstNameLabel.Text = reader["first_name"].ToString();
                            LastNameLabel.Text = reader["last_name"].ToString();
                            PhoneNumberLabel.Text = reader["phone_number"].ToString();
                            DateOfBirthLabel.Text = Convert.ToDateTime(reader["Date_of_Birth"]).ToString("dd MMM yyyy");
                            GenderLabel.Text = reader["Gender"].ToString();

                            // Store first and last names in session
                            Session["FirstName"] = reader["first_name"].ToString();
                            Session["LastName"] = reader["last_name"].ToString();

                            // Construct the address if any field is present
                            string street = reader["Street"] != DBNull.Value ? reader["Street"].ToString() : string.Empty;
                            string city = reader["City"] != DBNull.Value ? reader["City"].ToString() : string.Empty;
                            string province = reader["Province"] != DBNull.Value ? reader["Province"].ToString() : string.Empty;

                            AddressLabel.Text = !string.IsNullOrEmpty(street) || !string.IsNullOrEmpty(city) || !string.IsNullOrEmpty(province)
                                ? $"{street}, {city}, {province}".Trim(new char[] { ',', ' ' })
                                : "No address provided";

                            // Hide the create profile message and show the edit button
                            CreateProfileLabel.Visible = false;
                            CreateProfileButton.Visible = false; // Hide Create Profile button
                            EditProfileButton.Visible = true; // Show Edit Profile button
                        }
                        else
                        {
                            // If no profile exists
                            ProfileStatusLabel.Text = "You don't yet have a profile; press create profile to create a profile.";
                            CreateProfileButton.Visible = true; // Show Create Profile button
                            EditProfileButton.Visible = false; // Hide Edit Profile button
                        }
                    }
                    catch (Exception ex)
                    {
                        ProfileStatusLabel.Text = "An error occurred while retrieving the profile: " + ex.Message;
                    }
                }
            }
        }

        protected void EditProfileButton_Click(object sender, EventArgs e)
        {
            // Redirect to profile edit page
            Response.Redirect("19ProfilEdit.aspx");
        }

        protected void CreateProfileButton_Click(object sender, EventArgs e)
        {
            // Logic to create a new profile
            Response.Redirect("10profile.aspx"); // Assuming you want to navigate to the profile creation page
        }

        protected void MakeAppointmentButton_Click(object sender, EventArgs e)
        {
            // Check if the profile exists
            if (string.IsNullOrEmpty(FirstNameLabel.Text))
            {
                AppointmentStatusLabel.Text = "You must first create a profile before making an appointment.";
            }
            else
            {
                // Logic to navigate to appointment creation page
                Response.Redirect("5Appointment.aspx");
            }
        }
    }
}