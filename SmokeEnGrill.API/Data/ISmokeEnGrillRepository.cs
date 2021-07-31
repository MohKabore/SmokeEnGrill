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
    string GetAppSubDomain();
    // Task<PagedList<User>> GetUsers(UserParams userParams);
    Task<User> GetUser(int id, bool isCurrentUser);
    Task<Photo> GetPhoto(int id);
    Task<Photo> GetMainPhotoForUser(int userId);
    Task<bool> EmailExist(string email);
    Task<bool> UserNameExist(string userName, int currentUserId);
    Task<bool> AddEmployee(EmployeeForEditDto user);
    Task<bool> EditUserAccount(UserAccountForEditDto user);
    Task<IEnumerable<UserType>> getUserTypes();
    Task<bool> SendEmail(EmailFormDto emailFormDto);
    // bool SendSms(List<string> phoneNumbers, string content);

    Task<IEnumerable<City>> GetAllCities();
    Task<IEnumerable<District>> GetAllGetDistrictsByCityIdCities(int id);

    void AddUserLink(int userId, int parentId);

    Task<User> GetUserByEmail(string email);
    Task<EmailTemplate> GetEmailTemplate(int id);
    Task<SmsTemplate> GetSmsTemplate(int id);
    Task sendOk(int userTypeId, int userId);
    Task<IEnumerable<Setting>> GetSettings();
    Task<IEnumerable<Email>> SetEmailDataForRegistration(IEnumerable<RegistrationEmailDto> emailData, string content, string RegDeadLine);
    Task<Order> GetOrder(int id);
    Task<Email> SetEmailForAccountUpdated(string subject, string content, string lastName, byte gender, string parentEmail, int userId);
    string GetUserIDNumber(int userId, string lastName, string firstName);
    string GetInvoiceNumber(int invoiceId);
    Task<IEnumerable<PaymentType>> GetPaymentTypes();
    Task<IEnumerable<Bank>> GetBanks();
    Task<List<FinOpDto>> GetOrderPayments(int orderId);
    Task<List<OrderLine>> GetOrderLines(int orderId);
    Task<List<Product>> GetActiveProducts();
    Task<User> GetUserByEmailAndLogin(string username, string email);
    Task<Boolean> SendTeacherConfirmEmail(int userId);
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
