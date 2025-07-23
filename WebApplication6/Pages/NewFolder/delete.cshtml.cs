
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ADOKaniniRazor.Pages.Teachers
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _config;

        public DeleteModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public int Id { get; set; }
        [BindProperty]
        public string FirstName { get; set; }
        [BindProperty]
        public string LastName { get; set; }
        [BindProperty]
        public string Email { get; set; }
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
                SqlCommand cmd = new SqlCommand("SELECT Id, FirstName, LastName, Email, Photo FROM Teachers WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    Id = reader.GetInt32(0);
                    FirstName = reader.GetString(1);
                    LastName = reader.GetString(2);
                    Email = reader.GetString(3);
                    Photo = reader.IsDBNull(4) ? null : reader.GetString(4);
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
            string photoPath = null;

            // Retrieve photo path to delete the file
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT Photo FROM Teachers WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", Id);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    photoPath = reader.IsDBNull(0) ? null : reader.GetString(0);
                }
            }

            // Delete the teacher record
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Teachers WHERE Id = @id", conn);
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.ExecuteNonQuery();
            }

            // Delete the photo file if it exists
            if (!string.IsNullOrEmpty(photoPath) && System.IO.File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoPath)))
            {
                System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", photoPath));
            }

            Message = "Teacher deleted successfully!";
            return RedirectToPage("Index", new { Message });
        }
    }
}