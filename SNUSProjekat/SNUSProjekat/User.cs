using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace SNUSProjekat
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string Username { get; set; }
        public string EncryptedPassword { get; set; }
        public User() { }
        public User(string username, string encryptedPassword)
        {
            Username = username;
            EncryptedPassword = encryptedPassword;
        }
    }

    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}