/*----------------------------------------------------------------------------------
// Copyright 2019 Huawei Technologies Co.,Ltd.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License.  You may obtain a copy of the
// License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
// CONDITIONS OF ANY KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations under the License.
//----------------------------------------------------------------------------------*/
using System;
using System.Threading;

namespace OBS.Internal
{
    internal class ObsAsyncResult<V> : IAsyncResult, IDisposable
    {
        protected readonly object _state;

        protected readonly ManualResetEvent _event;

        protected AsyncCallback _callback;

        protected Exception _exception;

        protected bool _isCompleted = false;

        protected bool _disposed = false;

        protected V _result;

        public ObsAsyncResult(AsyncCallback callback, object state)
        {
            _callback = callback;
            _state = state;
            _event = new ManualResetEvent(false);
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_isCompleted)
                {
                    _event.Set();
                }
                return _event;
            }
        }

        public AsyncCallback AsyncCallback
        {
            get { return _callback; }
        }

        public object AsyncState
        {
            get { return _state; }
        }

        public bool CompletedSynchronously
        {
            get;
            set;
        }

        public V Get()
        {
            if (!_isCompleted)
            {
                _event.WaitOne();
            }

            if (_exception != null)
            {
                throw _exception;
            }
            return _result;
        }

        public virtual V Get(int millisecondsTimeout)
        {
            if (!_isCompleted)
            {
                if (!_event.WaitOne(millisecondsTimeout))
                {
                    throw new TimeoutException();
                }
            }

            if (_exception != null)
            {
                throw _exception;
            }
            return _result;
        }

        public virtual void Reset(AsyncCallback callback)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("IAsyncResult is disposed");
            }
            _isCompleted = false;
            _event.Reset();
            _result = default(V);
            _exception = null;
            if(callback != null)
            {
                _callback = callback;
            }
        }

        private void Notify()
        {
            _isCompleted = true;
            if (!_disposed)
            {
                _event.Set();
            }
            _callback?.Invoke(this);
        }

        public void Set(V result)
        {
            _result = result;
            Notify();
        }

        public void Set(Exception ex)
        {
            _exception = ex;
            Notify();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing && _event != null)
                {
                    _event.Close();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
