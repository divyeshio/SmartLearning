using AutoMapper;
using SmartLearning.Core.Entities.Common;
using SmartLearning.Web.DTO;

namespace SmartLearning.Web.Mappings
{
  public class MessageProfile : Profile
  {
    public MessageProfile()
    {
      CreateMap<Message, MessageViewModel>()
          .ForMember(dst => dst.From, opt => opt.MapFrom(x => x.FromUser.Email))
          .ForMember(dst => dst.FromName, opt => opt.MapFrom(x => x.FromUser.FirstName))
          .ForMember(dst => dst.ToClass, opt => opt.MapFrom(x => x.ToClassId))
          .ForMember(dst => dst.Avatar, opt => opt.MapFrom(x => x.FromUser.Avatar))
          .ForMember(dst => dst.Content, opt => opt.MapFrom(x => x.Content))
          .ForMember(dst => dst.Timestamp, opt => opt.MapFrom(x => x.Timestamp));
      CreateMap<MessageViewModel, Message>();
    }
  }
}
