using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Repositories {
    public class LoginRepository {

        EmpresaDbContext _db = new EmpresaDbContext();

        public Usuarios CheckCredentials(string login, string password) {
            List<Usuarios> a = _db.Usuarios.ToList();
            //el login puede ser el username, o el mail
            Usuarios Entity = _db.Usuarios.FirstOrDefault(x => x.Username.ToLower().Trim() == login.ToLower().Trim()
                                                    || x.Email.ToLower().Trim() == login.ToLower().Trim());

            if (Entity == null)
                return null;

            if (password == Entity.Password)
                return Entity;
            else
                return null;
        }

        public object GenerateToken(Usuarios Usuario) {
            var claims = new[] {
                    new Claim(ClaimTypes.Name, Usuario.Username)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("frase secreta para generar un array de bytes unico"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime TokenExpiration = DateTime.Now.AddMinutes(30);

            var token = new JwtSecurityToken(
                issuer: "Empresa/api",
                audience: "EmpresaApp",
                claims: claims,
                expires: TokenExpiration,
                signingCredentials: creds
            );

            return new {
                ID = Usuario.ID,
                //originalmente Username = Usuario.username, pero .net entendio q asi se tenia q llamar, y me tiro sugerencia de nombre inferido apra todas estas varaibles:
                Usuario.Username,
                Usuario.Nombre,
                Usuario.Apellido,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenExpiration
            };
        }

        public void UpdateUserlastLogin(decimal userId) {
            Usuarios u = _db.Usuarios.FirstOrDefault(x => x.ID == userId);
            u.LastLogin = DateTime.Now;
            _db.SaveChanges();
        }



        public void registerUser(string username, string password) {

            Usuarios user = new Usuarios();
            user.Username = username;
            user.Password = password;
            _db.Usuarios.Add(user);
            _db.SaveChanges();

        }

    }
}
