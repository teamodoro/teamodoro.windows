using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Timers;
using System.Windows.Media.Imaging;
using TeamodoroClient.Json;

namespace TeamodoroClient
{
    class Api
    {
        public delegate void TimerTickEvent();

        public delegate void StateChangedEvent(State state);

        public TimerTickEvent TimerTick;

        public StateChangedEvent StateChanged;

        private static readonly Api Instance = new Api();

        private Api() {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_current == null) return;

            _current.CurrentTime++;
            switch (GetState())
            {
                case State.running:
                    if (_current.CurrentTime == _current.Options.Running.Duration)
                    {
                        _current.CurrentTime = 0;
                        _current.TimesBeforeLongBreak--;
                        if (_current.TimesBeforeLongBreak == 0)
                        {
                            _current.TimesBeforeLongBreak = _current.Options.LongBreakEvery;
                            _current.State.Name = State.longBreak.GetDescription();
                            if (StateChanged != null) StateChanged(State.longBreak);
                        }
                        else
                        {
                            _current.State.Name = State.shortBreak.GetDescription();
                            if (StateChanged != null) StateChanged(State.shortBreak);
                        }
                    }
                    break;

                case State.shortBreak:
                    if (_current.CurrentTime == _current.Options.ShortBreak.Duration) 
                    {
                        _current.CurrentTime = 0;
                        _current.State.Name = State.running.GetDescription();
                        if (StateChanged != null) StateChanged(State.running);
                    }
                    break;

                case State.longBreak:
                    if (_current.CurrentTime == _current.Options.LongBreak.Duration)
                    {
                        _current.CurrentTime = 0;
                        _current.State.Name = State.running.GetDescription();
                        if (StateChanged != null) StateChanged(State.running);
                    }
                    break;
            }
            if (TimerTick != null) TimerTick();
        }

        public static Api GetInstance() {
            return Instance;
        }

        private CurrentObject _current;

        private readonly Timer _timer;

        private readonly CookieContainer _cookieContainer = new CookieContainer();

        private CurrentObject GetCurrent(String url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null)
                {
                    if (_current == null) return CreateDefaultCurrentObject();
                    _current.Connection = "No connection";
                    return _current;
                }

                request.CookieContainer = _cookieContainer;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                            response == null ? (object) -1 : response.StatusCode,
                            response == null ? "Response is null" : response.StatusDescription));

                    CookieCollection cookies = response.Cookies;
                    _cookieContainer.Add(cookies);
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof (CurrentObject));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    CurrentObject jsonResponse = objResponse as CurrentObject;
                    if (jsonResponse != null) jsonResponse.Connection = "Online: " + request.Host;
                    return jsonResponse;
                }
            }
            catch
            {
                return _current ?? CreateDefaultCurrentObject();
            }
        }

        public void UpdateState()
        {
            _timer.Stop();
            State savedState = GetState();
            _current = GetCurrent("http://teamodoro.sdfgh153.ru/api/current");
            _timer.Start();
            if (GetState() != savedState && StateChanged != null) StateChanged(GetState());
        }

        public State GetState()
        {
            try
            {
                return (State)Enum.Parse(typeof(State), _current.State.Name);
            }
            catch
            {
                return State.paused;
            }
        }

        public long GetCurrentTime()
        {
            return _current != null ? _current.CurrentTime : 0;
        }

        public long GetRemainingTime()
        {
            if (_current != null)
            {
                switch (GetState())
                {
                    case State.running: return _current.Options.Running.Duration - _current.CurrentTime;
                    case State.shortBreak: return _current.Options.ShortBreak.Duration - _current.CurrentTime;
                    case State.longBreak: return _current.Options.LongBreak.Duration - _current.CurrentTime;
                    default: return 0;
                }
            }
            return 0;
        }

        private String FormatTime(long t)
        {
            long m = t / 60;
            long s = t % 60;
            return m + ":" + (s < 10 ? "0" : "") + s;
        }

        public String GetCurrentTimeString()
        {
            return FormatTime(GetCurrentTime());
        }

        public String GetRemainingTimeString()
        {
            return FormatTime(GetRemainingTime());
        }

        public Color GetColor()
        {
            if (_current != null)
            {
                switch (GetState())
                {
                    case State.running: return Color.FromName(_current.Options.Running.Color);
                    case State.shortBreak: return Color.FromName(_current.Options.ShortBreak.Color);
                    case State.longBreak: return Color.FromName(_current.Options.LongBreak.Color);
                    default: return Color.Black;
                }
            }
            return Color.Black;
        }

        public System.Windows.Media.Color GetMediaColor()
        {
            Color color = GetColor();
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public String GetConnectionName()
        {
            String res = _current.Connection;
            return res;
        }

        private BitmapImage GetImageByUrl(String url)
        {
            try
            {
                var image = new BitmapImage();
                WebRequest request = WebRequest.Create(new Uri(url, UriKind.Absolute));
                BinaryReader reader = new BinaryReader(request.GetResponse().GetResponseStream());
                MemoryStream memoryStream = new MemoryStream();

                byte[] buffer = new byte[1024];
                int readed = reader.Read(buffer, 0, 1024);

                do
                {
                    memoryStream.Write(buffer, 0, readed);
                    readed = reader.Read(buffer, 0, 1024);
                } while (readed > 0);

                image.BeginInit();
                memoryStream.Seek(0, SeekOrigin.Begin);
                image.StreamSource = memoryStream;
                image.EndInit();

                return image;
            }
            catch
            {
                return null;
            }
        }

        public BitmapImage GetBackgroundImage()
        {
            return GetImageByUrl("http://teamodoro.sdfgh153.ru/img/bg3.jpg");
        }

        private CurrentObject CreateDefaultCurrentObject()
        {
            return new CurrentObject
            {
                Name = "offline",
                Options = new Options
                {
                    Running = new StateOption {Color = "white", Duration = 1500},
                    ShortBreak = new StateOption {Color = "green", Duration = 300},
                    LongBreak = new StateOption {Color = "yellow", Duration = 900},
                    LongBreakEvery = 4,
                    AliveTimeout = 60
                },
                State = new CurrentState {Name = State.running.GetDescription()},
                CurrentTime = 0,
                TimesBeforeLongBreak = 4,
                Connection = "No connection"
            };
        }

    }
}
