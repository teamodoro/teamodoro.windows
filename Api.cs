using System;
using System.Drawing;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Timers;
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
                            _current.State.Name = State.longBreak.ToString();
                            if (StateChanged != null) StateChanged(State.longBreak);
                        }
                        else
                        {
                            _current.State.Name = State.shortBreak.ToString();
                            if (StateChanged != null) StateChanged(State.shortBreak);
                        }
                    }
                    break;

                case State.shortBreak:
                    if (_current.CurrentTime == _current.Options.ShortBreak.Duration) 
                    {
                        _current.CurrentTime = 0;
                        _current.State.Name = State.running.ToString();
                        if (StateChanged != null) StateChanged(State.running);
                    }
                    break;

                case State.longBreak:
                    if (_current.CurrentTime == _current.Options.LongBreak.Duration)
                    {
                        _current.CurrentTime = 0;
                        _current.State.Name = State.running.ToString();
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

        private CurrentObject GetCurrent(String url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                if (request == null) return null;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response == null || response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).",
                            response == null ? (object) -1 : response.StatusCode,
                            response == null ? "Response is null" : response.StatusDescription));

                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof (CurrentObject));
                    object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    CurrentObject jsonResponse = objResponse as CurrentObject;
                    return jsonResponse;
                }
            }
            catch
            {
                // TODO: Error!!!
                return null;
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

    }
}
