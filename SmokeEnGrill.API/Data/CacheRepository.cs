using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EducNotes.API.Helpers;
using EducNotes.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SmokeEnGrill.API.Data;
using SmokeEnGrill.API.Models;

namespace EducNotes.API.data
{
  public class CacheRepository : ICacheRepository
  {
    private readonly DataContext _context;
    private readonly IMemoryCache _cache;
    int teacherTypeId, parentTypeId, studentTypeId, adminTypeId;
    public readonly IConfiguration _config;
    public string subDomain;
    public readonly IHttpContextAccessor _httpContext;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
      private ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();

    public CacheRepository(DataContext context, IConfiguration config, IMemoryCache memoryCache,
      IHttpContextAccessor httpContext, RoleManager<Role> roleManager, UserManager<User> userManager)
    {
      _roleManager = roleManager;
      _userManager = userManager;
      _httpContext = httpContext;
      _config = config;
      _context = context;
      _cache = memoryCache;
      teacherTypeId = _config.GetValue<int>("AppSettings:teacherTypeId");
      parentTypeId = _config.GetValue<int>("AppSettings:parentTypeId");
      adminTypeId = _config.GetValue<int>("AppSettings:adminTypeId");
      studentTypeId = _config.GetValue<int>("AppSettings:studentTypeId");
      string[] fullAddress = _httpContext.HttpContext?.Request?.Headers?["Host"].ToString()?.Split('.');
      if (fullAddress != null)
      {
        subDomain = fullAddress[0].ToLower();
        if (subDomain == "localhost:5000" || subDomain == "test2")
        {
          subDomain = "educnotes";
        }
        else if (subDomain == "test1" || subDomain == "www" || subDomain == "educnotes")
        {
          subDomain = "demo";
        }
      }
    }

    public async Task<List<User>> GetUsers()
    {
      List<User> users = new List<User>();

      string key = subDomain + CacheKeys.Users;
      // Look for cache key.
      if (!_cache.TryGetValue(key, out users))
      {
          SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
          await mylock.WaitAsync();
          try
          {
            // Look for cache key.
            if (!_cache.TryGetValue(key, out users))
            {
              // Key not in cache, so get data.
              users = await LoadUsers();
            }
          }
          finally
          {
            mylock.Release();
          }
      }

      return users;
    }

    public async Task<List<User>> GetStudents()
    {
      List<User> students = (await GetUsers()).Where(u => u.UserTypeId == studentTypeId).ToList();
      return students;
    }

    public async Task<List<User>> GetParents()
    {
      List<User> parents = (await GetUsers()).Where(u => u.UserTypeId == parentTypeId).ToList();
      return parents;
    }

    public async Task<List<User>> GetTeachers()
    {
      List<User> teachers = (await GetUsers()).Where(u => u.UserTypeId == teacherTypeId).ToList();
      return teachers;
    }

    public async Task<List<User>> GetEmployees()
    {
      List<User> employees = (await GetUsers()).Where(u => u.UserTypeId == adminTypeId).ToList();
      return employees;
    }

    public async Task<List<User>> LoadUsers()
    {
      // List<User> users = await _context.Users
      List<User> users = await _userManager.Users.Include(p => p.Photos)
                                                 .Include(c => c.District)
                                                 .Include(i => i.UserType)
                                                 .OrderBy(o => o.LastName).ThenBy(o => o.FirstName)
                                                 .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Users);
      _cache.Set(subDomain + CacheKeys.Users, users, cacheEntryOptions);

      return users;
    }

    public async Task<List<UserRole>> GetUserRoles()
    {
      List<UserRole> userRoles = new List<UserRole>();

      string key = subDomain + CacheKeys.UserRoles;
      // Look for cache key.
      if (!_cache.TryGetValue(key, out userRoles))
      {
          SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
          await mylock.WaitAsync();
          try
          {
            // Look for cache key.
            if (!_cache.TryGetValue(key, out userRoles))
            {
              // Key not in cache, so get data.
              userRoles = await LoadUserRoles();
            }
          }
          finally
          {
            mylock.Release();
          }
      }

      return userRoles;
    }

