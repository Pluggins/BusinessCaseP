using BCP_Facial.Data;
using BCP_Facial.Models;
using BCP_Facial.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Controllers.Api
{
    public class UserApiController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public UserApiController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        [HttpPost]
        [Route("Api/User/Create")]
        public async Task<UserInfoOutput> Create([FromBody] UserInfoInput input)
        {
            UserInfoOutput output = new UserInfoOutput();
            
            if (input == null)
            {
                output.Result = "INPUT_IS_NULL";
            } else
            {
                if (string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(input.ConfirmPassword))
                {
                    output.Result = "INPUT_IS_NULL";
                } else
                {
                    if (input.Password.Length < 6)
                    {
                        output.Result = "PASSWORD_LENGTH_LESS_6";
                    } else
                    {
                        BCPUser user = _db._BCPUsers.Where(e => e.Email.Equals(input.Email)).FirstOrDefault();

                        if (user == null)
                        {
                            IdentityUser newAspUser = new IdentityUser()
                            {
                                UserName = input.Email,
                                Email = input.Email
                            };

                            var status = await _userManager.CreateAsync(newAspUser, input.Password);

                            if (status.Succeeded)
                            {
                                user = new BCPUser()
                                {
                                    AspUser = newAspUser,
                                    Name = input.Name,
                                    Email = input.Email
                                };

                                _db._BCPUsers.Add(user);
                                _db.SaveChanges();

                                output.Result = "OK";
                            } else
                            {
                                output.Result = "INTERNAL_ERROR";
                            }
                        }
                        else
                        {
                            output.Result = "USER_ALREADY_EXIST";
                        }
                    }
                }
            }
            return output;
        }
    }
}
