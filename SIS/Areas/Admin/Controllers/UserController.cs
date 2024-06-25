using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SIS.Data;
using SIS.Models;
using SIS.Helper;
using Microsoft.AspNetCore.Authorization;
using Castle.Core.Internal;

namespace SIS.Areas.Admin.Controllers
{
   // [Authorize]
    [Area("Admin")]
    public class UserController : Controller
    {
       
        private readonly UserManager<User> _UserManager;
        private readonly FilesHelper filesHelper;
        private DBContext db;
        private readonly RoleHelper roleHelper;
        public UserController(UserManager<User> _UserManager, DBContext db, RoleHelper roleHelper)
        {
            this._UserManager = _UserManager;
            this.db = db;
            this.roleHelper = roleHelper;
            this.filesHelper = new FilesHelper();
        }
        public async Task<IActionResult> Index(string id)
        {

            var u = _UserManager.Users.ToList();
            if (!string.IsNullOrEmpty(id))
            {

                u = await roleHelper.ListUsersInRole(id);
            }
            return View(u);
        }
        public async Task<IActionResult> Update(string id)
        {

            if (id == "0")
                return View(new User {Id="0" });
            var md = await _UserManager.FindByIdAsync(id);//db.Users.SingleOrDefault(a => a.Id == id);
            //var md = db.Privileges.Single(a => a.Id == id);
            
            return View(md);
        }
        public async Task<IActionResult> Save(User md, IFormFile filePicture)
        {
            string filename = "";
            byte[] ProfilePicture = null;
            if (filePicture != null && filePicture.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await filePicture.CopyToAsync(ms);
                    ProfilePicture=ms.ToArray();
                }
                //    string pth = filesHelper.UplodeFile2(filePicture, "User");
               // if (!string.IsNullOrEmpty(pth))
               //     filename = pth;
            }

            bool isValid = true;
            string pwd=Request.Form["password"];
            string cpwd = Request.Form["ConfirmPassword"];
            string userRole = Request.Form["userRole"];
            
