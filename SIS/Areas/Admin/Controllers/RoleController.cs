using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIS.Data;
using SIS.Helper;
using SIS.Models;
namespace SIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly RoleHelper roleHelper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private DBContext db;
        public RoleController(RoleManager<IdentityRole> roleManager, DBContext db , RoleHelper roleHelper)
        {
            this.roleHelper = roleHelper;
            this._roleManager = roleManager;
            this.db = db;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }
        public async Task<IActionResult> Update(string id)
        {

            if (!string.IsNullOrEmpty(id)&&id=="0")
                return View(new IdentityRole(""));
            var md =await _roleManager.FindByIdAsync(id);
            //var md = db.Privileges.Single(a => a.Id == id);

            return View(md);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(IdentityRole md)
        {
            if (string.IsNullOrEmpty(md.Name))
            {
                return BadRequest("Role name cannot be empty.");
            }
            var isExrole = await _roleManager.FindByIdAsync(md.Id);
            
            if (isExrole == null)
            {

                var role = new IdentityRole(md.Name);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    await db.SaveChangesAsync();
                    TempData["Success"] = "تمت إضافةالعنصر بنجاح..!";
                }
                else
                {
                    TempData["error"] = "حدث خطأ أثناء تعديل العنصر..!" + result.Errors;
                }
            }
            else
            {
                isExrole.Name = md.Name;
                var result = await _roleManager.UpdateAsync(isExrole);
                if (result.Succeeded)
                {
                    await db.SaveChangesAsync();
                    TempData["Success"] = "تمت تعديل البيانات بنجاح..!";
                }
                else
                {
                    TempData["error"] = "حدث خطأ أثناء تعديل العنصر..!" + result.Errors;
                }

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            var users = await roleHelper.ListUsersInRole(role.Id);
            if (users != null)
            {
                TempData["error"] = "لم نتمكن من عملية الحذف لخطأ ما حدث.. حاول مرة أخرى..!";
                return RedirectToAction("Index");
            }
            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                TempData["Success"] = "تم حذف البيانات بنجاح..!";
            }
            else
            {
                TempData["error"] = "لم نتمكن من عملية الحذف لخطأ ما حدث.. حاول مرة أخرى..!";
            }
            return RedirectToAction("Index");
        }

        public List<IdentityRole> getRoleList()
        {
            return _roleManager.Roles.ToList();
        }
    }
}
