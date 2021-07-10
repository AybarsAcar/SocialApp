using System.Linq;
using Application.Activities;
using Application.Comments;
using AutoMapper;
using Domain;

namespace Application.Core
{
  public class MappingProfiles : Profile
  {
    public MappingProfiles()
    {
      // passed form .ProjectTo configuration in List method
      string currentUsername = null;

      // create maps from one object to another object
      CreateMap<Activity, Activity>();

      // we need to configure automapper to send related data when the names are not matching
      CreateMap<Activity, ActivityDto>()
        .ForMember(d => d.HostUsername,
          o => o.MapFrom(
            s => s.Attendees.FirstOrDefault(
              x => x.IsHost).AppUser.UserName));

      // fill the users in the attendees
      CreateMap<ActivityAttendee, AttendeeDto>()
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
        .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
        .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio))
        .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
        .ForMember(d => d.FollowerCount, o => o.MapFrom(s => s.AppUser.Followers.Count))
        .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.AppUser.Followings.Count))
        .ForMember(d => d.Following,
          o => o.MapFrom(s => s.AppUser.Followers.Any(x => x.Observer.UserName == currentUsername)));


      // map from AppUser to Profile
      // include the photos
      CreateMap<AppUser, Profiles.Profile>()
        .ForMember(d => d.Image, options => options.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsMain).Url))
        .ForMember(d => d.FollowerCount, o => o.MapFrom(s => s.Followers.Count))
        .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.Followings.Count))
        .ForMember(d => d.Following, o => o.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));

      // add the user info - the author of the comment
      CreateMap<Comment, CommentDto>()
        .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName))
        .ForMember(d => d.Username, o => o.MapFrom(s => s.Author.UserName))
        .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url));
    }
  }
}