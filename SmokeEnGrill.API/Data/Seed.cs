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

            //roleManager.CreateAsync(new Role { Name = "Idemia" }).Wait();

            // var users = context.Users.Where(r => r.TypeEmpId == 3);
            // int step = 0;
            // var lst2 = new List<int>();
            // foreach (var usr in users)
            // {
            //     var doublons = users.Where(r => r.TypeEmpId == usr.TypeEmpId && r.LastName == usr.LastName &&
            //     r.PhoneNumber == usr.PhoneNumber && r.Id != usr.Id);
            //     if (doublons.Count() > 0)
            //         lst2.AddRange(doublons.Select(a => a.Id));
            // }

            // try
            // {
            //     foreach (var userid in lst2.Distinct())
            //     {
            //         var userh = context.UserHistories.Where(a => a.UserId == userid);
            //         context.RemoveRange(userh);
            //         var user = context.Users.FirstOrDefault(a => a.Id == userid);
            //         context.Remove(user);
            //     }
            //     context.SaveChanges();
            // }
            // catch (System.Exception)
            // {

            //     throw;
            // }


            if (!userManager.Users.Any())
            {
                // var userData = System.IOvi.File.ReadAllText("Data/UserSeedData.json");
                // var users = JsonConvert.DeserializeObject<List<User>>(userData);

                //    context.AddRange(regions);
                //                context.SaveChanges();

                var payments = new List<PaymentType>{
                    new PaymentType{Name="Cash"},
                    new PaymentType{Name="Chèque"},
                    new PaymentType{Name="Carte VISA"}
                };
                context.AddRange(payments);


                var methods = new List<DeliveryMethod>{
                    new DeliveryMethod{Name="A domicile"},
                    new DeliveryMethod{Name="Sur site"},
                    new DeliveryMethod{Name="Sur place"}
                };
                context.AddRange(methods);

                var ordertypes = new List<OrderType>{
                    new OrderType{Name="OrderType1"},
                    new OrderType{Name="OrderType2"}
                };
                context.AddRange(ordertypes);

                var countries = new List<Country>{
                    new Country{Name="pays1"},
                    new Country{Name="pays2"}
                };
                context.AddRange(countries);
                

                var cities = new List<City>{
                    new City{Name="ville1"},
                    new City{Name="Ville2"}
                };
                context.AddRange(cities);


                 var districts = new List<District>{
                    new District{Name="district1"},
                    new District{Name="district2"}
                };
                context.AddRange(districts);


                var roles = new List<Role> {
                    new Role { Name = "Supervisor" },
                    new Role { Name = "Operateur" },
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
                    ValidatedCode = true
                };

                IdentityResult result = userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded)
                {
                    var admin = userManager.FindByNameAsync("Admin").Result;
                    userManager.AddToRolesAsync(admin, new[] { "Admin", "SuperAdmin" }).Wait();
                }


                var storetypes = new List<StoreType> {
                    new StoreType {Name = "Magasin"},
                    new StoreType {Name = "Client"},
                    new StoreType {Name = "Personnel"}
                };
                context.AddRange(storetypes);

                var stores = new List<Store> {
                new Store {Name = "MAIN STORE", StoreTypeId=1},

                };
                context.AddRange(stores);

                var inventOptypes = new List<InventOpType>() {
                    new InventOpType{Name ="Entrée Stock"},
                    new InventOpType{Name ="Transfert Stock"},
                    new InventOpType{Name ="Affectation Tablette"},
                    new InventOpType{Name ="Echange Tablette"},
                    new InventOpType{Name ="Panne"},
                    new InventOpType{Name ="Export"},
                    new InventOpType{Name ="En Réparation"},
                    new InventOpType{Name ="Retour Réparation"},
                    new InventOpType{Name ="Maintenance"}
                };
                context.AddRange(inventOptypes);

                context.SaveChanges();
            }
        }
    }
}