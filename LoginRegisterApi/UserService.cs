using System;
using System.Data;
using Microsoft.Data.SqlClient;
using LoginRegisterApi.Models;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;

namespace LoginRegisterApi.Services
{
    public class UserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public bool AddUser(string username, string email, string password)
        {
            // Kullanıcı zaten var mı kontrolü
            if (GetUserByUsername(username) != null || GetUserByEmail(email) != null)
                return false;

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)", connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                command.ExecuteNonQuery();
            }
            return true;
        }

        public User? GetUserByUsername(string username)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT Username, Email, PasswordHash FROM Users WHERE Username = @Username", connection);
                    command.Parameters.AddWithValue("@Username", username);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Username = reader.GetString(0),
                                Email = reader.GetString(1),
                                PasswordHash = reader.GetString(2)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata loglanabilir, şimdilik konsola yazalım
                Console.WriteLine($"GetUserByUsername Hatası: {ex.Message}");
                // İsterseniz burada özel bir hata mesajı da dönebilirsiniz
            }
            return null;
        }

        public User? GetUserByEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Username, Email, PasswordHash FROM Users WHERE Email = @Email", connection);
                command.Parameters.AddWithValue("@Email", email);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Username = reader.GetString(0),
                            Email = reader.GetString(1),
                            PasswordHash = reader.GetString(2)
                        };
                    }
                }
            }
            return null;
        }

        public bool VerifyPassword(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null) return false;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }
    }
}
