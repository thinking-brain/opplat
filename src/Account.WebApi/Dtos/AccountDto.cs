using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Account.WebApi.Data;
using Account.WebApi.Dtos;
using Account.WebApi.Models;
using LdapForNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Account.WebApi.Dtos {
    public class AccountDto {
        public string UserId { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Username { get; set; }

        public bool Activo { get; set; }
        public List<string> Roles { get; set; }

        public AccountDto () {
            Roles = new List<string> ();
        }
    }
}