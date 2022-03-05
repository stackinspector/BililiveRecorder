using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BililiveRecorder.Core.Api.Model;
using BililiveRecorder.Core.Config.V2;
using Newtonsoft.Json;

namespace BililiveRecorder.Core.Api.Http
{
    public class HttpApiClient : IApiClient, IDanmakuServerApiClient
    {
        private const string HttpHeaderAccept = "application/json, text/javascript, */*; q=0.01";
        private const string HttpHeaderOrigin = "https://live.bilibili.com";
        private const string HttpHeaderReferer = "https://live.bilibili.com/";
        private const string HttpHeaderUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36";
        private static readonly TimeSpan TimeOutTimeSpan = TimeSpan.FromSeconds(10);

        private readonly GlobalConfig config;
        private readonly HttpClient anonClient;
        private HttpClient mainClient;
        private bool disposedValue;

        public HttpClient MainHttpClient => mainClient;

        public HttpApiClient(GlobalConfig config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            config.PropertyChanged += this.Config_PropertyChanged;

            this.mainClient = null!;
            this.SetCookie();

            this.anonClient = new HttpClient
            {
                Timeout = TimeOutTimeSpan
            };
            var headers = this.anonClient.DefaultRequestHeaders;
            headers.Add("Accept", HttpHeaderAccept);
            headers.Add("Origin", HttpHeaderOrigin);
            headers.Add("Referer", HttpHeaderReferer);
            headers.Add("User-Agent", HttpHeaderUserAgent);
        }

        private void SetCookie()
        {
            var client = new HttpClient(new HttpClientHandler
            {
                UseCookies = false,
                UseDefaultCredentials = false,
            })
            {
                Timeout = TimeOutTimeSpan
            };
            var headers = client.DefaultRequestHeaders;
            headers.Add("Accept", HttpHeaderAccept);
            headers.Add("Origin", HttpHeaderOrigin);
            headers.Add("Referer", HttpHeaderReferer);
            headers.Add("User-Agent", HttpHeaderUserAgent);

            var cookie_string = this.config.Cookie;
            if (!string.IsNullOrWhiteSpace(cookie_string))
                headers.Add("Cookie", cookie_string);

            var old = Interlocked.Exchange(ref this.mainClient, client);
            old?.Dispose();
        }

        private void Config_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.config.Cookie))
                this.SetCookie();
        }

        private static async Task<T> FetchAsync<T>(HttpClient client, string url) where T : class
        {
            var resp = await client.GetAsync(url).ConfigureAwait(false);

            if (resp.StatusCode == (HttpStatusCode)412)
                throw new BilibiliApiRateLimitedException("Found rate limited from HTTP status code 412 when requesting " + url);

            resp.EnsureSuccessStatusCode();

            var text = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

            var obj = JsonConvert.DeserializeObject<BilibiliApiResponse<T>>(text);
            var code = obj?.Code;
            if (code == 0)
            {
                // code为0，则data一定不为null
                var data = obj?.Data;
                if (data == null)
                    throw new Exception("data=null when code=0");
                return data;
            }
            else if (code != -412)
                throw new BilibiliApiRateLimitedException("Found rate limited from response code -412 when requesting " + url);
            else
                throw new BilibiliApiCommonException("Response code " + code?.ToString() ?? "(null)" + ", full response:\n" + text);
        }

        public Task<RoomInfo> GetRoomInfoAsync(int roomid)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(HttpApiClient));

            var url = $@"{this.config.LiveApiHost}/room/v1/Room/get_info?id={roomid}";
            return FetchAsync<RoomInfo>(this.mainClient, url);
        }

        public Task<UserInfo> GetUserInfoAsync(int roomid)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(HttpApiClient));

            var url = $@"{this.config.LiveApiHost}/live_user/v1/UserInfo/get_anchor_in_room?roomid={roomid}";
            return FetchAsync<UserInfo>(this.mainClient, url);
        }

        public Task<ExtRoomInfo> GetExtRoomInfoAsync(int roomid)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(HttpApiClient));

            var url = $@"{this.config.LiveApiHost}/xlive/web-room/v1/index/getH5InfoByRoom?room_id={roomid}";
            return FetchAsync<ExtRoomInfo>(this.mainClient, url);
        }

        public Task<RoomPlayInfo> GetStreamUrlAsync(int roomid, int qn)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(HttpApiClient));

            var url = $@"{this.config.LiveApiHost}/xlive/web-room/v2/index/getRoomPlayInfo?room_id={roomid}&protocol=0,1&format=0,1,2&codec=0,1&qn={qn}&platform=web&ptype=8";
            return FetchAsync<RoomPlayInfo>(this.mainClient, url);
        }

        public Task<DanmuInfo> GetDanmakuServerAsync(int roomid)
        {
            if (this.disposedValue)
                throw new ObjectDisposedException(nameof(HttpApiClient));

            var url = $@"{this.config.LiveApiHost}/xlive/web-room/v1/index/getDanmuInfo?id={roomid}&type=0";
            return FetchAsync<DanmuInfo>(this.anonClient, url);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    this.config.PropertyChanged -= this.Config_PropertyChanged;
                    this.mainClient.Dispose();
                    this.anonClient.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                this.disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~HttpApiClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
