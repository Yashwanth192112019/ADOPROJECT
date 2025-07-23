using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace ADOKaniniRazor.Pages.Students
{
    public class EditModel : PageModel
    {
        private readonly IConfiguration _config;

        public EditModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public Student Student { get; set; }

        [BindProperty]
        public IFormFile PhotoFile { get; set; } // ?? New photo file (optional)

        public void OnGet(int id)
        {
            string connStr = _config.GetConnectionString("NewConnection");

            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Students WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Student = new Student
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    JoiningDate = (DateTime)reader["Join_Date"],
                    Photo = reader["Photo"] != DBNull.Value ? reader["Photo"].ToString() : null
                };
            }
        }

        public IActionResult OnPost()
        {
            string connStr = _config.GetConnectionString("NewConnection");

            var id = int.Parse(Request.Form["Id"].ToString());
            var name = Request.Form["Name"].ToString();
            var email = Request.Form["Email"].ToString();
            DateTime.TryParse(Request.Form["JoiningDate"].ToString(), out DateTime joiningDate);


            string newPhotoPath = null;

            if (PhotoFile != null)
            {
                // ?? Save new photo
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + PhotoFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    PhotoFile.CopyTo(stream);
                }

                newPhotoPath = "images/" + uniqueFileName;
            }

            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            // ?? Update query includes Photo only if a new one is uploaded
            string query = newPhotoPath != null
                ? "UPDATE Students SET Name = @n, Email = @e, Join_Date = @d, Photo = @p WHERE Id = @id"
                : "UPDATE Students SET Name = @n, Email = @e, Join_Date = @d WHERE Id = @id";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@n", name);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@d", joiningDate);
            cmd.Parameters.AddWithValue("@id", id);

            if (newPhotoPath != null)
            {
                cmd.Parameters.AddWithValue("@p", newPhotoPath);
            }

            cmd.ExecuteNonQuery();

            return RedirectToPage("/Students");
        }
    }

}
