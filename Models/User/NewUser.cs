﻿namespace shopping_bag.Models.User
{
    public class NewUser
    {
        public string Email { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
