using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace ADOKaniniRazor.Pages.Teachers
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _config;

        public EditModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public IFormFile PhotoFile { get; set; }

        [BindProperty]
        public int Id { get; set; }

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

        [BindProperty]
        public string Photo { get; set; }

        public string Message { get; set; }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string connStr = _config.GetConnectionString("NewConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Teachers WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Id = reader.GetInt32(0);
                    FirstName = reader.GetString(1);
                    LastName = reader.GetString(2);
                    Email = reader.GetString(3);
                    Phone = reader.IsDBNull(4) ? null : reader.GetString(4);
                    Subject = reader.IsDBNull(5) ? null : reader.GetString(5);
                    HireDate = reader.GetDateTime(6);
                    Department = reader.IsDBNull(7) ? null : reader.GetString(7);
                    Photo = reader.IsDBNull(8) ? null : reader.GetString(8);
                }
                else
                {
                    return NotFound();
                }
            }

            return Page();
        }

        public IActionResult OnPost()
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

            if (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(Email))
            {
                if (HireDate < new DateTime(1753, 1, 1))
                {
                    HireDate = DateTime.Now;
                }

                string relativePath = Photo;
                if (PhotoFile != null)
                {
                    // Save new photo
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + PhotoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        PhotoFile.CopyTo(stream);
                    }
                    relativePath = "images/" + uniqueFileName;

                    // Delete old photo if it exists
                    if (!string.IsNullOrEmpty(Photo) && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Photo)))
                    {
                        System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Photo));
                    }
                }

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Teachers SET FirstName = @fn, LastName = @ln, Email = @e, Phone = @p, Subject = @s, " +
                        "HireDate = @hd, Department = @d, Photo = @ph WHERE Id = @id", conn);
                    cmd.Parameters.AddWithValue("@fn", FirstName);
                    cmd.Parameters.AddWithValue("@ln", LastName);
                    cmd.Parameters.AddWithValue("@e", Email);
                    cmd.Parameters.AddWithValue("@p", (object)Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@s", (object)Subject ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@hd", HireDate);
                    cmd.Parameters.AddWithValue("@d", (object)Department ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ph", relativePath);
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                }

                Message = "Teacher updated successfully!";
                return RedirectToPage("Index", new { Message });
            }

            Message = "Please fill in all required fields.";
            return Page();
        }
    }
}