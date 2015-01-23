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

        private static Api _instance = new Api();

        private Api() {
            _timer = new Timer(1000);
            _timer.Elapsed += TimerElapsed;
            _timer.Start();
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            _current.CurrentTime++;
            switch (getState())
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

        public static Api getInstance() {
            return _instance;
        }

        private CurrentObject _current;

        private Timer _timer;

        private CurrentObject getCurrent(String url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(CurrentObject));
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

        public void updateState()
        {
            _timer.Stop();
            State savedState = getState();
            _current = getCurrent("http://teamodoro.sdfgh153.ru/api/current");
            _timer.Start();
            if (getState() != savedState && StateChanged != null) StateChanged(getState());
        }

        public State getState()
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

        public long getCurrentTime()
        {
            return _current != null ? _current.CurrentTime : 0;
        }

        public long getRemainingTime()
        {
            if (_current != null)
            {
                switch (getState())
                {
                    case State.running: return _current.Options.Running.Duration - _current.CurrentTime;
                    case State.shortBreak: return _current.Options.ShortBreak.Duration - _current.CurrentTime;
                    case State.longBreak: return _current.Options.LongBreak.Duration - _current.CurrentTime;
                    default: return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private String formatTime(long t)
        {
            long m = t / 60;
            long s = t % 60;
            return m.ToString() + ":" + (s < 10 ? "0" : "") + s.ToString();
        }

        public String getCurrentTimeString()
        {
            return formatTime(getCurrentTime());
        }

        public String getRemainingTimeString()
        {
            return formatTime(getRemainingTime());
        }

        public Color getColor()
        {
            if (_current != null)
            {
                switch (getState())
                {
                    case State.running: return Color.FromName(_current.Options.Running.Color);
                    case State.shortBreak: return Color.FromName(_current.Options.ShortBreak.Color);
                    case State.longBreak: return Color.FromName(_current.Options.LongBreak.Color);
                    default: return Color.Black;
                }
            }
            else
            {
                return Color.Black;
            }
        }

        public System.Windows.Media.Color getMediaColor()
        {
            Color color = getColor();
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

    }
}
