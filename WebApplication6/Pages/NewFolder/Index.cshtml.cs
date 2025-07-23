
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using WebApplication6;

namespace ADOKaniniRazor.Pages.Teachers
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public IList<Teacher> Teachers { get; set; }
        public int TeacherCount { get; set; }
        public string Message { get; set; }

        public void OnGet()
        {
            Teachers = new List<Teacher>();
            string connStr = _config.GetConnectionString("NewConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Get teacher count
                SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) FROM Teachers", conn);
                TeacherCount = (int)countCmd.ExecuteScalar();

                // Get teacher list
                SqlCommand cmd = new SqlCommand("SELECT * FROM Teachers", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Teachers.Add(new Teacher
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        Email = reader.GetString(3),
                        Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Subject = reader.IsDBNull(5) ? null : reader.GetString(5),
                        HireDate = reader.GetDateTime(6),
                        Department = reader.IsDBNull(7) ? null : reader.GetString(7),
                        Photo = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }
        }
    }
}