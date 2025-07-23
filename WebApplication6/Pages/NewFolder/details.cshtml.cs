
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ADOKaniniRazor.Pages.Teachers
{
    public class DetailsModel : PageModel
    {
        private readonly IConfiguration _config;

        public DetailsModel(IConfiguration config)
        {
            _config = config;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Subject { get; set; }
        public DateTime HireDate { get; set; }
        public string Department { get; set; }
        public string Photo { get; set; }

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
    }
}