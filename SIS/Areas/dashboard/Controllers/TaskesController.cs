using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIS.Data;
using SIS.Helper;
using SIS.Models;

namespace SIS.Areas.dashboard.Controllers
{
    [Authorize]
    [Area("dashboard")]
    public class TaskesController : Controller
    {
        private DBContext db;
        private readonly RoleHelper roleHelper;

        public TaskesController(DBContext db , RoleHelper roleHelper)
        {
            this.roleHelper = roleHelper;
            this.db = db;
        }
        // GET: AcademicYearController
        public async Task<ActionResult> Index(bool? state)
        {
            try
            {
                string m = User.Identity.Name;
                var userInfo =  await roleHelper.getUserInfobyEmail(m);
                string userId = userInfo.Id;
                var t = db.Taskes.Where(x=>x.UserId == userId).ToList();
                if (state != null)
                {
                    t = db.Taskes.Where(x => x.UserId == userId && x.State == state).ToList();
                }
                return View(t);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult change_state(int id)
        {
            try
            {
                Taske st = db.Taskes.Single(x => x.Id == id);
                if (!st.State)
                {
                    st.State = true;
                    db.SaveChanges();
                    return Json(new { status = "1" });
                }
                else
                {
                    st.State = false;
                    db.SaveChanges();
                    return Json(new { status = "1" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { state = "0" });
            }
        }

        public IActionResult Update(int id)
        {
            if (id == 0)
                return View(new Taske { Id = 0 });

            var md = db.Taskes.Single(a => a.Id == id);
            return View(md);
        }
        [HttpPost]
        public async Task<IActionResult> Save(Taske md)
        {
            if (md.Id == 0)
            {
                try
                {
                    string m = User.Identity.Name;
                    var userInfo = await roleHelper.getUserInfobyEmail(m);
                    string userId = userInfo.Id;

                    md.UserId = userId;
                    db.Taskes.Add(md);
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
            var md = db.Taskes.Single(a => a.Id == id);
            db.Remove(md);
            db.SaveChanges();
            TempData["Success"] = "تم حذف البيانات بنجاح..!";
            return RedirectToAction("Index");
        }
    }
}
