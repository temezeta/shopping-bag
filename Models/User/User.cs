namespace shopping_bag.Models.User
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenCreatedAt { get; set; }
        public DateTime TokenExpiresAt { get; set; }
        public List<UserRole> Roles { get; set; } = new List<UserRole>();

    }
}
