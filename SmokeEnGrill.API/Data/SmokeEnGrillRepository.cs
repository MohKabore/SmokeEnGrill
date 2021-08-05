using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SmokeEnGrill.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EducNotes.API.Dtos;
using Microsoft.AspNetCore.Http;
using EducNotes.API.data;
using System.Globalization;
using SmokeEnGrill.API.Helpers;
using EducNotes.API.Models;
using System.Web;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using CloudinaryDotNet.Actions;
using System.Net;
using EducNotes.API.Helpers;

namespace SmokeEnGrill.API.Data
{
  public class SmokeEnGrillRepository : ISmokeEnGrillRepository
  {
    private readonly DataContext _context;
    private readonly IConfiguration _config;
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    public readonly RoleManager<Role> _roleManager;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    int parentRoleId, memberRoleId, moderatorRoleId, adminRoleId, teacherRoleId;
    int employeeConfirmEmailId, resetPwdEmailId, updateAccountEmailId, broadcastTokenTypeId;
    string baseUrl;
    IHttpContextAccessor _httpContext;
    CultureInfo frC = new CultureInfo("fr-FR");
    public readonly ICacheRepository _cache;

    public SmokeEnGrillRepository(DataContext context, IConfiguration config, IEmailSender emailSender,
        UserManager<User> userManager, IMapper mapper, IHttpContextAccessor httpContext,
        RoleManager<Role> roleManager, ICacheRepository cache)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _cache = cache;
      _context = context;
      _httpContext = httpContext;
      _config = config;
      _emailSender = emailSender;
      _mapper = mapper;
      _config = config;
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      adminRoleId = _config.GetValue<int>("AppSettings:adminRoleId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      employeeConfirmEmailId = _config.GetValue<int>("AppSettings:employeeConfirmEmailId");
      baseUrl = _config.GetValue<String>("AppSettings:DefaultLink");
      resetPwdEmailId = _config.GetValue<int>("AppSettings:resetPwdEmailId");
      updateAccountEmailId = _config.GetValue<int>("AppSettings:updateAccountEmailId");
      broadcastTokenTypeId = _config.GetValue<int>("AppSettings:broadcastTokenTypeId");
    }

    public void Add<T>(T entity) where T : class
    {
      _context.Add(entity);
    }

    public async void AddAsync<T>(T entity) where T : class
    {
      await _context.AddAsync(entity);
    }

    public void Update<T>(T entity) where T : class
    {
      _context.Update(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
      _context.Remove(entity);
    }

    public void DeleteAll<T>(List<T> entities) where T : class
    {
      _context.RemoveRange(entities);
    }

    public async Task<bool> SaveAll()
    {
      return await _context.SaveChangesAsync() > 0;
    }
    public async Task<User> GetUser(int id, bool isCurrentUser)
    {
      var query = _context.Users.AsQueryable();
      // .Include(c => c.Class)
      // .Include(p => p.Photos).AsQueryable();

      if (isCurrentUser)
        query = query.IgnoreQueryFilters();

      var user = await query.FirstOrDefaultAsync(u => u.Id == id);

      return user;
    }

    public async Task<User> GetUserByEmail(string email)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Email.ToUpper() == email.ToUpper());
    }

    public async Task<bool> SendEmail(EmailFormDto emailFormDto)
    {
      try
      {
        await _emailSender.SendEmailAsync(emailFormDto.toEmail, emailFormDto.subject, emailFormDto.content);
        return true;
      }
      catch (System.Exception)
      {
        return false;
      }
    }

    private string ResetPasswordContent(string code)
    {
      return "<b>SmokeEnGrill 2020</b> a bien enrgistré votre demande de réinitialisation de mot de passe !<br>" +
          "Vous pouvez utiliser le lien suivant pour réinitialiser votre mot de passe: <br>" +
          " <a href=" + _config.GetValue<String>("AppSettings:DefaultResetPasswordLink") + code + "/>cliquer ici</a><br>" +
          "Si vous n'utilisez pas ce lien dans les 3 heures, il expirera." +
          "Pour obtenir un nouveau lien de réinitialisation de mot de passe, visitez" +
          " <a href=" + _config.GetValue<String>("AppSettings:DefaultforgotPasswordLink") + "/>réinitialiser son mot de passe</a>.<br>" +
          "Merci,";

    }


