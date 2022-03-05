using System;
using System.Threading.Tasks;
using BililiveRecorder.Core.Api.Model;

namespace BililiveRecorder.Core.Api
{
    public interface IApiClient : IDisposable
    {
        Task<BilibiliApiResponse<RoomInfo>> GetRoomInfoAsync(int roomid);
        Task<BilibiliApiResponse<UserInfo>> GetUserInfoAsync(int roomid);
        Task<BilibiliApiResponse<ExtRoomInfo>> GetExtRoomInfoAsync(int roomid);
        Task<BilibiliApiResponse<RoomPlayInfo>> GetStreamUrlAsync(int roomid, int qn);
    }
}
