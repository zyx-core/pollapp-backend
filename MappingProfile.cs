using AutoMapper;
using PollApp.Backend.Models;
using PollApp.Backend.DTOs;

namespace PollApp.Backend
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Poll, PollDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator));
            CreateMap<PollOption, PollOptionDto>()
                .ForMember(dest => dest.VotesCount, opt => opt.MapFrom(src => src.Votes.Count));
            CreateMap<User, UserDto>();
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
        }
    }
}
