using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Linq;

namespace BusinessLayer.Models
{
    public class JWTTokenService
    {
        private string _key;
        private JwtHeader _header;
        public JWTTokenService(string key)
        {
            _key = key;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            _header = new JwtHeader(credentials);
        }

        public string GetSecurityToken(string serializedJSON)
        {
            JwtPayload payload = new JwtPayload
            {
                { "report", serializedJSON }
            };

            var secToken = new JwtSecurityToken(_header, payload);

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(secToken);
        }

        public string ReadJWTToken(string secToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(secToken);
            return (string)token.Payload.First().Value;
        }

    }
}
