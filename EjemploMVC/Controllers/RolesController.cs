using Microsoft.AspNetCore.Identity; // RoleManager / UserManager
//using Microsoft.AspNetCore.Authorization; //Validador
using Microsoft.AspNetCore.Mvc; //Controller
using static System.Console;

namespace EjemploMVC.Controllers
{
    public class RolesController : Controller
    {
        private string AdminRole = "Administrators";
        private string UserEmail = "admin2@ejemplo.com";
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public RolesController(RoleManager<IdentityRole> roleManager,
                                UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        //[Authorize(Roles = "Developers")]
        public async Task<IActionResult> Index()
        {
            // Crear el role Administrators
            if (!(await roleManager.RoleExistsAsync(AdminRole)))
            {
                await roleManager.CreateAsync(new IdentityRole(AdminRole));
            }
            // Crear el usuario
            IdentityUser user = await userManager.FindByEmailAsync(UserEmail);
            if (user == null)
            {
                user = new();
                user.UserName = UserEmail;
                user.Email = UserEmail;
                IdentityResult result = await userManager.CreateAsync(user, "Pa$$w0rd");
                if (result.Succeeded)
                {
                    WriteLine($"RESULTADO: Usuario {user.UserName} fue creado exitosamente");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        WriteLine($"ERROR: {error.Description}");
                    }
                }
            }
            // Confirmar el correo
            if (!user.EmailConfirmed)
            {
                string token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                IdentityResult result = await userManager.ConfirmEmailAsync(user,token);
                if (result.Succeeded)
                {
                    WriteLine($"RESULTADO: El e-mail del usuario {user.UserName} fue confirmado exitosamente");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        WriteLine($"ERROR: {error.Description}");
                    }
                }
            }
            //Agregar el Rol de Administrador al usuario
            if (!(await userManager.IsInRoleAsync(user,AdminRole)))
            {
                IdentityResult result = await userManager.AddToRoleAsync(user, AdminRole);
                if (result.Succeeded)
                {
                    WriteLine($"RESULTADO: El usuario {user.UserName} fue agregado como {AdminRole}");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        WriteLine($"ERROR: {error.Description}");
                    }
                }
            }
            return Redirect("/");
        }
    }
}
