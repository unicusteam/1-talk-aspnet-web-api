using Apresentacao.Models;
using Apresentacao.Profiles;
using Apresentacao.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace Apresentacao.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private IUserService _service;
        private SigningConfiguration _signingConfiguration;
        private TokenConfiguration _tokenConfiguration;

        public AccountController(IUserService service, SigningConfiguration signingConfiguration, TokenConfiguration tokenConfiguration)
        {
            _service = service;
            _signingConfiguration = signingConfiguration;
            _tokenConfiguration = tokenConfiguration;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult<string> Post([FromBody]UserModel userModel)
        {
            try
            {
                SecurityToken securityToken = null;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                DateTime dateCreation = DateTime.Now;
                DateTime dateExpiration = dateCreation.Add(new TimeSpan(_tokenConfiguration.ClockSkew));
                User user = GetUser(userModel);

                if (user != null)
                {
                    ClaimsIdentity identity = new ClaimsIdentity(
                        new GenericIdentity(user.Login, "Login"),
                        new[] {
                        new Claim(ClaimNames.UserId, user.Id.ToString("N")),
                        new Claim(ClaimNames.UserName, user.Login),
                        }
                    );

                    securityToken = handler.CreateToken(new SecurityTokenDescriptor
                    {
                        Issuer = _tokenConfiguration.Issuer,
                        Audience = _tokenConfiguration.Audience,
                        SigningCredentials = _signingConfiguration.SigningCredentials,
                        Subject = identity,
                        NotBefore = dateCreation,
                        Expires = dateExpiration
                    });
                }

                if (securityToken == null)
                {
                    new Exception();
                }

                return JsonConvert.SerializeObject(new
                {
                    authenticated = true,
                    created = dateCreation.ToString("yyyy-MM-dd HH:mm:ss"),
                    dateExpiration = dateExpiration.ToString("yyyy-MM-dd HH:mm:ss"),
                    accessToken = handler.WriteToken(securityToken),
                    message = "OK"
                });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new
                {
                    authenticated = false,
                    message = $"Falha ao autenticar. {ex.Message}"
                });
            }
        }

        private User GetUser(UserModel userModel)
        {
            if (string.IsNullOrWhiteSpace(userModel?.Login) || string.IsNullOrWhiteSpace(userModel?.Password))
            {
                return null;
            }

            return _service.GetByLogin(userModel.Login, userModel.Password);
        }
    }
}