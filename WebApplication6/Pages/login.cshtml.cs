using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ADOKaniniRazor.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IConfiguration _config;

        public string Message { get; set; }

        public LoginModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnPost()
        {
            string username = Request.Form["Username"];
            string password = Request.Form["Password"];

            string connString = _config.GetConnectionString("NewConnection");

            using SqlConnection conn = new SqlConnection(connString);
            conn.Open();

            SqlCommand cmd = new SqlCommand("SELECT * FROM Admin WHERE Username=@u AND Password=@p", conn);
            cmd.Parameters.AddWithValue("@u", username);
            cmd.Parameters.AddWithValue("@p", password);

            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                Response.Redirect("/Students");
            }
            else
            {
                Message = "Invalid login credentials.";
            }
        }
    }
}
