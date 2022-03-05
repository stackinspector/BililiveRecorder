using System;
using System.Threading.Tasks;
using BililiveRecorder.Core.Api.Model;

namespace BililiveRecorder.Core.Api
{
    public interface IApiClient : IDisposable
    {
        Task<RoomInfo> GetRoomInfoAsync(int roomid);
        Task<UserInfo> GetUserInfoAsync(int roomid);
        Task<ExtRoomInfo> GetExtRoomInfoAsync(int roomid);
        Task<RoomPlayInfo> GetStreamUrlAsync(int roomid, int qn);
    }
}
