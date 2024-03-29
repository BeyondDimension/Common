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
using OBS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OBS.Internal
{
    internal class TransferStream : Stream
    {
        internal delegate void BytesTransferred(int bytes);
        internal delegate void BytesAction(long bytes);
        internal delegate void EventDelegate();

        internal event BytesTransferred BytesReaded;
        internal event BytesTransferred BytesWrited;
        internal event BytesAction BytesReset;
        internal event EventDelegate StartWrite;
        internal event EventDelegate StartRead;
        internal event EventDelegate CloseStream;

        protected bool readFlag = false;
        protected bool writeFlag = false;
        protected long readedBytes = 0;

        internal Stream OriginStream { get; set; }

        public override bool CanRead
        {
            get
            {
                return OriginStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return OriginStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return OriginStream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return OriginStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return OriginStream.Position;
            }
            set
            {
                writeFlag = false;
                readFlag = false;
                OriginStream.Position = value;
            }
        }

        public TransferStream(Stream originStream)
        {
            OriginStream = originStream;
        }

        public override void Flush()
        {
            OriginStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            writeFlag = false;
            readFlag = false;
            return OriginStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            OriginStream.SetLength(value);
        }

        public void ResetReadProgress()
        {
            BytesReset?.Invoke(readedBytes);
            readedBytes = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!readFlag)
            {
                readFlag = true;
                StartRead?.Invoke();
            }
            int bytes = OriginStream.Read(buffer, offset, count);
            readedBytes += bytes;
            BytesReaded?.Invoke(bytes);
            return bytes;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!writeFlag)
            {
                writeFlag = true;
                StartWrite?.Invoke();
            }
            OriginStream.Write(buffer, offset, count);
            BytesWrited?.Invoke(count);
        }

        public override void Close()
        {
            try
            {
                OriginStream.Close();
            }
            finally
            {
                CloseStream?.Invoke();
            }
        }

    }

    internal class BytesUnit
    {
        public DateTime DateTime
        {
            get;
            set;
        }

        public long Bytes
        {
            set;
            get;
        }
    }

    internal abstract class TransferStreamManager
    {
        protected object sender;
        protected EventHandler<TransferStatus> handler;
        protected long totalBytes;
        protected long transferredBytes;
        protected long newlyTransferredBytes;
        protected DateTime startCheckpoint;
        protected DateTime lastCheckpoint;
        protected double interval;
        protected volatile IList<BytesUnit> lastInstantaneousBytes;

        public TransferStreamManager(object sender, EventHandler<TransferStatus> handler, long totalBytes,
            long transferredBytes)
        {
            this.sender = sender;
            this.handler = handler;
            this.totalBytes = totalBytes;
            this.transferredBytes = transferredBytes < 0 ? 0 : transferredBytes;
            startCheckpoint = DateTime.Now;
            lastCheckpoint = DateTime.Now;
        }

        public virtual void TransferStart()
        {
            startCheckpoint = DateTime.Now;
            lastCheckpoint = DateTime.Now;
            lastInstantaneousBytes = new List<BytesUnit>();
        }

        public virtual void TransferReset(long resetBytes)
        {
            startCheckpoint = DateTime.Now;
            lastCheckpoint = DateTime.Now;
            lastInstantaneousBytes = new List<BytesUnit>();
            newlyTransferredBytes = 0;
            transferredBytes -= resetBytes;
        }

        protected IList<BytesUnit> CreateCurrentInstantaneousBytes(long bytes, DateTime now)
        {
            IList<BytesUnit> currentInstantaneousBytes = new List<BytesUnit>();
            IList<BytesUnit> _lastInstantaneousBytes = lastInstantaneousBytes;
            if (_lastInstantaneousBytes != null)
            {
                foreach (BytesUnit item in _lastInstantaneousBytes)
                {
                    if ((now - item.DateTime).TotalMilliseconds < 1000)
                    {
                        currentInstantaneousBytes.Add(item);
                    }
                }
            }
            BytesUnit unit = new BytesUnit
            {
                DateTime = now,
                Bytes = bytes,
            };
            currentInstantaneousBytes.Add(unit);
            return currentInstantaneousBytes;
        }

        public virtual void TransferEnd()
        {
            if(handler == null)
            {
                return;
            }
            DateTime now = DateTime.Now;
            TransferStatus status = new TransferStatus(newlyTransferredBytes,
                          transferredBytes, totalBytes, (now - lastCheckpoint).TotalSeconds, (now - startCheckpoint).TotalSeconds);
            status.SetInstantaneousBytes(CreateCurrentInstantaneousBytes(newlyTransferredBytes, now));
            handler(sender, status);
        }

        public void BytesTransfered(int bytes)
        {
            if (handler == null)
            {
                return;
            }

            if (bytes > 0)
            {
                DoBytesTransfered(bytes);
            }
        }

        protected abstract void DoBytesTransfered(int bytes);

    }

    internal class TransferStreamByBytes : TransferStreamManager
    {
        public TransferStreamByBytes(object sender, EventHandler<TransferStatus> handler, long totalBytes,
            long transferredBytes, double intervalByBytes) : base(sender, handler, totalBytes, transferredBytes)
        {
            interval = intervalByBytes;
        }

        protected override void DoBytesTransfered(int bytes)
        {
            transferredBytes += bytes;
            newlyTransferredBytes += bytes;
            DateTime now = DateTime.Now;
            IList<BytesUnit> currentInstantaneousBytes = CreateCurrentInstantaneousBytes(bytes, now);
            lastInstantaneousBytes = currentInstantaneousBytes;
            if (newlyTransferredBytes >= interval || transferredBytes == totalBytes)
            {
                TransferStatus status = new TransferStatus(newlyTransferredBytes,
                   transferredBytes, totalBytes, (now - lastCheckpoint).TotalSeconds, (now - startCheckpoint).TotalSeconds);
                status.SetInstantaneousBytes(currentInstantaneousBytes);
                handler(sender, status);
                // Reset
                newlyTransferredBytes = 0;
                lastCheckpoint = now;
            }
        }
    }

}
