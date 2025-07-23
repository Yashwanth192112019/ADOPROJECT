using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ADOKaniniRazor.Pages
{
    public class SignupModel : PageModel
    {
        private readonly IConfiguration _config;
        public SignupModel(IConfiguration config) => _config = config;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string Message { get; set; }

        public void OnPost()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                Message = "Please enter both Username and Password.";
                return;
            }

            string connStr = _config.GetConnectionString("NewConnection");

            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Admin WHERE Username = @u", conn);
            checkCmd.Parameters.AddWithValue("@u", Username);
            int count = (int)checkCmd.ExecuteScalar();

            if (count > 0)
            {
                Message = "Username already exists. Choose another.";
                return;
            }

            SqlCommand cmd = new SqlCommand("INSERT INTO Admin (Username, Password) VALUES (@u, @p)", conn);
            cmd.Parameters.AddWithValue("@u", Username);
            cmd.Parameters.AddWithValue("@p", Password);
            cmd.ExecuteNonQuery();

            Message = "Registration successful! You can now log in.";
        }
    }
}
