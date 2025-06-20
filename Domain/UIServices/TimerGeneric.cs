
using Timer = System.Timers.Timer; // Alias explícito para evitar ambigüedad

namespace SuSuerteV2.Domain.UIServices
{
    public class TimerGeneric : IDisposable
    {
        public event Action<string>? Tick;
        public event Action? TimeOut;
        public event Action? Stopped;

        private readonly Timer _timer; // Ahora usa el alias definido
        private bool _disposed = false;
        private bool _isRunning = false;

        public int Minutes { get; private set; }
        public int Seconds { get; private set; }

        public bool IsRunning => _isRunning;

        public TimerGeneric(TimeSpan initialTime)
        {
            if (initialTime.TotalSeconds <= 0)
                throw new ArgumentException("El tiempo inicial debe ser mayor a cero", nameof(initialTime));

            Minutes = initialTime.Minutes + (initialTime.Hours * 60);
            Seconds = initialTime.Seconds;

            _timer = new Timer(1000); // Intervalo de 1 segundo
            _timer.Elapsed += OnTimerTick;
            _timer.AutoReset = true;
        }

        public TimerGeneric(string timeString) : this(ParseTimeString(timeString))
        {
        }

        private static TimeSpan ParseTimeString(string timeString)
        {
            if (string.IsNullOrWhiteSpace(timeString))
                throw new ArgumentNullException(nameof(timeString));

            var parts = timeString.Split(':');
            if (parts.Length != 2)
                throw new FormatException("El formato del tiempo debe ser MM:SS");

            if (!int.TryParse(parts[0], out int minutes) || !int.TryParse(parts[1], out int seconds))
                throw new FormatException("Los valores deben ser numéricos");

            if (minutes < 0 || seconds < 0 || seconds >= 60)
                throw new ArgumentOutOfRangeException(nameof(timeString), "Los valores deben ser positivos y los segundos < 60");

            return new TimeSpan(0, minutes, seconds);
        }

        public void Start()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TimerGeneric));

            if (!_isRunning)
            {
                _isRunning = true;
                _timer.Start();
                InvokeTick();
            }
        }

        public void Stop()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TimerGeneric));

            if (_isRunning)
            {
                _isRunning = false;
                _timer.Stop();
                Stopped?.Invoke();
            }
        }

        public void Reset(TimeSpan newTime)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(TimerGeneric));

            bool wasRunning = _isRunning;
            Stop();

            Minutes = newTime.Minutes + (newTime.Hours * 60);
            Seconds = newTime.Seconds;

            if (wasRunning)
                Start();
        }

        private void OnTimerTick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (Seconds > 0)
                {
                    Seconds--;
                }
                else if (Minutes > 0)
                {
                    Minutes--;
                    Seconds = 59;
                }
                else
                {
                    Stop();
                    TimeOut?.Invoke();
                    return;
                }

                InvokeTick();
            }
            catch (Exception ex)
            {
                Stop();
                EventLogger.SaveLog(EventType.Error, $"Error en el temporizador: {ex.Message}", ex);
            }
        }

        private void InvokeTick()
        {
            string timeString = $"{Minutes:D2}:{Seconds:D2}";
            Tick?.Invoke(timeString);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timer.Elapsed -= OnTimerTick;
                    _timer.Dispose();
                }
                _disposed = true;
            }
        }

        ~TimerGeneric()
        {
            Dispose(false);
        }
    }
}