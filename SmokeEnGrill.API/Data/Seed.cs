using System;
using System.Collections.Generic;
using System.Linq;
using SmokeEnGrill.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using EducNotes.API.Models;

namespace SmokeEnGrill.API.Data
{
    public class Seed
    {
        public static void SeedUsers(DataContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {

            if (!userManager.Users.Any())
            {
                var payments = new List<PaymentType>{
                    new PaymentType { Name = "espèce" },
                    new PaymentType { Name = "mobile money"},
                    new PaymentType { Name = "chèque"}
                };
                context.AddRange(payments);

                var methods = new List<DeliveryMethod> {
                    new DeliveryMethod{ Name= "à emporter" },
                    new DeliveryMethod { Name = "livraison"},
                    new DeliveryMethod { Name = "sur place"}
                };
                context.AddRange(methods);

                var ordertypes = new List<OrderType>{
                    new OrderType { Name = "vente" },
                    new OrderType { Name = "achat" }
                };
                context.AddRange(ordertypes);

                var countries = new List<Country> {
                    new Country { Name = "Côte d'Ivore" },
                };
                context.AddRange(countries);


                // var cities = new List<City> {
                //     new City { Name="ville1" },
                //     new City{ Name="Ville2" }
                // };
                // context.AddRange(cities);


                //  var districts = new List<District>{
                //     new District{Name="district1"},
                //     new District{Name="district2"}
                // };
                // context.AddRange(districts);

                var storetypes = new List<StoreType> {
                    new StoreType {Name = "Magasin"},
                    new StoreType {Name = "employé"}
                };
                context.AddRange(storetypes);
                context.SaveChanges();

                var stores = new List<Store> {
                new Store { Name = "magasin principal", StoreTypeId = 1 },
                };
                context.AddRange(stores);

                var inventOptypes = new List<InventOpType>() {
                    new InventOpType { Name = "entrée Stock" },
                    new InventOpType { Name = "sortie Stock" },
                    new InventOpType { Name = "transfert Stock" },
                    new InventOpType { Name = "inventaire" }
                };
                context.AddRange(inventOptypes);

                var roles = new List<Role> {
                    new Role { Name = "gérant" },
                    new Role { Name = "employé" },
                    new Role {Name = "livreur"},
                    new Role {Name = "fournisseur"},
                    new Role { Name = "Admin" },
                    new Role { Name = "SuperAdmin" }
                };

                foreach (var role in roles)
                {
                    roleManager.CreateAsync(role).Wait();
                }

                var adminUser = new User
                {
                    UserName = "Admin",
                    FirstName = "admin",
                    LastName = "admin",
                    Email = "adminUser@SmokeEnGrill.com",
                    EmailConfirmed = true,
                    UserTypeId = 1
                };

                IdentityResult result = userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] { "Admin", "SuperAdmin" }).Wait();
                }

                context.SaveChanges();
            }
        }
    }
}