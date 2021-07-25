using System;
using System.Globalization;
using System.Linq;
using AutoMapper;
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
                .ForMember(dest => dest.PhotoUrl, opt =>
                {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt =>
                {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                })
                .ForMember(dest => dest.DateOfBirth, opt =>
                {
                    opt.MapFrom(d => d.DateOfBirth.ToString("dd/MM/yyyy", frC));
                })
                .ForMember(dest => dest.Step, opt =>
                {
                    opt.MapFrom(src =>
                    Convert.ToInt32(src.PreSelected) + Convert.ToInt32(src.Selected) + Convert.ToInt32(src.OnTraining)
                        + Convert.ToInt32(src.Trained) + Convert.ToInt32(src.Hired) + Convert.ToInt32(src.Reserved));
                });
            // CreateMap<RetrofitInventOp, RetrofitInventOpDto>()
            // .ForMember(dest => dest.OpDate, opt =>
            //         {
            //             opt.MapFrom(d => d.OpDate.ToString("dd/MM/yyyy HH:mm", frC));
            //         });

            // CreateMap<EnrolmentCenter, EcsForListDto>()
            // .ForMember(dest => dest.DisplayCode, opt =>
            // {
            //     opt.MapFrom(src => src.Municipality.City.Department.Region.Code + "-" + src.Municipality.City.Department.Code + "-"
            //                 + src.Municipality.City.Code + "-" + src.Municipality.Code + "-" + src.Code);
            // });

            // CreateMap<RetrofitStoreProduct, ProductDto>()
            //              .ForMember(dest => dest.Id, opt =>
            //              {
            //                  opt.MapFrom(src => src.Product.Id);
            //              })
            //              .ForMember(dest => dest.Name, opt =>
            //              {
            //                  opt.MapFrom(src => src.Product.Name);
            //              });
            // CreateMap<PhotoForCreationDto, Photo>();
            // CreateMap<UserForRegisterDto, User>();
            // CreateMap<NewStoreDto, RetrofitStore>();
            // CreateMap<TrainingCreationDto, Training>();
            // CreateMap<TrainingForUpdateDto, Training>();
            // CreateMap<TrainingClassCreationDto, TrainingClass>();
            // CreateMap<CityUpdateDto, City>();
            // CreateMap<TrainingClass, TrainingClassDetailDto>();
            // CreateMap<Photo, PhotoforDetailDto>();
            // .ForMember(dest => dest.StartDate, opt =>
            // {
            //     opt.MapFrom(src => src.StartDate?.ToString("dd/MM/yyyy", frC));
            // })
            // .ForMember(dest => dest.EndDate, opt =>
            // {
            //     opt.MapFrom(src => src.EndDate?.ToString("dd/MM/yyyy", frC));
            // });
            CreateMap<UserForUpdateDto, User>();
            // CreateMap<NewEcDto, EnrolmentCenter>();
            // CreateMap<MunicipalityForCreationDto, Municipality>();
            // CreateMap<Tablet, TabletDetailDto>();
            // CreateMap<FailureToSaveDto, Failure>();
            // CreateMap<EcDataToSaveDto, EcData>();
            // CreateMap<SdCardDto, Sdcard>();
            // CreateMap<SdCardTabletDto, SdcardTablet>();
            // CreateMap<ExportToCreateDto, Export>();
            // CreateMap<Failure, FailureListDto>()
            // .ForMember(dest => dest.FailureDateString, opt =>
            // {
            //     opt.MapFrom(src => src.FailureDate.ToString("dd/MM/yyyy", frC));
            // })
            //  .ForMember(dest => dest.MaintDateString, opt =>
            // {
            //     opt.MapFrom(src => Convert.ToDateTime(src.MaintDate).ToString("dd/MM/yyyy", frC));
            // });



        }
    }
}