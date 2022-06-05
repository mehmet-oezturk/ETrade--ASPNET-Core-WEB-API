using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyServices
{
    public class TokenService
    {
       // public static string GenarateToken(Account account, IConfiguration configuration) ESKİ
        public static string GenarateToken(string jwtKey,DateTime expires,IEnumerable<Claim> claims, string issuer = "site.com", string audience = "site.com")
        {   // byte[] jwtKey = Encoding.UTF8.GetBytes(configuration["JwtOptions:Key"]);  ESKİ
            byte[] key = Encoding.UTF8.GetBytes(jwtKey);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            

            //claimler için liste oluşturyoruz
            //List<Claim> claims = new List<Claim>
            //        {
            //            new Claim("Id",account.Id.ToString()),
            //            new Claim("type",((int)account.Type).ToString()),
            //            new Claim(ClaimTypes.Name,account.Username),

            //            new Claim(ClaimTypes.Role,account.Type.ToString())

            //        };
            //claimleri oluşturdukdan sonra token oluşturuyoruz
            JwtSecurityToken jwtSecurityToken =
                new JwtSecurityToken(issuer, audience, claims, expires: expires, signingCredentials: credentials);

            string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            return token;
        }
    }
}
