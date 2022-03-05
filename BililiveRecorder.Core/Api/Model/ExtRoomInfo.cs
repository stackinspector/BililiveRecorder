using Newtonsoft.Json;

namespace BililiveRecorder.Core.Api.Model
{
    public class ExtRoomInfo
    {
        [JsonProperty("room_info")]
        public ExtRoomRoomInfo? RoomInfo { get; set; }

        [JsonProperty("anchor_info")]
        public ExtRoomUserInfo? UserInfo { get; set; }

        public class ExtRoomRoomInfo
        {
            [JsonProperty("room_id")]
            public int RoomId { get; set; }

            [JsonProperty("live_status")]
            public int LiveStatus { get; set; }

            [JsonProperty("area_id")]
            public int AreaId { get; set; }

            [JsonProperty("parent_area_id")]
            public int ParentAreaId { get; set; }

            [JsonProperty("area_name")]
            public string AreaName { get; set; } = string.Empty;

            [JsonProperty("parent_area_name")]
            public string ParentAreaName { get; set; } = string.Empty;

            [JsonProperty("title")]
            public string Title { get; set; } = string.Empty;
        }

        public class ExtRoomUserInfo
        {
            [JsonProperty("base_info")]
            public ExtRoomUserBaseInfo? BaseInfo { get; set; }
        }

        public class ExtRoomUserBaseInfo
        {
            [JsonProperty("uname")]
            public string Name { get; set; } = string.Empty;
        }
    }
}
