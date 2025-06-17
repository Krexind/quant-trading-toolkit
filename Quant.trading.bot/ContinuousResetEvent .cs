using System;
using System.Threading;
using System.Threading.Tasks;

namespace QuantitativeTrading
{
    public class ContinuousResetEvent : IDisposable
    {
        private readonly int continuousTime;
        private readonly int unlockDelay;

        private AutoResetEvent autoResetEvent;
        private bool disposedValue;
        private DateTime unlockTime;

        public ContinuousResetEvent(int continuousTime, int unlockDelay)
            => (autoResetEvent, this.continuousTime, this.unlockDelay) = (new(false), continuousTime, unlockDelay);

        public void Set()
        {
            if (DateTime.Now > unlockTime)
            {
                autoResetEvent.Set();
                unlockTime = DateTime.Now.AddSeconds(continuousTime);
            }
        }

        public async Task WaitOneAsync()
        {
            autoResetEvent.WaitOne();
            await Task.Delay(unlockDelay);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置受控狀態 (受控物件)
                    if (autoResetEvent != null)
                    {
                        autoResetEvent.Dispose();
                        autoResetEvent = null;
                    }
                }

                // TODO: 釋出非受控資源 (非受控物件) 並覆寫完成項
                // TODO: 將大型欄位設為 Null
                disposedValue = true;
            }
        }

        // TODO: 僅有當 'Dispose(bool disposing)' 具有會釋出非受控資源的程式碼時，才覆寫完成項
        ~ContinuousResetEvent()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 請勿變更此程式碼。請將清除程式碼放入 'Dispose(bool disposing)' 方法
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
