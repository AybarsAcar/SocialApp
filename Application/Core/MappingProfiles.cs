using System.Linq;
using Application.Activities;
using AutoMapper;
using Domain;

namespace Application.Core
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      // create maps from one object to another object
      CreateMap<Activity, Activity>();

      // we need to configure automapper to send related data when the names are not matching
      CreateMap<Activity, ActivityDto>()
        .ForMember(d => d.HostUsername,
          o => o.MapFrom(
            s => s.Attendees.FirstOrDefault(
              x => x.IsHost).AppUser.UserName));

      // fill the users in the attendees
      CreateMap<ActivityAttendee, Profiles.Profile>()
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
        .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
        .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio));
    }
  }
}