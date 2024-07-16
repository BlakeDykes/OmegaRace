using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaRace
{
    public class TimeManager
    {
        private static TimeManager instance = null;

        private static TimeManager Instance()
        {
            if (instance == null)
            {
                instance = new TimeManager();
            }
            return instance;
        }


        float prevTime;
        float currentTime;
        float frameTime;
        float timeOffset;

        public static void Initialize(float now)
        {
            TimeManager inst = Instance();
            inst.prevTime = now;
            inst.currentTime = now;
            inst.frameTime = 0;
            inst.timeOffset = 0;
        }
        static public void Update(float now)
        {
            TimeManager inst = Instance();

            inst.prevTime = inst.currentTime;
            inst.currentTime = now + inst.timeOffset;
            inst.frameTime = inst.currentTime - inst.prevTime;

            ScreenLog.Add(string.Format("Time: {0}:{1,2:D2}:{2}", 
                (int)inst.currentTime / 60, 
                (int)inst.currentTime % 60, 
                ((inst.currentTime % 1).ToString().TrimStart('0', '.'))));
        }

        static public void SetCurrentTime_Cristians(float t, float t0)
        {
            TimeManager inst = Instance();

            float newTime = t + ((inst.currentTime - t0) * .05f);
            inst.timeOffset = newTime - inst.currentTime;
            inst.currentTime = newTime;
            inst.prevTime = inst.currentTime;
        }

        public static float GetCurrentTime() { return Instance().currentTime; }
        public static float GetFrameTime() { return Instance().frameTime; }

    }
}
