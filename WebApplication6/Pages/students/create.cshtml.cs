using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace ADOKaniniRazor.Pages.Students
{
    public class CreateModel : PageModel
    {

        [BindProperty]
        public IFormFile PhotoFile { get; set; }


        private readonly IConfiguration _config;

        public CreateModel(IConfiguration config)
        {
            _config = config;
        }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public DateTime JoiningDate { get; set; }

        public string Message { get; set; }

        public void OnPost()
        {
            string connStr = _config.GetConnectionString("NewConnection");

            Name = Request.Form["Name"];
            Email = Request.Form["Email"];
            DateTime.TryParse(Request.Form["JoinDate"], out DateTime joinDate);
            JoiningDate = joinDate;

            if (!string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Email) && PhotoFile != null)
            {
                if (JoiningDate < new DateTime(1753, 1, 1))
                {
                    JoiningDate = DateTime.Now;
                }

                // ?? Save the image
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + PhotoFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    PhotoFile.CopyTo(stream);
                }

                // ?? Store just the relative path or file name
                string relativePath = "images/" + uniqueFileName;

                using SqlConnection conn = new SqlConnection(connStr);
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO Students (Name, Email, Join_Date, Photo) VALUES (@n, @e, @d, @p)", conn);
                cmd.Parameters.AddWithValue("@n", Name);
                cmd.Parameters.AddWithValue("@e", Email);
                cmd.Parameters.AddWithValue("@d", JoiningDate);
                cmd.Parameters.AddWithValue("@p", relativePath);
                cmd.ExecuteNonQuery();

                Message = "Student added successfully!";
            }
        }

    }
}
