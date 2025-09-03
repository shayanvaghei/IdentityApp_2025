using API.DTOs.MyProfile;
using API.Models;
using API.Services.IServices;
using API.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OtpNet;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _jwtKey;
        private readonly SymmetricSecurityKey _mfaKey;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            _mfaKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["MFA:Key"]));
        }
        public string CreateJWT(AppUser user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(SD.UserId, user.Id.ToString()),
                new Claim(SD.Name, user.Name),
                new Claim(SD.UserName, user.UserName),
                new Claim(SD.Email, user.Email),
            };

            var credentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpiresInDays"])),
                SigningCredentials = credentials,
                Issuer = _config["JWT:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwt);
        }
        public QrCodeDto GenerateQrCode(string email)
        {
            var key = KeyGeneration.GenerateRandomKey(10);
            string secret = Base32Encoding.ToString(key);
            string issuer = _config["MFA:Issuer"];
            string uri = $"otpauth://totp/{issuer}:{email}?secret={secret}&issuer={issuer}&digits=6";

            return new QrCodeDto(secret, uri);
        }
        public bool ValidateCode(string secretKey, string code)
        {
            var totp = new Totp(Base32Encoding.ToBytes(secretKey));
            return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
        }
        public string CreateMfaToken(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(SD.UserName, userName)
            };

            var creds = new SigningCredentials(_mfaKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["MFA:TokenExpiresInMinutes"])),
                SigningCredentials = creds,
                Issuer = _config["MFA:Issuer"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GetUserNameFromMfaToken(string mfaToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                tokenHandler.ValidateToken(mfaToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _mfaKey,
                    ValidateIssuer = true,
                    ValidIssuer = _config["MFA:Issuer"],
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                var token = (JwtSecurityToken)validatedToken;
                return token.Claims.First(x => x.Type == SD.UserName)?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