    // public async Task<PagedList<User>> GetUsers(UserParams userParams)
    // {
    //     var users = _context.Users.Include(p => p.Photos)
    //         .OrderByDescending(u => u.LastActive).AsQueryable();

    //     users = users.Where(u => u.Id != userParams.userId);

    //     users = users.Where(u => u.Gender == userParams.Gender);

    //     if (userParams.Likers)
    //     {
    //         var userLikers = await GetUserLikes(userParams.userId, userParams.Likers);
    //         users = users.Where(u => userLikers.Contains(u.Id));
    //     }

    //     if (userParams.Likees)
    //     {
    //         var userLikees = await GetUserLikes(userParams.userId, userParams.Likers);
    //         users = users.Where(u => userLikees.Contains(u.Id));
    //     }

    //     if (userParams.MinAge != 18 || userParams.MaxAge != 99)
    //     {
    //         var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
    //         var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

    //         users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
    //     }

    //     if (!string.IsNullOrEmpty(userParams.OrderBy))
    //     {
    //         switch (userParams.OrderBy)
    //         {
    //             case "created":
    //                 users = users.OrderByDescending(u => u.Created);
    //                 break;
    //             default:
    //                 users.OrderByDescending(u => u.LastActive);
    //                 break;
    //         }
    //     }

    //     return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
    // }

    public async Task<IEnumerable<UserType>> getUserTypes()
    {
      return await _context.UserTypes.Where(u => u.Name != "Admin").ToListAsync();
    }

    public async Task<Boolean> UserInRole(int userId, int roleId)
    {
      List<UserRole> userRolesCached = await _cache.GetUserRoles();
      UserRole userRole = userRolesCached.FirstOrDefault(u => u.UserId == userId && u.RoleId == roleId);
      return (userRolesCached != null);
    }

    public async Task<List<Token>> GetTokens()
    {
      var tokens = await _cache.GetTokens();
      return tokens;
    }

    public async Task<List<Token>> GetBroadcastTokens()
    {
      List<Token> tokensCached = await _cache.GetTokens();

      var tokens = tokensCached
          .Where(t => t.TokenTypeId == broadcastTokenTypeId)
          .OrderBy(t => t.Name).ToList();
      return tokens;
    }

    public async Task<Email> SetEmailForAccountUpdated(string subject, string content, string lastName,
      byte gender, string parentEmail, int userId)
    {
      List<Setting> settings = await _context.Settings.ToListAsync();
      var schoolName = (settings.First(s => s.Name.ToLower() == "schoolname")).Value;
      var tokens = await GetTokens();

      Email newEmail = new Email();
      newEmail.EmailTypeId = 1;
      newEmail.ToAddress = parentEmail;
      newEmail.FromAddress = "no-reply@educnotes.com";
      newEmail.Subject = subject.Replace("<NOM_ECOLE>", schoolName);
      List<TokenDto> tags = GetAccountUpdatedTokenValues(tokens, lastName, gender);
      newEmail.Body = ReplaceTokens(tags, content);
      newEmail.InsertUserId = 1;
      newEmail.InsertDate = DateTime.Now;
      newEmail.UpdateUserId = 1;
      newEmail.UpdateDate = DateTime.Now;
      newEmail.ToUserId = userId;

      return newEmail;
    }

