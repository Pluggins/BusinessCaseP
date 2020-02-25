using BCP_Facial.Data;
using BCP_Facial.Models;
using BCP_Facial.Models.ViewModels;
using BCP_Facial.Services;
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
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                if (string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password) || string.IsNullOrEmpty(input.ConfirmPassword))
                {
                    Response.StatusCode = 400;
                    output.Result = "INPUT_IS_NULL";
                } else
                {
                    if (input.Password.Length < 6)
                    {
                        Response.StatusCode = 400;
                        output.Result = "PASSWORD_LENGTH_LESS_6";
                    } else
                    {
                        if (input.Password.Equals(input.ConfirmPassword))
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
                                }
                                else
                                {
                                    Response.StatusCode = 500;
                                    output.Result = "INTERNAL_ERROR";
                                }
                            }
                            else
                            {
                                Response.StatusCode = 400;
                                output.Result = "USER_ALREADY_EXIST";
                            }
                        } else
                        {
                            Response.StatusCode = 400;
                            output.Result = "PASSWORD_MISMATCH";
                        }
                        
                    }
                }
            }
            return output;
        }

        [HttpPost]
        [Route("Api/User/WebLogin")]
        public async Task<UserInfoOutput> WebLogin([FromBody] UserInfoInput input)
        {
            UserInfoOutput output = new UserInfoOutput();
            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                if (string.IsNullOrEmpty(input.Email) || string.IsNullOrEmpty(input.Password))
                {
                    Response.StatusCode = 400;
                    output.Result = "INPUT_IS_NULL";
                }
                else
                {
                    IdentityUser aspUser = _db._AspNetUsers.Where(e => e.UserName.ToLower().Equals(input.Email.ToLower())).FirstOrDefault();
                    if (aspUser == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "USER_NOT_FOUND";
                    }
                    else
                    {
                        if (_userManager.PasswordHasher.VerifyHashedPassword(aspUser, aspUser.PasswordHash, input.Password) == PasswordVerificationResult.Success)
                        {
                            await _signInManager.SignInAsync(aspUser, true);
                            output.Result = "OK";
                        }
                        else
                        {
                            Response.StatusCode = 400;
                            output.Result = "PASSWORD_MISMATCH";
                        }
                    }
                }
            }
            return output;
        }

        [HttpPost]
        [Route("Api/User/CheckRole")]
        public async Task<UserInfoOutput> CheckRole()
        {
            UserInfoOutput output = new UserInfoOutput();
            AspUserService aspUser = new AspUserService(_db, this);

            if (aspUser.User.Status == 1)
            {
                if (User.IsInRole("LECTURER"))
                {
                    await _userManager.RemoveFromRoleAsync(aspUser.User.AspUser, "LECTURER");
                }
                if (User.IsInRole("ADMIN"))
                {
                    await _userManager.RemoveFromRoleAsync(aspUser.User.AspUser, "ADMIN");
                }
            }

            if (aspUser.User.Status == 2)
            {
                if (!User.IsInRole("LECTURER"))
                {
                    await _userManager.AddToRoleAsync(aspUser.User.AspUser, "LECTURER");
                }
                if (User.IsInRole("ADMIN"))
                {
                    await _userManager.RemoveFromRoleAsync(aspUser.User.AspUser, "ADMIN");
                }
            }

            if (aspUser.User.Status == 3)
            {
                if (User.IsInRole("LECTURER"))
                {
                    await _userManager.RemoveFromRoleAsync(aspUser.User.AspUser, "LECTURER");
                }
                if (!User.IsInRole("ADMIN"))
                {
                    await _userManager.AddToRoleAsync(aspUser.User.AspUser, "ADMIN");
                }
            }

            if (aspUser.User.Status == 4)
            {
                if (!User.IsInRole("LECTURER"))
                {
                    await _userManager.AddToRoleAsync(aspUser.User.AspUser, "LECTURER");
                }
                if (!User.IsInRole("ADMIN"))
                {
                    await _userManager.AddToRoleAsync(aspUser.User.AspUser, "ADMIN");
                }
            }

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(aspUser.User.AspUser, true);
            output.Result = "OK";
            return output;
        }

        [HttpPost]
        [Route("Api/User/Logout")]
        public async Task<UserInfoOutput> Logout()
        {
            await _signInManager.SignOutAsync();
            UserInfoOutput output = new UserInfoOutput()
            {
                Result = "OK"
            };

            return output;
        }
    }
}
