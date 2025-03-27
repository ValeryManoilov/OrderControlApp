using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OrderControlApp.Services
{
    public class AuthOptions
    {
        public const string Issuer = "OrderController";
        public const string Audience = "Audience";
        const string Key = "a-string-secret-at-least-256-bits-long";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    }
}
