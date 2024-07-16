using System;
using System.Diagnostics;
using System.IO;
using Microsoft.IO;
using System.Collections.Generic;
using Lidgren.Network;

namespace OmegaRace
{
    public class MessageHandler
    {
        static int MEM_STREAM_BLOCK_SIZE = 128;
        static int MEM_STREAM_LARGE_BUFFER_MULTIPLE = MEM_STREAM_BLOCK_SIZE * MEM_STREAM_BLOCK_SIZE;
        static int MEM_STREAM_MAX_BUFFER_SIZE = 16 * MEM_STREAM_LARGE_BUFFER_MULTIPLE;

        protected byte[] _Data;
        protected RecyclableMemoryStreamManager _StreamManager;

        public byte[] Data { get => _Data; }

        public MessageHandler()
        {
            _StreamManager = new RecyclableMemoryStreamManager(MEM_STREAM_BLOCK_SIZE, MEM_STREAM_LARGE_BUFFER_MULTIPLE, MEM_STREAM_MAX_BUFFER_SIZE);
            _StreamManager.GenerateCallStacks = true;
            _StreamManager.AggressiveBufferReturn = true;
            _StreamManager.MaximumFreeLargePoolBytes = MEM_STREAM_MAX_BUFFER_SIZE * 4;
            _StreamManager.MaximumFreeSmallPoolBytes = 15 * MEM_STREAM_BLOCK_SIZE;
        }

        public virtual void SetData(Command c)
        {
            MemoryStream stream = _StreamManager.GetStream();
            BinaryWriter writer = new BinaryWriter(stream);

            c.Serialize(ref writer);

            _Data = stream.ToArray();

            writer.Dispose();
        }

        public virtual void RouteCommand(byte[] b)
        {
            MemoryStream stream = _StreamManager.GetStream(b);
            BinaryReader reader = new BinaryReader(stream);

            CommandQueueManager.RouteIn(ref reader);

            reader.Dispose();
        }

        public virtual void RouteCommand()
        {
            MemoryStream stream = _StreamManager.GetStream(Data);
            BinaryReader reader = new BinaryReader(stream);

            CommandQueueManager.RouteIn(ref reader);

            reader.Dispose();
        }

    }
}
