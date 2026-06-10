using AutoMapper;
using PollApp.Backend.Models;

namespace PollApp.Backend.DTOs
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Poll, PollDto>();
            CreateMap<PollOption, PollOptionDto>()
                .ForMember(dest => dest.VotesCount, opt => opt.MapFrom(src => src.Votes.Count));
            CreateMap<Comment, CommentDto>();
        }
    }
}
