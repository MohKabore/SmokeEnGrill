using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EducNotes.API.Dtos;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Helpers;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Data
{
  public interface  ISmokeEnGrillRepository
  {
    void Add<T>(T entity) where T : class;
    void Update<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    void DeleteAll<T>(List<T> entities) where T : class;
    Task<bool> SaveAll();
    Task<User> GetUser(int id, bool isCurrentUser);
    Task<IEnumerable<UserType>> getUserTypes();
    Task<bool> SendEmail(EmailFormDto emailFormDto);
    void AddUserLink(int userId, int parentId);

    Task<User> GetUserByEmail(string email);
    Task<Order> GetOrder(int id);
    Task<Email> SetEmailForAccountUpdated(string subject, string content, string lastName, byte gender, string parentEmail, int userId);
    string GetInvoiceNumber(int invoiceId);
    Task<IEnumerable<PaymentType>> GetPaymentTypes();
    Task<IEnumerable<Bank>> GetBanks();
    Task<List<FinOpDto>> GetOrderPayments(int orderId);
    Task<List<OrderLine>> GetOrderLines(int orderId);
    Task<List<Product>> GetActiveProducts();
    Task<List<Token>> GetTokens();
    string ReplaceTokens(List<TokenDto> tokens, string content);
    List<TokenDto> GetMessageTokenValues(List<Token> tokens, UserToSendMsgDto user);
    Task<List<Token>> GetBroadcastTokens();
    MsgRecipientsDto setRecipientsList(List<User> users, int msgChoice, Boolean sendToNotValidated);
    Task<Boolean> UserInRole(int userId, int roleId);
    Task<UserWithRolesDto> GetUserWithRoles(int userId);
    Task<List<MenuItemDto>> GetUserTypeMenu(int userTypeId, int userId);
    Task<List<MenuCapabilitiesDto>> GetMenuCapabilities(int userTypeId, int userId);
    Task<ErrorDto> SaveRole(RoleDto user);
    Task<List<District>> GetDistricts();
    Task<List<City>> GetCities();
    Task<List<Country>> GetCountries();
    Task<List<Zone>> GetZones();
    Task<List<PayableAt>> GetPayableAts();
  }
}