    public async Task<List<UserRole>> LoadUserRoles()
    {
      List<UserRole> userRoles = await _context.UserRoles.Include(p => p.User).ThenInclude(i => i.Photos)
                                                         .Include(i => i.Role)
                                                         .OrderBy(o => o.Role.Name)
                                                         .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.UserRoles);
      _cache.Set(subDomain + CacheKeys.UserRoles, userRoles, cacheEntryOptions);

      return userRoles;
    }

    public async Task<List<EmailTemplate>> GetEmailTemplates()
    {
      List<EmailTemplate> templates = new List<EmailTemplate>();
      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.EmailTemplates, out templates))
      {
        // Key not in cache, so get data.
        templates = await LoadEmailTemplates();
      }
      return templates;
    }

    public async Task<List<EmailTemplate>> LoadEmailTemplates()
    {
      List<EmailTemplate> templates = await _context.EmailTemplates
                                            .Include(i => i.EmailCategory)
                                            .OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.EmailTemplates);
      _cache.Set(subDomain + CacheKeys.EmailTemplates, templates, cacheEntryOptions);

      return templates;
    }

    public async Task<List<SmsTemplate>> GetSmsTemplates()
    {
      List<SmsTemplate> templates = new List<SmsTemplate>();
      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.SmsTemplates, out templates))
      {
        // Key not in cache, so get data.
        templates = await LoadSmsTemplates();
      }
      return templates;
    }

    public async Task<List<SmsTemplate>> LoadSmsTemplates()
    {
      List<SmsTemplate> templates = await _context.SmsTemplates
                                            .Include(i => i.SmsCategory)
                                            .OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.SmsTemplates);
      _cache.Set(subDomain + CacheKeys.SmsTemplates, templates, cacheEntryOptions);

      return templates;
    }

    public async Task<List<Setting>> GetSettings()
    {
      List<Setting> settings = new List<Setting>();
      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Settings, out settings))
      {
        // Key not in cache, so get data.
        settings = await LoadSettings();
      }
      return settings;
    }

    public async Task<List<Setting>> LoadSettings()
    {
      List<Setting> settings = await _context.Settings.OrderBy(o => o.DisplayName).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Settings);
      _cache.Set(subDomain + CacheKeys.Settings, settings, cacheEntryOptions);

      return settings;
    }

    public async Task<List<Token>> GetTokens()
    {
      List<Token> tokens = new List<Token>();
      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Tokens, out tokens))
      {
        // Key not in cache, so get data.
        tokens = await LoadTokens();
      }
      return tokens;
    }

    public async Task<List<Token>> LoadTokens()
    {
      List<Token> tokens = await _context.Tokens.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Tokens);
      _cache.Set(subDomain + CacheKeys.Tokens, tokens, cacheEntryOptions);

      return tokens;
    }

    public async Task<List<Role>> GetRoles()
    {
      List<Role> roles = new List<Role>();

      string key = subDomain + CacheKeys.Roles;
      if (!_cache.TryGetValue(key, out roles))
      {
          SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
          await mylock.WaitAsync();
          try
          {
            // Look for cache key.
            if (!_cache.TryGetValue(key, out roles))
            {
              // Key not in cache, so get data.
              roles = await LoadRoles();
            }
          }
          finally
          {
            mylock.Release();
          }
      }

      return roles;
    }

    public async Task<List<Role>> LoadRoles()
    {
      List<Role> roles = await _context.Roles.OrderBy(o => o.Name).ToListAsync();
      // List<Role> roles = await _roleManager.Roles.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Roles);
      _cache.Set(subDomain + CacheKeys.Roles, roles, cacheEntryOptions);

      return roles;
    }

    public async Task<List<Order>> GetOrders()
    {
      List<Order> orders = new List<Order>();

      string key = subDomain + CacheKeys.Orders;
      if (!_cache.TryGetValue(key, out orders))
      {
          SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
          await mylock.WaitAsync();
          try
          {
            // Look for cache key.
            if (!_cache.TryGetValue(key, out orders))
            {
              // Key not in cache, so get data.
              orders = await LoadOrders();
            }
          }
          finally
          {
            mylock.Release();
          }
      }

      return orders;
    }

    public async Task<List<Order>> LoadOrders()
    {
      List<Order> orders = await _context.Orders.Include(i => i.Client)
                                                .Include(i => i.Supplier).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Orders);
      _cache.Set(subDomain + CacheKeys.Orders, orders, cacheEntryOptions);

      return orders;
    }

    public async Task<List<OrderLine>> GetOrderLines()
    {
      List<OrderLine> lines = new List<OrderLine>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.OrderLines, out lines))
      {
        // Key not in cache, so get data.
        lines = await LoadOrderLines();
      }

      return lines;
    }

    public async Task<List<OrderLine>> LoadOrderLines()
    {
      List<OrderLine> lines = await _context.OrderLines
        .Include(i => i.Order)
        .Include(i => i.Product)
        .Include(i => i.Product)
        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.OrderLines);
      _cache.Set(subDomain + CacheKeys.OrderLines, lines, cacheEntryOptions);

      return lines;
    }

    public async Task<List<UserLink>> GetUserLinks()
    {
      List<UserLink> userlinks = new List<UserLink>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.UserLinks, out userlinks))
      {
        // Key not in cache, so get data.
        userlinks = await LoadUserLinks();
      }

      return userlinks;
    }

    public async Task<List<UserLink>> LoadUserLinks()
    {
      List<UserLink> userlinks = await _context.UserLinks.ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.UserLinks);
      _cache.Set(subDomain + CacheKeys.UserLinks, userlinks, cacheEntryOptions);

      return userlinks;
    }

    public async Task<List<FinOp>> GetFinOps()
    {
      List<FinOp> finops = new List<FinOp>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.FinOps, out finops))
      {
        // Key not in cache, so get data.
        finops = await LoadFinOps();
      }

      return finops;
    }

    public async Task<List<FinOp>> LoadFinOps()
    {
      List<FinOp> finops = await _context.FinOps
        .Include(i => i.Cheque).ThenInclude(i => i.Bank)
        .Include(i => i.PaymentType)
        .Include(i => i.Invoice)
        .Include(i => i.FromBank)
        .Include(i => i.FromBankAccount)
        .Include(i => i.FromCashDesk)
        .Include(i => i.ToBankAccount)
        .Include(i => i.ToCashDesk)
        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.FinOps);
      _cache.Set(subDomain + CacheKeys.FinOps, finops, cacheEntryOptions);

      return finops;
    }

    public async Task<List<FinOpOrderline>> GetFinOpOrderLines()
    {
      List<FinOpOrderline> finoporderlines = new List<FinOpOrderline>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.FinOpOrderLines, out finoporderlines))
      {
        // Key not in cache, so get data.
        finoporderlines = await LoadFinOpOrderLines();
      }

      return finoporderlines;
    }

    public async Task<List<FinOpOrderline>> LoadFinOpOrderLines()
    {
      List<FinOpOrderline> finoporderlines = await _context.FinOpOrderlines
        .Include(d => d.Invoice)
        .Include(o => o.OrderLine).ThenInclude(p => p.Product)
        .Include(i => i.FinOp).ThenInclude(i => i.Cheque).ThenInclude(i => i.Bank)
        .Include(i => i.FinOp).ThenInclude(i => i.PaymentType)
        .Include(i => i.FinOp).ThenInclude(i => i.FromBankAccount)
        .Include(i => i.FinOp).ThenInclude(i => i.ToBankAccount)
        .Include(i => i.FinOp).ThenInclude(i => i.FromCashDesk)
        .Include(i => i.FinOp).ThenInclude(i => i.ToCashDesk)
        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.FinOpOrderLines);
      _cache.Set(subDomain + CacheKeys.FinOpOrderLines, finoporderlines, cacheEntryOptions);

      return finoporderlines;
    }

    public async Task<List<Cheque>> GetCheques()
    {
      List<Cheque> cheques = new List<Cheque>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Cheques, out cheques))
      {
        // Key not in cache, so get data.
        cheques = await LoadCheques();
      }

      return cheques;
    }

    public async Task<List<Cheque>> LoadCheques()
    {
      List<Cheque> cheques = await _context.Cheques.ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Cheques);
      _cache.Set(subDomain + CacheKeys.Cheques, cheques, cacheEntryOptions);

      return cheques;
    }

    public async Task<List<Bank>> GetBanks()
    {
      List<Bank> banks = new List<Bank>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Banks, out banks))
      {
        // Key not in cache, so get data.
        banks = await LoadBanks();
      }

      return banks;
    }

    public async Task<List<Bank>> LoadBanks()
    {
      List<Bank> banks = await _context.Banks.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Banks);
      _cache.Set(subDomain + CacheKeys.Banks, banks, cacheEntryOptions);

      return banks;
    }

    public async Task<List<PaymentType>> GetPaymentTypes()
    {
      List<PaymentType> paymentTypes = new List<PaymentType>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.PaymentTypes, out paymentTypes))
      {
        // Key not in cache, so get data.
        paymentTypes = await LoadPaymentTypes();
      }

      return paymentTypes;
    }

    public async Task<List<PaymentType>> LoadPaymentTypes()
    {
      List<PaymentType> paymentTypes = await _context.PaymentTypes.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.PaymentTypes);
      _cache.Set(subDomain + CacheKeys.PaymentTypes, paymentTypes, cacheEntryOptions);

      return paymentTypes;
    }

    public async Task<List<Product>> GetProducts()
    {
      List<Product> products = new List<Product>();

      string key = subDomain + CacheKeys.Products;
      if (!_cache.TryGetValue(key, out products))
      {
        SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
        await mylock.WaitAsync();
        try
        {
          // Look for cache key.
          if (!_cache.TryGetValue(key, out products))
          {
            // Key not in cache, so get data.
            products = await LoadProducts();
          }
        }
        finally
        {
          mylock.Release();
        }
      }

      return products;
    }

    public async Task<List<Product>> LoadProducts()
    {
      List<Product> products = await _context.Products.Include(i => i.ProductType)
                                                      .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Products);
      _cache.Set(subDomain + CacheKeys.Products, products, cacheEntryOptions);

      return products;
    }

    public async Task<List<ProductType>> GetProductTypes()
    {
      List<ProductType> productTypes = new List<ProductType>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.ProductTypes, out productTypes))
      {
        // Key not in cache, so get data.
        productTypes = await LoadProductTypes();
      }

      return productTypes;
    }

    public async Task<List<ProductType>> LoadProductTypes()
    {
      List<ProductType> productTypes = await _context.ProductTypes.OrderBy(o => o.Name)
                                                                  .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.ProductTypes);
      _cache.Set(subDomain + CacheKeys.ProductTypes, productTypes, cacheEntryOptions);

      return productTypes;
    }

    public async Task<List<UserType>> GetUserTypes()
    {
      List<UserType> usertypes = new List<UserType>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.UserTypes, out usertypes))
      {
        // Key not in cache, so get data.
        usertypes = await LoadUserTypes();
      }

      return usertypes;
    }

    public async Task<List<UserType>> LoadUserTypes()
    {
      List<UserType> usertypes = await _context.UserTypes
                                        .Include(i => i.Users)
                                        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.UserTypes);
      _cache.Set(subDomain + CacheKeys.UserTypes, usertypes, cacheEntryOptions);

      return usertypes;
    }

    public async Task<List<Menu>> GetMenus()
    {
      List<Menu> menus = new List<Menu>();

      string key = subDomain + CacheKeys.Menus;
      if (!_cache.TryGetValue(key, out menus))
      {
        SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
        await mylock.WaitAsync();
        try
        {
          // Look for cache key.
          if (!_cache.TryGetValue(key, out menus))
          {
            // Key not in cache, so get data.
            menus = await LoadMenus();
          }
        }
        finally
        {
          mylock.Release();
        }
      }

      return menus;
    }

    public async Task<List<Menu>> LoadMenus()
    {
      List<Menu> menus = await _context.Menus
                                        .Include(i => i.UserType)
                                        .OrderBy(o => o.Name)
                                        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Menus);
      _cache.Set(subDomain + CacheKeys.Menus, menus, cacheEntryOptions);

      return menus;
    }

    public async Task<List<MenuItem>> GetMenuItems()
    {
      List<MenuItem> menuItems = new List<MenuItem>();

      string key = subDomain + CacheKeys.MenuItems;
      if (!_cache.TryGetValue(key, out menuItems))
      {
        SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
        await mylock.WaitAsync();
        try
        {
          // Look for cache key.
          if (!_cache.TryGetValue(key, out menuItems))
          {
            // Key not in cache, so get data.
            menuItems = await LoadMenuItems();
          }
        }
        finally
        {
          mylock.Release();
        }
      }

      return menuItems;
    }

    public async Task<List<MenuItem>> LoadMenuItems()
    {
      List<MenuItem> menuItems = await _context.MenuItems
                                        .Include(i => i.ParentMenu)
                                        .OrderBy(o => o.ParentMenuId).ThenBy(o => o.DsplSeq).ThenBy(o => o.DisplayName)
                                        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.MenuItems);
      _cache.Set(subDomain + CacheKeys.MenuItems, menuItems, cacheEntryOptions);

      return menuItems;
    }

    public async Task<List<Capability>> GetCapabilities()
    {
      List<Capability> capabilities = new List<Capability>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Capabilities, out capabilities))
      {
        // Key not in cache, so get data.
        capabilities = await LoadCapabilities();
      }

      return capabilities;
    }

    public async Task<List<Capability>> LoadCapabilities()
    {
      List<Capability> capabilities = await _context.Capabilities
                                        .Include(i => i.MenuItem)
                                        .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Capabilities);
      _cache.Set(subDomain + CacheKeys.Capabilities, capabilities, cacheEntryOptions);

      return capabilities;
    }

    public async Task<List<RoleCapability>> GetRoleCapabilities()
    {
      List<RoleCapability> roleCapabilities = new List<RoleCapability>();

      string key = subDomain + CacheKeys.RoleCapabilities;
      // Look for cache key.
      if (!_cache.TryGetValue(key, out roleCapabilities))
      {
        SemaphoreSlim mylock = _locks.GetOrAdd(key, k => new SemaphoreSlim(1, 1));
        await mylock.WaitAsync();
        try
        {
          // Look for cache key.
          if (!_cache.TryGetValue(key, out roleCapabilities))
          {
            // Key not in cache, so get data.
            roleCapabilities = await LoadRoleCapabilities();
          }
        }
        finally
        {
          mylock.Release();
        }
      }

      return roleCapabilities;
    }

    public async Task<List<RoleCapability>> LoadRoleCapabilities()
    {
      List<RoleCapability> roleCapabilities = await _context.RoleCapabilities.Include(i => i.Role)
                                                                             .Include(i => i.Capability)
                                                                             .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.RoleCapabilities);
      _cache.Set(subDomain + CacheKeys.RoleCapabilities, roleCapabilities, cacheEntryOptions);

      return roleCapabilities;
    }

    public async Task<List<Country>> GetCountries()
    {
      List<Country> countries = new List<Country>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Countries, out countries))
      {
        // Key not in cache, so get data.
        countries = await LoadCountries();
      }

      return countries;
    }

    public async Task<List<Country>> LoadCountries()
    {
      List<Country> countries = await _context.Countries.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Countries);
      _cache.Set(subDomain + CacheKeys.Countries, countries, cacheEntryOptions);

      return countries;
    }

    public async Task<List<City>> GetCities()
    {
      List<City> cities = new List<City>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Cities, out cities))
      {
        // Key not in cache, so get data.
        cities = await LoadCities();
      }

      return cities;
    }

    public async Task<List<City>> LoadCities()
    {
      List<City> cities = await _context.Cities.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Cities);
      _cache.Set(subDomain + CacheKeys.Cities, cities, cacheEntryOptions);

      return cities;
    }

    public async Task<List<District>> GetDistricts()
    {
      List<District> districts = new List<District>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Districts, out districts))
      {
        // Key not in cache, so get data.
        districts = await LoadDistricts();
      }

      return districts;
    }

    public async Task<List<District>> LoadDistricts()
    {
      List<District> districts = await _context.Districts.Include(i => i.City).OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Districts);
      _cache.Set(subDomain + CacheKeys.Districts, districts, cacheEntryOptions);

      return districts;
    }

    public async Task<List<Photo>> GetPhotos()
    {
      List<Photo> photos = new List<Photo>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Photos, out photos))
      {
        // Key not in cache, so get data.
        photos = await LoadPhotos();
      }

      return photos;
    }

    public async Task<List<Photo>> LoadPhotos()
    {
      List<Photo> photos = await _context.Photos.ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Photos);
      _cache.Set(subDomain + CacheKeys.Photos, photos, cacheEntryOptions);

      return photos;
    }

    public async Task<List<Zone>> GetZones()
    {
      List<Zone> zones = new List<Zone>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.Zones, out zones))
      {
        // Key not in cache, so get data.
        zones = await LoadZones();
      }

      return zones;
    }

    public async Task<List<Zone>> LoadZones()
    {
      List<Zone> zones = await _context.Zones.OrderBy(o => o.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.Zones);
      _cache.Set(subDomain + CacheKeys.Zones, zones, cacheEntryOptions);

      return zones;
    }

    public async Task<List<LocationZone>> GetLocationZones()
    {
      List<LocationZone> locationzones = new List<LocationZone>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.LocationZones, out locationzones))
      {
        // Key not in cache, so get data.
        locationzones = await LoadLocationZones();
      }

      return locationzones;
    }

    public async Task<List<LocationZone>> LoadLocationZones()
    {
      List<LocationZone> locationzones = await _context.LocationZones.Include(i => i.District)
                                                                     .Include(i => i.City)
                                                                     .Include(i => i.Country)
                                                                     .ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.LocationZones);
      _cache.Set(subDomain + CacheKeys.LocationZones, locationzones, cacheEntryOptions);

      return locationzones;
    }

    public async Task<List<PayableAt>> GetPayableAts()
    {
      List<PayableAt> payableAts = new List<PayableAt>();

      // Look for cache key.
      if (!_cache.TryGetValue(subDomain + CacheKeys.PayableAts, out payableAts))
      {
        // Key not in cache, so get data.
        payableAts = await LoadPayableAts();
      }

      return payableAts;
    }

    public async Task<List<PayableAt>> LoadPayableAts()
    {
      List<PayableAt> payableAts = await _context.PayableAts.OrderBy(p => p.Name).ToListAsync();

      // Set cache options.
      var cacheEntryOptions = new MemoryCacheEntryOptions()
        // Keep in cache for this time, reset time if accessed.
        .SetSlidingExpiration(TimeSpan.FromDays(7));

      // Save data in cache.
      _cache.Remove(subDomain + CacheKeys.PayableAts);
      _cache.Set(subDomain + CacheKeys.PayableAts, payableAts, cacheEntryOptions);

      return payableAts;
    }
  }
}