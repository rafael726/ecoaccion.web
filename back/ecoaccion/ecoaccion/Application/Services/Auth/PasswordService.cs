﻿using ecoaccion.Core.Interfaces.Services.Auth;

namespace ecoaccion.Application.Services.Auth
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword( string password )
        {
            return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        }

        public bool VerifyPassword( string password, string hash )
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
