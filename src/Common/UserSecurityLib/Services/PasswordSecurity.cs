using System;
using System.Security.Cryptography;
using System.Text;
using WeeControl.Common.UserSecurityLib.Interfaces;

namespace WeeControl.Common.UserSecurityLib.Services;

public class PasswordSecurity : IPasswordSecurity
{
    public string Hash(string password)
    {
        using var md5 = MD5.Create();
        var md5data = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
        return Convert.ToBase64String(md5data);
    }

    public bool Verify(string password, string storedPassword)
    {
        return storedPassword == Hash(password);
    }
}