            if (string.IsNullOrEmpty(md.FullName) && string.IsNullOrEmpty(pwd) && string.IsNullOrEmpty(cpwd) && pwd!= cpwd && string.IsNullOrEmpty(userRole))
            {
                isValid = false;
            }
            if (isValid)
            {
                var u = await _UserManager.FindByEmailAsync(md.Email);
                if (u == null)
                {
                    md.UserName = md.Email;
                    md.Id=Guid.NewGuid().ToString();
                    //md.UserLogo = ProfilePicture;
                    var result = await _UserManager.CreateAsync(md, pwd);

                    if (result.Succeeded)
                    {
                        //get user info
                        var nw_user = await _UserManager.FindByIdAsync(md.Id);
                        //add th user to role
                        //1.Remove from all role
                        if (nw_user != null) {
                            bool a=await roleHelper.AddUserToRole(nw_user.Id, userRole);
                            }
                        //await db.SaveChangesAsync();
                        TempData["Success"] = "تمت إضافةالعنصر بنجاح..!";
                    }
                    else
                    {
                        TempData["error"] = "حدث خطأ أثناء تعديل العنصر..!" + result.Errors;
                    }
                }
                else
                {
                    bool z = false;
                    if(u.PasswordHash == pwd)
                        z = true;
                    
                    u.FullName = md.FullName;
                    u.City = md.City;
                    u.Address = md.Address;
                    u.Country = md.Country;
                    //bind 
                    u.collegeId = (md.collegeId.HasValue? md.collegeId.Value : null);
                    u.DeptId = (md.DeptId.HasValue ? md.DeptId.Value : null);
                    u.SpecialId = (md.SpecialId.HasValue ? md.SpecialId.Value : null);
                    //u.UserLogo = ProfilePicture;
                    var result = await _UserManager.UpdateAsync(u);
                    if (result.Succeeded)
                    {
                        
                        if (u != null)
                        {
                            //change user password
                            if (u.PasswordHash != pwd)//user have change the password
                            {
                                bool b = await roleHelper.SetUserPassword(u.Id, pwd);
                            }
                                bool a = await roleHelper.AddUserToRole(u.Id, userRole);
                        }
                        await db.SaveChangesAsync();
                        TempData["Success"] = "تمت تعديل البيانات بنجاح..!";
                    }
                    else
                    {
                        TempData["error"] = "حدث خطأ أثناء تعديل العنصر..!" + result.Errors;
                    }

                }
            }
            else
            {
                TempData["error"] = "كلمة المرور غير متطابقة او أن الاسم مفقود..!" ;
            }
                return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> Delete(string id)
        {
            var b = await roleHelper.RemoveUser(id);
            if (b == true) { 
            TempData["Success"] = "تم حذف البيانات بنجاح..!";
        }
            else
            {
                TempData["error"] = " لايمكن حذف المستخدم..!";
                
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UserPrivilies(string id)
        {
            var md = await _UserManager.FindByIdAsync(id);
            return View(md);
        }

        [HttpPost]
        public async Task<IActionResult> SavePrivilges()
        {
            String id = Request.Form["Id"];
            var u = await _UserManager.FindByIdAsync(id);
            if (u == null)
            {
                TempData["error"] = " لا يوجد هذا المستخدم..!";
            }
            else
            {
                String roleID = Request.Form["roleID"];
                var lst = db.Privileges
                    //.Where(a => a.priv_cat == roleID)
                    .ToList();
                foreach (var v in lst)
                { //just add mising priviliges if none
                    if (db.Privileges_UserBased.Where(x => x.PrivilegesId == v.Id && x.UserId == u.Id).Count() == 0)
                    {
                        Privileges_UserBased o = new Privileges_UserBased { PrivilegesId = v.Id, UserId = u.Id, is_insert = false, is_delete = false, is_displayed = false, is_print = false, is_update = false };
                        db.Privileges_UserBased.Add(o);
                        db.SaveChanges();
                    }
                }
                //------------------------------
                String[] insertCheck = Request.Form["insertCheck"];
                String[] updateCheck = Request.Form["updateCheck"];
                String[] deleteCheck = Request.Form["deleteCheck"];
                String[] displayCheck = Request.Form["displayCheck"];
                String[] printCheck = Request.Form["printCheck"];
                //   if (insertCheck != null)
                //{
                //var minus = lst.Where(x => !insertCheck.Contains(x.id.ToString())).ToList();
                //int k = minus.Count();
                foreach (var prg in lst)
                {
                    var pr = db.Privileges_UserBased.Where(x => x.PrivilegesId == prg.Id && x.UserId == u.Id).SingleOrDefault();
                    if (pr != null)
                    {
                        try
                        {
                            if (insertCheck != null && insertCheck.Contains(prg.Id.ToString()))
                                pr.is_insert = true;
                            else
                                pr.is_insert = false;
                            //-----
                            if (updateCheck != null && updateCheck.Contains(prg.Id.ToString()))
                                pr.is_update = true;
                            else
                                pr.is_update = false;
                            //-----
                            if (deleteCheck != null && deleteCheck.Contains(prg.Id.ToString()))
                                pr.is_delete = true;
                            else
                                pr.is_delete = false;
                            //-----
                            if (displayCheck != null && displayCheck.Contains(prg.Id.ToString()))
                                pr.is_displayed = true;
                            else
                                pr.is_displayed = false;
                            //-----
                            if (printCheck != null && printCheck.Contains(prg.Id.ToString()))
                                pr.is_print = true;
                            else
                                pr.is_print = false;
                            db.SaveChanges();
                        }
                        catch (Exception zx) { }
                    }
                }
                TempData["Success"] = "تم حفظ الصلاحيات بنجاح..!";
            }
                return Redirect("Index");
        }

        public async Task<IActionResult> UserProfile()
        {
            
            var u = await roleHelper.getUserInfobyEmail(User.Identity.Name);
            
            return View(u);
        }
        public async Task<IActionResult> SaveProfile(User md, IFormFile filePicture)
        {
            string filename = "";
            byte[] ProfilePicture = null;
            if (filePicture != null && filePicture.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await filePicture.CopyToAsync(ms);
                    ProfilePicture = ms.ToArray();
                }
                //    string pth = filesHelper.UplodeFile2(filePicture, "User");
                // if (!string.IsNullOrEmpty(pth))
                //     filename = pth;
            }

            bool isValid = true;
            string pwd = Request.Form["password"];
            string cpwd = Request.Form["ConfirmPassword"];
            string userRole = Request.Form["userRole"];

            if (string.IsNullOrEmpty(md.FullName) && string.IsNullOrEmpty(pwd) && string.IsNullOrEmpty(cpwd) && pwd != cpwd && string.IsNullOrEmpty(userRole))
            {
                isValid = false;
            }
            if (isValid)
            {
                var u = await _UserManager.FindByEmailAsync(md.Email);
                if (u == null)
                {
                    md.UserName = md.Email;
                    md.Id = Guid.NewGuid().ToString();
                    md.UserLogo = ProfilePicture;
                    var result = await _UserManager.CreateAsync(md, pwd);

                    if (result.Succeeded)
                    {
                        //get user info
                        var nw_user = await _UserManager.FindByIdAsync(md.Id);
                        //add th user to role
                        //1.Remove from all role
                        if (nw_user != null)
                        {
                            bool a = await roleHelper.AddUserToRole(nw_user.Id, userRole);
                        }
                        //await db.SaveChangesAsync();
                        TempData["Success"] = "تمت إضافةالعنصر بنجاح..!";
                    }
                    else
                    {
                        TempData["error"] = "حدث خطأ أثناء تعديل العنصر..!" + result.Errors;
                    }
                }
                else
                {
                    bool z = false;
                    if (u.PasswordHash == pwd)
                        z = true;

                    u.FullName = md.FullName;
                    u.City = md.City;
                    u.Address = md.Address;
                    u.Country = md.Country;
                    u.UserLogo = ProfilePicture;
                    var result = await _UserManager.UpdateAsync(u);
                    if (result.Succeeded)
                    {

                        if (u != null)
                        {
                            //change user password
                            if (u.PasswordHash != pwd)//user have change the password
                            {
                                bool b = await roleHelper.SetUserPassword(u.Id, pwd);
                            }
                          //  bool a = await roleHelper.AddUserToRole(u.Id, userRole);
                        }
                        await db.SaveChangesAsync();
                        TempData["Success"] = "تمت تعديل البيانات بنجاح..!";
                    }
                    else
                    {
                        TempData["error"] = "حدث خطأ أثناء تعديل العنصر..!" + result.Errors;
                    }

                }
            }
            else
            {
                TempData["error"] = "كلمة المرور غير متطابقة او أن الاسم مفقود..!";
            }
            return RedirectToAction("UserProfile");
        }
    }
}
