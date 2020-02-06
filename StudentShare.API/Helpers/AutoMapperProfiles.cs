using System.Linq;
using AutoMapper;
using StudentShare.API.Dtos;
using StudentShare.API.Models;

namespace StudentShare.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        // creating the maps for automapper
        public AutoMapperProfiles()
        {  
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => // for each member, define the photourl property
                opt.MapFrom(src => src.Photo.FirstOrDefault(p => p.MainPhoto).Url)); // where the info comes from
            CreateMap<User, UserForDetailsDto>()
                .ForMember(dest => dest.PhotoUrl, opt => 
                opt.MapFrom(src => src.Photo.FirstOrDefault(p => p.MainPhoto).Url)); 
            CreateMap<Photo, PhotosForDetailsDto>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap(); // reverse map allows the message to go both ways
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(m => m.SenderPhotoUrl, opt => opt
                    .MapFrom(u => u.Sender.Photo.FirstOrDefault(p => p.MainPhoto).Url)) // populating the photos to feed back as well as the messages for sender
                .ForMember(m => m.RecipientPhotoUrl, opt => opt
                    .MapFrom(u => u.Recipient.Photo.FirstOrDefault(p => p.MainPhoto).Url)); // populating the photos to feed back as well as the messages for recipient
        }
    }
}