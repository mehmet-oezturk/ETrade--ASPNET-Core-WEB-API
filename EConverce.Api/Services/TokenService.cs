using EConverce.Api.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EConverce.Api.Services
{
    //public class TokenService
    //{
    //    public static string GenarateToken(Account account,IConfiguration configuration)
    //    {
    //        byte[] jwtKey = Encoding.UTF8.GetBytes(configuration["JwtOptions:Key"]);
    //        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(jwtKey);
    //        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    //        string issuer = "site.com";
    //        string audience = "site.com";

    //        //claimler için liste oluşturyoruz
    //        List<Claim> claims = new List<Claim>
    //                {
    //                    new Claim("Id",account.Id.ToString()),
    //                    new Claim("type",((int)account.Type).ToString()),
    //                    new Claim(ClaimTypes.Name,account.Username),

    //                    new Claim(ClaimTypes.Role,account.Type.ToString())

    //                };
    //        //claimleri oluşturdukdan sonra token oluşturuyoruz
    //        JwtSecurityToken jwtSecurityToken =
    //            new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddDays(30), signingCredentials: credentials);

    //        string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
    //        return token;
    //    }
    //}


}
