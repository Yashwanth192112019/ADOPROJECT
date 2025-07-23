using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ADOKaniniRazor.Pages.Students
{
    public class DeleteModel : PageModel
    {
        private readonly IConfiguration _config;
        public DeleteModel(IConfiguration config) => _config = config;

        public Student Student { get; set; }

        public void OnGet(int id)
        {
            string connStr = _config.GetConnectionString("NewConnection");
            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Students WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                Student = new Student
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    JoiningDate = Convert.ToDateTime(reader["Join_Date"])
                };
            }
        }

        public IActionResult OnPost()
        {
            int id = int.Parse(Request.Form["Id"]);

            string connStr = _config.GetConnectionString("NewConnection");
            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            SqlCommand cmd = new SqlCommand("DELETE FROM Students WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            return RedirectToPage("/Students");
        }
    }
}
