using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
using EducNotes.API.Dtos;
using SmokeEnGrill.API.Data;
using SmokeEnGrill.API.Dtos;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        private readonly DataContext _context;
        CultureInfo frC = new CultureInfo("fr-FR");

        public AutoMapperProfiles(DataContext context)
        {
            _context = context;

        }
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForAutoCompleteDto>()
                .ForMember(dest => dest.UserTypeName, opt => {
                    opt.MapFrom(src => src.UserType.Name);
                });
            CreateMap<User, UserForCallSheetDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                });
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                })
                .ForMember(dest => dest.PhoneNumber, opt => {
                    opt.MapFrom(d => d.PhoneNumber.FormatPhoneNumber());
                })
                .ForMember(dest => dest.strDateOfBirth, opt => {
                    opt.MapFrom(d => d.DateOfBirth.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strCreated, opt => {
                    opt.MapFrom(src => src.Created.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strGender, opt => {
                    opt.MapFrom(src => src.Gender == 0 ? "femme" : "homme");
                })
                .ForMember(dest => dest.DistrictName, opt => {
                    opt.MapFrom(src => src.District.Name);
                });
            CreateMap<User, UserForAccountDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                })
                .ForMember(dest => dest.PhoneNumber, opt => {
                    opt.MapFrom(d => d.PhoneNumber.FormatPhoneNumber());
                })
                .ForMember(dest => dest.SecondPhoneNumber, opt => {
                    opt.MapFrom(d => d.SecondPhoneNumber.FormatPhoneNumber());
                })
                .ForMember(dest => dest.UserTypeName, opt => {
                    opt.MapFrom(src => src.UserType.Name);
                });
            CreateMap<MenuItem, MenuItemDto>();
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<Product, ProductForConfigDto>()
                .ForMember(dest => dest.TypeName, opt => opt
                  .MapFrom(u => u.ProductType.Name))
            CreateMap<EmployeeForEditDto, User>();
            CreateMap<User, EmployeeForEditDto>()
                .ForMember(u => u.PhotoUrl, opt => opt
                    .MapFrom(u => u.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.strDateOfBirth, opt => {
                    opt.MapFrom(d => d.DateOfBirth.ToString("dd/MM/yyyy", frC));
                });
            CreateMap<ImportUserDto, User>();
            CreateMap<QuickStudentAssignmentDto, User>();
            CreateMap<UserFromExelDto, User>();
            CreateMap<Email, EmailForListDto>()
                .ForMember(dest => dest.EmailType, opt => {
                  opt.MapFrom(d => d.EmailType.Name);
                })
                .ForMember(dest => dest.SentBy, opt => {
                  opt.MapFrom(d => d.InsertUser.LastName + ' ' + d.InsertUser.FirstName);
                })
                .ForMember(dest => dest.DateSent, opt => {
                  opt.MapFrom(d => d.InsertDate.ToString("dd/MM/yyyy", frC));
                });
            CreateMap<User, UserToValidateDto>()
                .ForMember(dest => dest.UserType, opt => {
                  opt.MapFrom(d => d.UserType.Name);
                });
            CreateMap<SmsTemplateForSaveDto, SmsTemplate>();
            // CreateMap<SmsTemplate, SmsTemplateForListDto>();
            CreateMap<EmailTemplateForSaveDto, EmailTemplate>();
            // CreateMap<EmailTemplate, EmailTemplateForListDto>()
            //     .ForMember(dest => dest.EmailCategoryName, opt => {
            //       opt.MapFrom(src => src.EmailCategory.Name);
            //     });
            CreateMap<ProductDto, Product>();
            CreateMap<PayableDto, PayableAt>();
            CreateMap<FinOp, FinOpDto>()
                .ForMember(dest => dest.InvoiceNum, opt => {
                    opt.MapFrom(src => src.Invoice.InvoiceNum);
                })
                .ForMember(dest => dest.strFinOpDate, opt => {
                    opt.MapFrom(src => src.FinOpDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strInvoiceDate, opt => {
                    opt.MapFrom(src => src.Invoice.InvoiceDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.InvoiceAmount, opt => {
                    opt.MapFrom(src => src.Invoice.Amount);
                })
                .ForMember(dest => dest.ChequeNum, opt => {
                    opt.MapFrom(src => src.Cheque.ChequeNum);
                })
                .ForMember(dest => dest.ChequeBankName, opt => {
                    opt.MapFrom(src => src.Cheque.Bank.Name);
                })
                .ForMember(dest => dest.ChequeAmount, opt => {
                    opt.MapFrom(src => src.Cheque.Amount);
                })
                .ForMember(dest => dest.ChequePictureUrl, opt => {
                  opt.MapFrom(src => src.Cheque.PictureUrl);
                })
                .ForMember(dest => dest.FromBankName, opt => {
                  opt.MapFrom(src => src.FromBank.Name);
                })
                .ForMember(dest => dest.strAmount, opt => {
                  opt.MapFrom(src => src.Amount.ToString("N0") + " F");
                })
                .ForMember(dest => dest.PaymentTypeName, opt => {
                  opt.MapFrom(src => src.PaymentType.Name);
                })
                .ForMember(dest => dest.FromBankAccountName, opt => {
                  opt.MapFrom(src => src.FromBankAccount.Name);
                })
                .ForMember(dest => dest.FromCashDeskName, opt => {
                  opt.MapFrom(src => src.FromCashDesk.Name);
                })
                .ForMember(dest => dest.ToBankAccountName, opt => {
                  opt.MapFrom(src => src.ToBankAccount.Name);
                })
                .ForMember(dest => dest.ToCashDeskName, opt => {
                  opt.MapFrom(src => src.ToCashDesk.Name);
                });
            CreateMap<FinOpOrderline, PaymentDto>()
                .ForMember(dest => dest.InvoiceNum, opt => {
                  opt.MapFrom(src => src.Invoice.InvoiceNum);
                })
                .ForMember(dest => dest.FinOpTypeId, opt => {
                  opt.MapFrom(src => src.FinOp.FinOpTypeId);
                })
                .ForMember(dest => dest.ProductId, opt => {
                  opt.MapFrom(src => src.OrderLine.Product.Id);
                })
                .ForMember(dest => dest.ProductName, opt => {
                  opt.MapFrom(src => src.OrderLine.Product.Name);
                })
                .ForMember(dest => dest.PaymentTypeId, opt => {
                  opt.MapFrom(src => src.FinOp.PaymentTypeId);
                })
                .ForMember(dest => dest.TypeName, opt => {
                  opt.MapFrom(src => src.FinOp.PaymentType.Name);
                })
                .ForMember(dest => dest.ChequeNum, opt => {
                  opt.MapFrom(src => src.FinOp.Cheque.ChequeNum);
                })
                .ForMember(dest => dest.ChequeBank, opt => {
                  opt.MapFrom(src => src.FinOp.Cheque.Bank.Name);
                })
                .ForMember(dest => dest.strFinOpDate, opt => {
                  opt.MapFrom(src => src.FinOp.FinOpDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.FromBankAccount, opt => {
                  opt.MapFrom(src => src.FinOp.FromBankAccount.Name);
                })
                .ForMember(dest => dest.FromCashDesk, opt => {
                  opt.MapFrom(src => src.FinOp.FromCashDesk.Name);
                })
                .ForMember(dest => dest.ToBankAccount, opt => {
                  opt.MapFrom(src => src.FinOp.ToBankAccount.Name);
                })
                .ForMember(dest => dest.ToCashDesk, opt => {
                  opt.MapFrom(src => src.FinOp.ToCashDesk.Name);
                })
                .ForMember(dest => dest.Cashed, opt => {
                  opt.MapFrom(src => src.FinOp.Cashed);
                })
                .ForMember(dest => dest.Received, opt => {
                  opt.MapFrom(src => src.FinOp.Received);
                })
                .ForMember(dest => dest.DepositedToBank, opt => {
                  opt.MapFrom(src => src.FinOp.DepositedToBank);
                })
                .ForMember(dest => dest.Rejected, opt => {
                  opt.MapFrom(src => src.FinOp.Rejected);
                });
            CreateMap<OrderLine, OrderLineDto>()
                .ForMember(dest => dest.ProductName, opt => {
                  opt.MapFrom(src => src.Product.Name);
                })
                .ForMember(dest => dest.NbDaysLate, opt => {
                  opt.MapFrom(src => (src.Validity - DateTime.Now).TotalDays + 1);
                })
                .ForMember(dest => dest.strAmountHT, opt => {
                  opt.MapFrom(src => src.AmountHT.ToString("N0") + " F");
                })
                .ForMember(dest => dest.strAmountTTC, opt => {
                  opt.MapFrom(src => src.AmountTTC.ToString("N0") + " F");
                });
            CreateMap<Order, OrderUserToValidateDto>()
                .ForMember(dest => dest.strOrderDate, opt => {
                  opt.MapFrom(src => src.OrderDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strDeadline, opt => {
                  opt.MapFrom(src => src.Deadline.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strDeadline, opt => {
                  opt.MapFrom(src => src.Deadline.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.NbDaysLate, opt => {
                  opt.MapFrom(src => (src.Validity - DateTime.Now).TotalDays + 1);
                })
                .ForMember(dest => dest.NbChildren, opt => {
                  opt.MapFrom(src => src.Lines.Count());
                })
                .ForMember(dest => dest.strValidity, opt => {
                  opt.MapFrom(src => src.Validity.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strAmountTTC, opt => {
                  opt.MapFrom(src => src.AmountTTC.ToString("N0") + " F");
                });
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.strOrderDate, opt => {
                  opt.MapFrom(src => src.OrderDate.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strDeadline, opt => {
                  opt.MapFrom(src => src.Deadline.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strDeadline, opt => {
                  opt.MapFrom(src => src.Deadline.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.NbDaysLate, opt => {
                  opt.MapFrom(src => (src.Validity - DateTime.Now).TotalDays + 1);
                })
                .ForMember(dest => dest.strValidity, opt => {
                  opt.MapFrom(src => src.Validity.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.strAmountHT, opt => {
                  opt.MapFrom(src => src.AmountHT.ToString("N0") + " F");
                })
                .ForMember(dest => dest.strAmountTTC, opt => {
                  opt.MapFrom(src => src.AmountTTC.ToString("N0") + " F");
                });
            CreateMap<User, ParentUserDto>()
                .ForMember(dest => dest.Cell, opt => {
                  opt.MapFrom(src => src.PhoneNumber);
                });
            CreateMap<District, DistrictDto>()
                .ForMember(dest => dest.CityName, opt => {
                  opt.MapFrom(src => src.City.Name);
                });
        }
    }
}