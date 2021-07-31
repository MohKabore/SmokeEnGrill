using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EducNotes.API.Models;
using SmokeEnGrill.API.Models;

namespace EducNotes.API.data
{
  public interface ICacheRepository
  {
    Task<List<User>> GetUsers();
    Task<List<User>> GetEmployees();
    Task<List<User>> LoadUsers();
    Task<List<EmailTemplate>> GetEmailTemplates();
    Task<List<EmailTemplate>> LoadEmailTemplates();
    Task<List<SmsTemplate>> GetSmsTemplates();
    Task<List<SmsTemplate>> LoadSmsTemplates();
    Task<List<Setting>> GetSettings();
    Task<List<Setting>> LoadSettings();
    Task<List<Token>> GetTokens();
    Task<List<Token>> LoadTokens();
    Task<List<Role>> GetRoles();
    Task<List<Role>> LoadRoles();
    Task<List<Order>> GetOrders();
    Task<List<Order>> LoadOrders();
    Task<List<OrderLine>> GetOrderLines();
    Task<List<OrderLine>> LoadOrderLines();
    Task<List<UserLink>> GetUserLinks();
    Task<List<UserLink>> LoadUserLinks();
    Task<List<FinOp>> GetFinOps();
    Task<List<FinOp>> LoadFinOps();
    Task<List<FinOpOrderline>> GetFinOpOrderLines();
    Task<List<FinOpOrderline>> LoadFinOpOrderLines();
    Task<List<Cheque>> GetCheques();
    Task<List<Cheque>> LoadCheques();
    Task<List<Bank>> GetBanks();
    Task<List<Bank>> LoadBanks();
    Task<List<PaymentType>> GetPaymentTypes();
    Task<List<PaymentType>> LoadPaymentTypes();
    Task<List<Product>> GetProducts();
    Task<List<Product>> LoadProducts();
    Task<List<ProductType>> GetProductTypes();
    Task<List<ProductType>> LoadProductTypes();
    Task<List<UserType>> GetUserTypes();
    Task<List<UserType>> LoadUserTypes();
    Task<List<Menu>> GetMenus();
    Task<List<Menu>> LoadMenus();
    Task<List<MenuItem>> GetMenuItems();
    Task<List<MenuItem>> LoadMenuItems();
    Task<List<Capability>> GetCapabilities();
    Task<List<Capability>> LoadCapabilities();
    Task<List<UserRole>> GetUserRoles();
    Task<List<UserRole>> LoadUserRoles();
    Task<List<Country>> GetCountries();
    Task<List<Country>> LoadCountries();
    Task<List<City>> GetCities();
    Task<List<City>> LoadCities();
    Task<List<District>> GetDistricts();
    Task<List<District>> LoadDistricts();
    Task<List<Photo>> GetPhotos();
    Task<List<Photo>> LoadPhotos();
    Task<List<RoleCapability>> GetRoleCapabilities();
    Task<List<RoleCapability>> LoadRoleCapabilities();
    Task<List<Zone>> GetZones();
    Task<List<Zone>> LoadZones();
    Task<List<LocationZone>> GetLocationZones();
    Task<List<LocationZone>> LoadLocationZones();
    Task<List<PayableAt>> GetPayableAts();
    Task<List<PayableAt>> LoadPayableAts();
  }
}