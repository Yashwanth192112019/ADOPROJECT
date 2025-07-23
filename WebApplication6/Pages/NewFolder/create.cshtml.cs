
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace ADOKaniniRazor.Pages.Teachers
{
    public class CreateModel : PageModel
    {
        private readonly IConfiguration _config;

        public CreateModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public IFormFile PhotoFile { get; set; }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public string Subject { get; set; }

        [BindProperty]
        public DateTime HireDate { get; set; }

        [BindProperty]
        public string Department { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            try
            {
                string connStr = _config.GetConnectionString("NewConnection");
                FirstName = Request.Form["FirstName"];
                LastName = Request.Form["LastName"];
                Email = Request.Form["Email"];
                Phone = Request.Form["Phone"];
                Subject = Request.Form["Subject"];
                Department = Request.Form["Department"];
                DateTime.TryParse(Request.Form["HireDate"], out DateTime hireDate);
                HireDate = hireDate;

                if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) &&
                    !string.IsNullOrEmpty(Email) && PhotoFile != null)
                {
                    if (HireDate < new DateTime(1753, 1, 1))
                    {
                        HireDate = DateTime.Now;
                    }

                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + PhotoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        PhotoFile.CopyTo(stream);
                    }

                    string relativePath = "images/" + uniqueFileName;

                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Teachers (FirstName, LastName, Email, Phone, Subject, HireDate, Department, Photo) " +
                        "VALUES (@fn, @ln, @e, @p, @s, @hd, @d, @ph)", conn);
                        cmd.Parameters.AddWithValue("@fn", FirstName);
                        cmd.Parameters.AddWithValue("@ln", LastName);
                        cmd.Parameters.AddWithValue("@e", Email);
                        cmd.Parameters.AddWithValue("@p", (object)Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@s", (object)Subject ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@hd", HireDate);
                        cmd.Parameters.AddWithValue("@d", (object)Department ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ph", relativePath);
                        cmd.ExecuteNonQuery();
                    }

                    Message = "Teacher added successfully!";
                    return RedirectToPage("Index", new { Message });
                }

                Message = "Please fill in all required fields.";
                return Page();
            }
            catch (SqlException ex)
            {
                Message = $"Database error: {ex.Message}";
                return Page();
            }
            catch (Exception ex)
            {
                Message = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}