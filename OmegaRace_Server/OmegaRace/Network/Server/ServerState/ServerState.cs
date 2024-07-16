using System;
using System.Diagnostics;

namespace OmegaRace
{
    public abstract class ServerState
    {
        public abstract void Update(byte[] playerIds);
    }
}
