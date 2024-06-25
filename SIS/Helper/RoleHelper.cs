using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIS.Data;

namespace SIS.Helper
{
    public interface IRoleHelper
    {
        List<IdentityRole> ListRoles();
    }
    public class RoleHelper: IRoleHelper
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _UserManager;
        private readonly DBContext db;
        public RoleHelper(RoleManager<IdentityRole> _roleManager, UserManager<User> _UserManager, DBContext db)
        {
            this._roleManager = _roleManager;
            this._UserManager = _UserManager;
            this.db = db;
        }
        public List<IdentityRole> ListRoles()
        {
            return _roleManager.Roles.ToList();
        }
        public SelectList RenderRolesList()
        {
            var roles=_roleManager.Roles.ToList();
            var rolesList = roles.Select(a => new { a.Id, a.Name }).ToList();
            var lst = new SelectList(rolesList, "Id", "Name");
            return lst;
        }
        public async Task<User> getUserByEmail(string email)
        {
            // Find the user by ID
            if (!string.IsNullOrEmpty(email))
                return await _UserManager.FindByEmailAsync(email);
            return null;
        }
        public async Task<string> getRoleName(string id)
        {
            string name = "";
            var role= await _roleManager.FindByIdAsync(id);
            if (role != null)
                name = role.Name;
            return name;
        }
        public async Task<User> getUserInfobyEmail(string email)
        {
            if (!string.IsNullOrEmpty(email)) { 
            var u = await _UserManager.FindByEmailAsync(email);
            if (u != null)
                return u;
        }
            return null;
        }
        public async Task<bool> RemoveUserFromAllRoles(string userId)
        {
            var user = await _UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var roles = await _UserManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    var result = await _UserManager.RemoveFromRolesAsync(user, roles);
                    return result.Succeeded;
                }
            }
            return false;
        }
        public async Task<bool> AddUserToRoleByname(string userId, string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                bool b=await AddUserToRole(userId, role.Name);
                return b;
            }
            return false;
        }
            public async Task<bool> AddUserToRole(string userId, string roleName)
        {
            bool isRemoved = await RemoveUserFromAllRoles(userId);
           // if (isRemoved)
            //{
                var user = await _UserManager.FindByIdAsync(userId);
                if (user != null)
                {
                    var role = await _roleManager.FindByIdAsync(roleName);
                    if (role != null)
                    {
                        var result = await _UserManager.AddToRoleAsync(user, role.Name);
                        return true;
                    }
                }
                
            //}
            return false;
        }
        public async Task<List<User>> ListUsersInRole(string id)
        {
            // Find the role by name
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                // Role does not exist
                return null;
            }

            // Get the list of users in the role
            var usersInRole = await _UserManager.GetUsersInRoleAsync(role.Name);

            return usersInRole.ToList();
        }
        public async Task<bool> RemoveUser(string userId)
        {
            // Find the user by ID
            var user = await _UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                // User does not exist
                return false;
            }

            // Delete the user
            var result = await _UserManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // User deleted successfully
                return true;
            }
            return false;
        }

        public async Task<bool> SetUserPassword(string userId, string newPassword)
        {
            // Find the user by ID
            var user = await _UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                // User does not exist
                return false;
            }
            var result1 = await _UserManager.RemovePasswordAsync(user);
            
            if (result1.Succeeded)
            {
                var result = await _UserManager.AddPasswordAsync(user, newPassword);
                if (result.Succeeded)
            {
                // User deleted successfully
                return true;
            }
        }
            return false;
        }
        public async Task<string> getUserRole(string userId)
        {
            var user = await _UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                // User does not exist
                return string.Empty;
            }
            var roles = await _UserManager.GetRolesAsync(user);
            if (roles.Count > 0)
            {
                return roles[0];
            }
            return string.Empty;
        }
        public async Task<string> getUserRoleId(string userId)
        {

            string roleName = await getUserRole(userId);
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if(role!=null)
                    return role.Id;
            }
            
            return string.Empty;
        }
        public async Task<bool> createUser(string userName, string password, string role)
        {
            User us = new User();
            us.Email = userName;
            us.UserName = userName;
            us.FullName = userName;
            us.Address = "None";
            us.City = "None";
            us.Country = "None";
            us.HomeTown = "None";
            us.Id = Guid.NewGuid().ToString();
            var result = await _UserManager.CreateAsync(us, password);

            if (result.Succeeded)
            {
                //get user info
                var nw_user = await _UserManager.FindByIdAsync(us.Id);
                //add th user to role
                //1.Remove from all role
                if (nw_user != null)
                {
                    bool a = await AddUserToRoleByName(nw_user.Id, role);
                }
                //await db.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AddUserToRoleByName(string userId, string roleName)
        {
            bool isRemoved = await RemoveUserFromAllRoles(userId);
            // if (isRemoved)
            //{
            var user = await _UserManager.FindByIdAsync(userId);
            if (user != null)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var result = await _UserManager.AddToRoleAsync(user, role.Name);
                    return true;
                }
            }

            //}
            return false;
        }
        //Priviliges===============================
        public async Task<bool> checkUserPriviliges(string userID, string privileges, string privileges_type)
        {
            bool authorize = false;


            var prv = await db.Privileges_UserBased.Where(x => x.UserId == userID && x.Privileges.priv_name == privileges).FirstOrDefaultAsync();
            if (prv != null)
            {
                if (privileges_type == "is_print" && prv.is_print == true)
                    authorize = true;
                else if (privileges_type == "is_delete" && prv.is_delete == true)
                    authorize = true;
                else if (privileges_type == "is_displayed" && prv.is_displayed == true)
                    authorize = true;
                else if (privileges_type == "is_insert" && prv.is_insert == true)
                    authorize = true;
                else if (privileges_type == "is_update" && prv.is_update == true)
                    authorize = true;
            }



            return authorize;
        }
        public async Task<bool> checkUserPriviliges(string userID, int priv_ID, string privileges_type)
        {
            string s = await getUserRole(userID);
            if (string.IsNullOrEmpty("المدراء"))
            {
                return true;
            }
            else
            {

                var q = db.Privileges.Where(x => x.Id == priv_ID).SingleOrDefault();
                if (q != null)
                    return await checkUserPriviliges(userID, q.priv_name, privileges_type);
            }

            return false;
        }
        public async Task<bool> checkUserPriviligesByEmail(string email, int priv_ID, string privileges_type)
        {

            return true;
            var userInfo = await getUserInfobyEmail(email);
            if (userInfo != null)
            {
                return await checkUserPriviliges(userInfo.Id, priv_ID, privileges_type);
            }
            return true;
        }
        public async Task<(int,int,int)> checkUserCollegePriviliges(string email)
        {
            int result1 = 0, result2=0, result3=0;
            var userInfo = await getUserInfobyEmail(email);
            if (userInfo != null)
            {
                //int colege = (userInfo.collegeId.HasValue? userInfo.collegeId.Value:0);
                result1 = (userInfo.collegeId.HasValue ? userInfo.collegeId.Value : 0);
                result2 = (userInfo.DeptId.HasValue ? userInfo.DeptId.Value : 0);
                result3 = (userInfo.SpecialId.HasValue ? userInfo.SpecialId.Value : 0);
            }
            return (result1, result2, result3);
        }
    }

}
