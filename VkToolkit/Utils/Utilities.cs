﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using VkToolkit.Enums;
using VkToolkit.Model;

namespace VkToolkit.Utils
{
    internal static class Utilities
    {
        public static Lyrics GetLyrics(JObject lyrics)
        {
            var output = new Lyrics
                             {
                                 Id = (long) lyrics["lyrics_id"],
                                 Text = (string) lyrics["text"]
                             };
            return output;
        }
        public static Audio GetAudioFromJObject(JObject audio)
        {
            // todo case when album id is not null
            var output = new Audio
                             {
                                 Id = (long)audio["aid"],
                                 OwnerId = (long)audio["owner_id"],
                                 Duration = (int)audio["duration"],
                                 Artist = (string)audio["artist"],
                                 Title = (string)audio["title"],
                                 Url = new Uri((string)audio["url"]),
                                 Performer = (string) audio["performer"]
                             };

            if (audio["lyrics_id"] != null)
                output.LyricsId = Convert.ToInt64((string) audio["lyrics_id"]);

            if (audio["album"] != null)
                output.AlbumId = Convert.ToInt64((string) audio["album"]);
            
            return output;
        }

        public static User GetProfileFromJObject(JObject current)
        {
            var profile = new User();

            profile.FirstName = (string) current["first_name"];
            profile.LastName = (string) current["last_name"];
            profile.Nickname = (string) current["nickname"];
            profile.ScreenName = (string) current["screen_name"];
            profile.Sex = (int?) current["sex"];
            profile.BirthDate = (string) current["bdate"];
            profile.City = (string) current["city"];
            profile.Country = (string) current["country"];
            profile.Photo = (string) current["photo"];
            profile.PhotoMedium = (string) current["photo_medium"];
            profile.PhotoBig = (string) current["photo_big"];
            profile.HasMobile = (int?) current["has_mobile"];
            profile.Rate = (string) current["rate"];
            profile.MobilePhone = (string) current["mobile_phone"];
            profile.HomePhone = (string) current["home_phone"];
            profile.Online = (int?) current["online"];
            profile.NameGen = (string) current["name_gen"];

            if (current["timezone"] != null)
            {
                double timezone;
                profile.Timezone = double.TryParse(current["timezone"].ToString(), out timezone) ? timezone : 0;
            }

            if (current["uid"] != null)
                profile.Id = (long) current["uid"];
            else if (current["id"] != null)
            {
                long id;
                profile.Id = long.TryParse((string)current["id"], out id) ? id : 0;
            }

            if (current["name"] != null)
            {
                // split for name and surname
                string[] parts = ((string) current["name"]).Split(' ');
                profile.FirstName = parts[0];
                profile.LastName = parts[1];
            }

            if (current["university"] != null)
            {
                profile.Education = new Education
                {
                    UniversityId = (string)current["university"],
                    UniversityName = (string)current["university_name"],
                    FacultyId = (string)current["faculty"],
                    FacultyName = (string)current["faculty_name"],
                    Graduation = (string)current["graduation"]
                };
            }

            if (current["counters"] != null)
            {
                var counters = (JObject)current["counters"];

                profile.Counters = new Counters
                {
                    Albums = (int)counters["albums"],
                    Videos = (int)counters["videos"],
                    Audios = (int)counters["audios"],
                    Notes = (int)counters["notes"],
                    Photos = (int)counters["photos"],
                    Groups = (int)counters["groups"],
                    Friends = (int)counters["friends"],
                    OnlineFriends = (int)counters["online_friends"],
                    UserPhotos = (int)counters["user_photos"],
                    UserVideos = (int)counters["user_videos"],
                    Followers = (int)counters["followers"],
                    Subscriptions = (int)counters["subscriptions"]
                };
            }

            return profile;
        }

        public static Group GetGroupFromJObject(JObject group)
        {
            var output = new Group
            {
                Id = (long)group["gid"],
                Name = (string)group["name"],
                Link = (string)group["link"],
                Photo = (string)group["photo"],
                PhotoMedium = (string)group["photo_medium"],
                PhotoBig = (string)group["photo_big"],
                ScreenName = (string)group["screen_name"],
                Description = (string)group["description"],
                WikiPage = (string)group["wiki_page"]
            };

            var isClosed = (int?)group["is_closed"];
            var isAdmin = (int?)group["is_admin"];
            var isMember = (int?) group["is_member"];

            output.CityId = (long?)group["city"];
            output.CountryId = (long?)group["country"];

            long datetime;
            if (long.TryParse((string)group["start_date"], out datetime) && datetime > 0)
                output.StartDate = UnixTimeStampToDateTime(datetime);

            if (isClosed.HasValue)
            {
                output.IsClosed = isClosed.Value == 1;
            }

            if (isMember.HasValue)
            {
                output.IsMember = isMember.Value == 1;
            }
            //else
            //    output.IsMember = false;
            
            if (isAdmin.HasValue)
            {
                output.IsAdmin = isAdmin.Value == 1;
            }
            else
                output.IsAdmin = false;

            output.Type = GetGroupType((string)group["type"]);

            return output;
        }

        public static GroupType GetGroupType(string type)
        {
            switch (type)
            {
                case "page":
                    return GroupType.Page;

                case "event":
                    return GroupType.Event;

                case "group":
                    return GroupType.Group;

                default:
                    return GroupType.Undefined;
            }
        }

        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dt = dt.AddSeconds(unixTimeStamp).ToLocalTime();
            return dt;
        }

        public static FriendStatus GetFriendStatus(int status)
        {
            switch (status)
            {
                case 0:
                    return FriendStatus.NotFriend;

                case 1:
                    return FriendStatus.OutputRequest;

                case 2:
                    return FriendStatus.InputRequest;

                case 3:
                    return FriendStatus.Friend;
                    
                default:
                    throw new ArgumentException("friend_status not defined!", "status");
            }
        }

        public static string GetEnumerationAsString<T>(IEnumerable<T> uids)
        {
            var uidsLst = uids.ToList();
            string ids = "";
            for (int i = 0; i < uidsLst.Count; i++)
            {
                ids += uidsLst[i];
                if (i != uidsLst.Count - 1)
                    ids += ",";
            }

            return ids;
        }
    }
}