using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using SIS.Data;
using SIS.Models;
namespace SIS.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PriviligesController : Controller
    {
       
        private DBContext db;
        private readonly RoleManager<IdentityRole> _roleManager;
        public PriviligesController(RoleManager<IdentityRole> roleManager, DBContext db)
        {
            this.db = db;
            this._roleManager = roleManager;
        }
        public IActionResult Index(string id)
        {

            var t = db.Privileges.ToList();
            if (!string.IsNullOrEmpty(id))
            {

                t = t.Where(a=>a.priv_cat==id).ToList();
            }
            return View(t);
        }
        public async Task<IActionResult> Update(int id)
        {
            var roles = _roleManager.Roles.ToList();
            var rolesList = roles.Select(a => new { a.Id, a.Name }).ToList();
            ViewBag.rolesList = new SelectList(rolesList, "Id", "Name");
            if (id == 0)
                return View(new Privileges { Id = 0,priv_key=Guid.NewGuid() });

            var md = db.Privileges.Single(a => a.Id == id);
            
            return View(md);
        }
        [HttpPost]
        public IActionResult Save(Privileges md)
        {
            if (md.Id == 0)
            {
                try
                {
                    
                    db.Privileges.Add(md);
                    db.SaveChanges();
                    TempData["Success"] = "تم الأضافة بنجاح..!";
                }
                catch (Exception ee)
                {
                    TempData["error"] = "حدث خطأ أثناء اضافة العنصر..!";
                }
            }
            else
            {
                try
                {
                    if(md.priv_key == new Guid())
                        md.priv_key=Guid.NewGuid();
                    db.Update(md);
                    db.SaveChanges();
                    TempData["Success"] = "تم تعديل البيانات بنجاح..!";
                }
                catch (Exception ee)
                {
                    TempData["error"] = "حدث خطأ أثناء تعديل العنصر..!";
                }
            }
            return RedirectToAction("Index");
        }
        //[HttpPost]
        public IActionResult Delete(int id)
        {
            var md = db.Privileges.Single(a => a.Id == id);
            db.Remove(md);
            db.SaveChanges();
            TempData["Success"] = "تم حذف البيانات بنجاح..!";
            return RedirectToAction("Index");
        }
    }
}