    public List<TokenDto> GetAccountUpdatedTokenValues(IEnumerable<Token> tokens, string lastName, byte gender)
    {
      List<TokenDto> tokenValues = new List<TokenDto>();

      foreach (var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;
        switch (td.TokenString)
        {
          case "<N_PARENT>":
            td.Value = lastName;
            break;
          case "<M_MME>":
            td.Value = gender == 0 ? "Mme" : "M.";
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public async Task<List<Country>> GetCountries()
    {
      List<Country> countries = await _cache.GetCountries();
      return countries;
    }

    public async Task<List<City>> GetCities()
    {
      List<City> cities = await _cache.GetCities();
      return cities;
    }

    public async Task<List<District>> GetDistricts()
    {
      List<District> districts = await _cache.GetDistricts();
      return districts;
    }

    public async Task<List<Zone>> GetZones()
    {
      List<Zone> zones = await _cache.GetZones();
      return zones;
    }

    public List<TokenDto> GetMessageTokenValues(List<Token> tokens, UserToSendMsgDto user)
    {
      List<TokenDto> tokenValues = new List<TokenDto>();

      foreach(var token in tokens)
      {
        TokenDto td = new TokenDto();
        td.TokenString = token.TokenString;

        switch (td.TokenString)
        {
          case "#N_UTILISATEUR#":
            td.Value = user.LastName.FirstLetterToUpper();
            break;
          case "#P_UTILISATEUR#":
            td.Value = user.FirstName.FirstLetterToUpper();
            break;
          case "#M_MME#":
            td.Value = user.Gender == 0 ? "Mme" : "M.";
            break;
          case "#CELL_UTILISATEUR#":
            td.Value = user.Mobile;
            break;
          case "#EMAIL_UTILISATEUR#":
            td.Value = user.Email;
            break;
          case "#TOKEN#":
            td.Value = user.Token;
            break;
          default:
            break;
        }

        if(td.Value != null)
          tokenValues.Add(td);
      }

      return tokenValues;
    }

    public void AddUserLink(int userId, int parentId)
    {
      var nouveau_link = new UserLink
      {
        UserId = userId,
        UserPId = parentId
      };
      Add(nouveau_link);
    }

    public async Task<Order> GetOrder(int id)
    {
      List<Order> orders = await _cache.GetOrders();
      List<OrderLine> lines = await _cache.GetOrderLines();

      var order = orders.FirstOrDefault(o => o.Id == id);
      order.Lines = lines.Where(o => o.OrderId == order.Id).ToList();

      return order;
    }

    public async Task<IEnumerable<PaymentType>> GetPaymentTypes()
    {
      List<PaymentType> paymentTypesCached = await _cache.GetPaymentTypes();
      var types = paymentTypesCached.ToList();
      return types;
    }

    public async Task<List<PayableAt>> GetPayableAts()
    {
      List<PayableAt> payableAts= await _cache.GetPayableAts();
      return payableAts;
    }

    public async Task<IEnumerable<Bank>> GetBanks()
    {
      List<Bank> banks = await _cache.GetBanks();
      return banks;
    }

    public string ReplaceTokens(List<TokenDto> tokens, string content)
    {
      foreach (var token in tokens)
      {
        content = content.Replace(token.TokenString, token.Value);
      }
      return content;
    }

    public async Task<List<Product>> GetActiveProducts()
    {
      List<Product> productsCached = await _cache.GetProducts();
      var products = productsCached.Where(p => p.Active)
                                   .OrderBy(p => p.Name)
                                   .ToList();
      return products;
    }

    public MsgRecipientsDto setRecipientsList(List<User> users, int msgChoice, Boolean sendToNotValidated)
    {
      MsgRecipientsDto recipients = new MsgRecipientsDto();
      recipients.UsersOK = new List<User>();
      recipients.UsersNOK = new List<User>();
      foreach (var user in users)
      {
        if (msgChoice == 1) //email
        {
          if (!string.IsNullOrEmpty(user.Email) && (user.EmailConfirmed || sendToNotValidated))
          {
            recipients.UsersOK.Add(user);
          }
          else
          {
            recipients.UsersNOK.Add(user);
          }
        }
        else //sms
        {
          if (!string.IsNullOrEmpty(user.PhoneNumber) && (user.PhoneNumberConfirmed || sendToNotValidated))
          {
            recipients.UsersOK.Add(user);
          }
          else
          {
            recipients.UsersNOK.Add(user);
          }
        }
      }

      return recipients;
    }
    public async Task<UserWithRolesDto> GetUserWithRoles(int userId)
    {
      User user = await _context.Users.Include(i => i.Photos).FirstAsync(u => u.Id == userId);
      UserWithRolesDto userWithRoles = new UserWithRolesDto();
      userWithRoles.Id = user.Id;
      userWithRoles.LastName = user.LastName;
      userWithRoles.FirstName = user.FirstName;
      Photo photo = user.Photos.FirstOrDefault(p => p.IsMain == true);
      if (photo != null)
        userWithRoles.PhotoUrl = photo.Url;
      List<UserRole> rolesFromDB = await _context.UserRoles.Include(i => i.Role)
                                                           .Where(r => r.UserId == user.Id)
                                                           .ToListAsync();
      List<Role> roles = rolesFromDB.Select(s => s.Role).ToList();
      userWithRoles.Roles = roles;

      return userWithRoles;
    }

    public async Task<List<MenuItemDto>> GetMenu(int userTypeId)
    {
      List<Menu> menus = await _cache.GetMenus();
      List<MenuItem> menuItemsCached = await _cache.GetMenuItems();

      //get userType menu
      Menu menu = menus.First(m => m.UserTypeId == userTypeId);
      List<MenuItem> menuItemsByMenuId = menuItemsCached.Where(m => m.MenuId == menu.Id).ToList();
      List<MenuItemDto> userMenuItems = _mapper.Map<List<MenuItemDto>>(menuItemsByMenuId);

      List<MenuItemDto> menuItems = new List<MenuItemDto>();
      foreach (var menuItem in userMenuItems)
      {
        //check if the menu already exists in this object
        Boolean menuExists = menuItems.FirstOrDefault(m => m.Id == menuItem.Id) != null;
        if (menuExists == false)
        {
          //doesn't exist so now check if this is a top level item
          if (menuItem.ParentMenuId == null)
          {
            //top level item so just add it
            menuItems.Add(menuItem);
          }
          else
          {
            //get the parent menu item from this object if it exists
            MenuItemDto parent = menuItems.FirstOrDefault(m => m.Id == Convert.ToInt32(menuItem.ParentMenuId));
            if (parent == null)
            {
              //if it gets here then the parent isn't in the list yet. find the parent in the list
              MenuItemDto newParentMenuItem = FindOrLoadParent(userMenuItems, menuItems, Convert.ToInt32(menuItem.ParentMenuId));

              //add the current child menu item to the newly added parent
              newParentMenuItem.ChildMenuItems.Add(menuItem);
              menuItems.Add(newParentMenuItem);
            }
            else
            {
              //parent already existed in this object. add this menu to the child of the parent
              parent.ChildMenuItems.Add(menuItem);
            }
          }
        }
      }

      return menuItems;
    }

    public MenuItemDto FindOrLoadParent(List<MenuItemDto> menuItems, List<MenuItemDto> userMenuItems, int parentMenuItemId)
    {
      //find the menu item in the entity list
      MenuItemDto parentMenuItem = menuItems.Single(m => m.Id == parentMenuItemId);

      //check if it has a parent
      if (parentMenuItem.ParentMenuId != null)
      {
        //since this has a parent it should be added to its parent's children.
        //try to find the parent in the list already
        MenuItemDto parent = userMenuItems.FirstOrDefault(m => m.Id == Convert.ToInt32(parentMenuItem.ParentMenuId));
        if (parent == null)
        {
          //this one's parent wasn't found, so add it
          MenuItemDto newParent = FindOrLoadParent(menuItems, userMenuItems, Convert.ToInt32(parentMenuItem.ParentMenuId));
          newParent.ChildMenuItems.Add(parentMenuItem);
        }
      }

      return parentMenuItem;
    }

    public async Task<List<MenuItemDto>> GetUserTypeMenu(int userTypeId, int userId)
    {
      List<Menu> menus = await _cache.GetMenus();
      List<MenuItem> menuItemsCached = await _cache.GetMenuItems();
      List<UserRole> userRolesCached = await _cache.GetUserRoles();
      List<RoleCapability> roleCapabilities = await _cache.GetRoleCapabilities();

      //get userType menu
      Menu menu = menus.First(m => m.UserTypeId == userTypeId);
      List<MenuItem> menuItemsByMenuId = menuItemsCached.Where(m => m.MenuId == menu.Id).ToList();
      List<MenuItemDto> userMenuItems = _mapper.Map<List<MenuItemDto>>(menuItemsByMenuId);
      List<UserRole> userRolesFromDB = userRolesCached.Where(r => r.UserId == userId).ToList();
      List<Role> userRoles = userRolesFromDB.Select(u => u.Role).ToList();
      List<RoleCapability> capabilities = roleCapabilities.ToList();

      List<MenuItemDto> menuItems = new List<MenuItemDto>();
      foreach (var menuItem in userMenuItems)
      {
        Boolean hasAccessToMenuItem = await HasAccessToMenu(userId, menuItem, userRoles, userMenuItems, capabilities);
        if (hasAccessToMenuItem)
        {
          //check if the menu already exists in this object
          Boolean menuExists = menuItems.FirstOrDefault(m => m.Id == menuItem.Id) != null;
          if (menuExists == false)
          {
            //doesn't exist so now check if this is a top level item
            if (menuItem.ParentMenuId == null)
            {
              //top level item so just add it
              menuItems.Add(menuItem);
            }
            else
            {
              //get the parent menu item from this object if it exists
              MenuItemDto parent = menuItems.FirstOrDefault(m => m.Id == Convert.ToInt32(menuItem.ParentMenuId));
              if (parent == null)
              {
                //if it gets here then the parent isn't in the list yet. find the parent in the list
                MenuItemDto newParentMenuItem = FindOrLoadParent(userMenuItems, menuItems, Convert.ToInt32(menuItem.ParentMenuId));

                //add the current child menu item to the newly added parent
                newParentMenuItem.ChildMenuItems.Add(menuItem);
                menuItems.Add(newParentMenuItem);
              }
              else
              {
                //parent already existed in this object. add this menu to the child of the parent
                parent.ChildMenuItems.Add(menuItem);
              }
            }
          }
        }
      }

      return menuItems;
    }

    public async Task<Boolean> HasAccessToMenu(int userId, MenuItemDto menuItem, List<Role> userRoles, List<MenuItemDto> menuItems, List<RoleCapability> capabilities)
    {
      if (menuItem.IsAlwaysEnabled)
      {
        return true;
      }
      else
      {
        //Loop through all the roles this user is in. The first time the user has
        //access to the menu item return true. If you get through all the
        //roles then the user does not have access to this menu item.
        foreach (Role role in userRoles)
        {
          //check if the user is in this role
          // Boolean userInRole = await UserInRole(userId, role.Id);
          // if(userInRole)
          // {
          // }
          //try to find the capability with the menu item id
          List<RoleCapability> capabilitiesList = capabilities.Where(r => r.Capability.MenuItemId == menuItem.Id && r.RoleId == role.Id)
                                                              .ToList();
          foreach (RoleCapability capability in capabilitiesList)
          {
            if ((capability != null) && (capability.AccessFlag != (byte)Enums.CapabilityAccessFlag.None))
            {
              //If the record is in the table and the user has access other
              //then None then return true.
              return true;
            }
          }
        }
      }

      //If it gets here then the user didn’t have access to this menu item. BUT they
      //may have access to one of its children, now check the children and if they
      //have access to any of them return true.
      List<MenuItemDto> menuChildItems = menuItems.Where(m => m.ParentMenuId == menuItem.Id).ToList();
      if (menuChildItems.Count > 0)
      {
        foreach (MenuItemDto child in menuChildItems)
        {
          Boolean childInRole = await HasAccessToMenu(userId, child, userRoles, menuItems, capabilities);
          if (childInRole)
          {
            return true;
          }
        }
      }

      //if it never found a role with any capability then return false.
      return false;
    }

    public async Task<List<MenuCapabilitiesDto>> GetMenuCapabilities(int userTypeId, int userId)
    {
      List<Capability> capabilities = await _cache.GetCapabilities();
      List<MenuItem> menuItemsCached = await _cache.GetMenuItems();
      List<MenuItemDto> menuItems = await GetMenu(userTypeId);

      List<MenuCapabilitiesDto> menuCapabilities = new List<MenuCapabilitiesDto>();
      foreach (MenuItemDto menuItem in menuItems)
      {
        MenuCapabilitiesDto item = new MenuCapabilitiesDto();
        item.MenuItemId = menuItem.Id;
        item.MenuItemName = menuItem.DisplayName;
        List<Capability> itemCapabilities = capabilities.Where(c => c.MenuItemId == menuItem.Id).ToList();
        item.Capabilities = itemCapabilities;
        menuCapabilities.Add(item);

        if (menuItem.ChildMenuItems.Count() > 0)
        {
          foreach (MenuItemDto child in menuItem.ChildMenuItems)
          {
            MenuCapabilitiesDto childItem = new MenuCapabilitiesDto();
            childItem.MenuItemId = child.Id;
            childItem.MenuItemName = child.DisplayName;
            List<Capability> childItemCapabilities = capabilities.Where(c => c.MenuItemId == child.Id).ToList();
            childItem.Capabilities = childItemCapabilities;
            item.ChildMenuItems.Add(childItem);
          }
        }
      }

      return menuCapabilities;
    }

    public async Task<ErrorDto> SaveRole(RoleDto roleDto)
    {
      DateTime today = DateTime.Now;

      using(var identityContextTransaction = _context.Database.BeginTransaction())
      {
        ErrorDto status = new ErrorDto();
        status.NoError = true;
        try
        {
          Role role = new Role();
          role.Id = roleDto.RoleId;
          role.Name = roleDto.RoleName;

          //is it a new role?
          if (role.Id == 0)
          {
            var result = await _roleManager.CreateAsync(role);
          }
          else
          {
            List<Role> roles = await _cache.GetRoles();
            List<RoleCapability> roleCapabilities = await _cache.GetRoleCapabilities();
            List<UserRole> userRoles = await _cache.GetUserRoles();

            role = roles.First(r => r.Id == roleDto.RoleId);
            role.Name = roleDto.RoleName;
            role.NormalizedName = roleDto.RoleName.ToUpper();
            // role.ConcurrencyStamp =  Guid.NewGuid().ToString();
            Update(role);

            //remove previous role capabilities
            List<RoleCapability> prevRoleCapabilities = roleCapabilities.Where(r => r.RoleId == role.Id).ToList();
            DeleteAll(prevRoleCapabilities);

            //remove previous users in the role
            List<UserRole> prevUsersInRole = userRoles.Where(r => r.RoleId == role.Id).ToList();
            DeleteAll(prevUsersInRole);
          }

          //add role capabilities
          foreach (RoleCapabilityDto capability in roleDto.Capabilities)
          {
            if (capability.AccessFlag > 0)
            {
              RoleCapability rc = new RoleCapability();
              rc.RoleId = role.Id;
              rc.CapabilityId = capability.CapabilityId;
              rc.AccessFlag = capability.AccessFlag;
              rc.InsertUserId = 1;
              rc.InsertDate = today;
              rc.UpdateUserId = 1;
              rc.UpdateDate = today;
              Add(rc);
            }
          }

          foreach (UserInRoleDto user in roleDto.UsersInRole)
          {
            UserRole userRole = new UserRole();
            userRole.UserId = user.Id;
            userRole.RoleId = role.Id;
            Add(userRole);
          }

          await SaveAll();
          identityContextTransaction.Commit();
          return status;
        }
        catch (Exception ex)
        {
          var de = ex.Message;
          identityContextTransaction.Rollback();
          status.Message = ex.Message;
          status.NoError = false;
          return status;
        }
        finally
        {
          await _cache.LoadRoles();
          await _cache.LoadRoleCapabilities();
          await _cache.LoadUserRoles();
        }
      }
    }

    public async Task<List<FinOpDto>> GetOrderPayments(int orderId)
    {
      List<FinOp> finOps = await _cache.GetFinOps();
      var paymentsFromDB = finOps.Where(f => f.OrderId == orderId).ToList();
      var payments = _mapper.Map<List<FinOpDto>>(paymentsFromDB);
      return payments;
    }

    public async Task<List<OrderLine>> GetOrderLines(int orderId)
    {
      List<OrderLine> linesCached = await _cache.GetOrderLines();
      var lines = linesCached.Where(ol => ol.OrderId == orderId).ToList();
      return lines;
    }

    public string GetInvoiceNumber(int invoiceId)
    {
      var today = DateTime.Now;
      string year = today.Year.ToString().Substring(2);
      var todaymonth = today.Month;
      string month = todaymonth.ToString().Length == 1 ? "0" + todaymonth : todaymonth.ToString();
      var todayday = today.Day;
      string day = todayday.ToString().Length == 1 ? "0" + todayday : todayday.ToString();

      var num = year + month + day + "-" + invoiceId.ToString();
      return num;
    }

  }
}