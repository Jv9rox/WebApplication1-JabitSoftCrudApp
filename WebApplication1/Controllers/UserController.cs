using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand("Select * From users", conn);
            conn.Open();
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            List<User> users = new List<User>();
            if (sqlDataReader.HasRows)
            {
                while (sqlDataReader.Read())
                {
                    User user = new User
                    {
                        Id = (string)sqlDataReader["Id"],
                        Name = (string)sqlDataReader["Name"],
                        Email = (string)sqlDataReader["Email"],
                        CreatedDate = (DateTime)sqlDataReader["CreatedDate"]
                    };
                    users.Add(user);
                }
            }
            conn.Close();
            return View(users);
        }
        public IActionResult Delete(int id)
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand($"Delete From users where Id = {id}", conn);
            conn.Open();
            int numberOfRows = cmd.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(User user) 
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            string query = "INSERT INTO users (Name, Email, Password, CreatedDate) VALUES (Name, @Email, @Password, @CreatedDate)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@CreatedDate", user.CreatedDate);

                int numberOfRows = cmd.ExecuteNonQuery();
            }
            conn.Close();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Update(int id)
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand($"Select * From users where Id = {id}", conn);
            conn.Open();
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            List<User> users = new List<User>();
            while (sqlDataReader.HasRows)
            {
                if (sqlDataReader.Read())
                {
                    User user = new User
                    {
                        Id = (string)sqlDataReader["Id"],
                        Name = (string)sqlDataReader["Name"],
                        Email = (string)sqlDataReader["Email"],
                        CreatedDate = (DateTime)sqlDataReader["CreatedDate"]
                    };
                    users.Add(user);
                }
            }
            conn.Close();

            return View(users[0]);
        }
        [HttpPost]
        public IActionResult Update(int id, User user)
        {
            SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            SqlCommand cmd = new SqlCommand($"Update users SET Name = {user.Name}, Email = {user.Email}, Password = {user.Password} where Id = {id}", conn);
            conn.Open();
            int numberOfRows = cmd.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction(nameof(Index));
        }
    }
}
