using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace ADOKaniniRazor.Pages
{
    public class StudentsModel : PageModel
    {
        public List<Student> Students { get; set; } = new();

        public string Message { get; set; }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public DateTime JoiningDate { get; set; }

        [BindProperty]
        public string SearchTerm { get; set; }

        public void OnGet()
        {
            LoadStudents();
        }

        private readonly IConfiguration _config;
        public StudentsModel(IConfiguration config)
        {
            _config = config;
        }


        public void OnPost()
        {
            string connStr = _config.GetConnectionString("NewConnection");

            // Capture form values
            SearchTerm = Request.Form["SearchTerm"];
            Name = Request.Form["Name"];
            Email = Request.Form["Email"];
            DateTime.TryParse(Request.Form["JoinDate"], out DateTime joinDate);
            JoiningDate = joinDate;

            // CASE 1: User is searching
            if (!string.IsNullOrEmpty(SearchTerm) && string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Email))
            {
                LoadStudents(SearchTerm);
                return;
            }

            // CASE 2: User is adding a student
            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email) && JoiningDate != default)
            {
                if (JoiningDate < new DateTime(1753, 1, 1))
                {
                    JoiningDate = DateTime.Now;
                }

                using SqlConnection conn = new SqlConnection(connStr);
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Students (Name, Email, Join_Date) VALUES (@n, @e, @d)", conn);
                cmd.Parameters.AddWithValue("@n", Name);
                cmd.Parameters.AddWithValue("@e", Email);
                cmd.Parameters.AddWithValue("@d", JoiningDate);
                cmd.ExecuteNonQuery();

                Message = "Student added successfully!";
            }

            // Always reload students after any action
            LoadStudents();
        }


        private void LoadStudents(string keyword = "")
        {
            string connStr = _config.GetConnectionString("NewConnection");
            using SqlConnection conn = new SqlConnection(connStr);
            conn.Open();

            string query = "SELECT * FROM Students";
            if (!string.IsNullOrEmpty(keyword))
            {
                query += " WHERE Name LIKE @search OR Email LIKE @search OR CONVERT(VARCHAR, Join_Date, 103) LIKE @search";
            }

            SqlCommand cmd = new SqlCommand(query, conn);
            if (!string.IsNullOrEmpty(keyword))
            {
                cmd.Parameters.AddWithValue("@search", "%" + keyword + "%");
            }

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Students.Add(new Student
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Email = reader["Email"].ToString(),
                    JoiningDate = Convert.ToDateTime(reader["Join_Date"]),
                    Photo = reader["Photo"] != DBNull.Value ? reader["Photo"].ToString() : null  // ✅ Add this line
                });
            }
        }




        //public void OnPost()
        //{

        //    string connStr = _config.GetConnectionString("NewConnection");
        //    using SqlConnection conn = new SqlConnection(connStr);
        //    conn.Open();


        //    SqlCommand cmd = new SqlCommand("SELECT * FROM Students", conn);

        //    SqlDataReader reader = cmd.ExecuteReader();

        //    while (reader.Read())
        //    {
        //        Students.Add(new Student
        //        {
        //            Id = Convert.ToInt32(reader["Id"]),
        //            Name = reader["Name"].ToString(),
        //            Email = reader["Email"].ToString(),
        //            JoiningDate = Convert.ToDateTime(reader["Join_Date"])
        //        });
        //    }
        //}

    }
}
