using System;
using System.Threading.Tasks;
using BililiveRecorder.Core.Api.Model;
using Polly;
using Polly.Registry;

namespace BililiveRecorder.Core.Api
{
    public class PolicyWrappedApiClient<T> : IApiClient, IDanmakuServerApiClient, IDisposable where T : class, IApiClient, IDanmakuServerApiClient, IDisposable
    {
        private readonly T client;
        private readonly IReadOnlyPolicyRegistry<string> policies;

        public PolicyWrappedApiClient(T client, IReadOnlyPolicyRegistry<string> policies)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.policies = policies ?? throw new ArgumentNullException(nameof(policies));
        }

        public async Task<DanmuInfo> GetDanmakuServerAsync(int roomid) => await this.policies
            .Get<IAsyncPolicy>(PolicyNames.PolicyDanmakuApiRequestAsync)
            .ExecuteAsync(_ => this.client.GetDanmakuServerAsync(roomid), new Context(PolicyNames.CacheKeyDanmaku + ":" + roomid))
            .ConfigureAwait(false);

        public async Task<RoomInfo> GetRoomInfoAsync(int roomid) => await this.policies
            .Get<IAsyncPolicy>(PolicyNames.PolicyRoomInfoApiRequestAsync)
            .ExecuteAsync(_ => this.client.GetRoomInfoAsync(roomid), new Context(PolicyNames.CacheKeyRoomInfo + ":" + roomid))
            .ConfigureAwait(false);

        public async Task<ExtRoomInfo> GetExtRoomInfoAsync(int roomid) => await this.policies
            .Get<IAsyncPolicy>(PolicyNames.PolicyRoomInfoApiRequestAsync)
            .ExecuteAsync(_ => this.client.GetExtRoomInfoAsync(roomid), new Context(PolicyNames.CacheKeyExtRoomInfo + ":" + roomid))
            .ConfigureAwait(false);

        public async Task<RoomPlayInfo> GetStreamUrlAsync(int roomid, int qn) => await this.policies
            .Get<IAsyncPolicy>(PolicyNames.PolicyStreamApiRequestAsync)
            .ExecuteAsync(_ => this.client.GetStreamUrlAsync(roomid, qn), new Context(PolicyNames.CacheKeyStream + ":" + roomid + ":" + qn))
            .ConfigureAwait(false);

        public async Task<UserInfo> GetUserInfoAsync(int roomid) => await this.policies
            .Get<IAsyncPolicy>(PolicyNames.PolicyRoomInfoApiRequestAsync)
            .ExecuteAsync(_ => this.client.GetUserInfoAsync(roomid), new Context(PolicyNames.CacheKeyUserInfo + ":" + roomid))
            .ConfigureAwait(false);

        public void Dispose() => this.client.Dispose();
    }
}
