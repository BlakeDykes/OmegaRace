using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.IO;

namespace OmegaRace
{
    public enum PREDICTION_TYPE
    {
        PosP_Null,
        PosP_ClientSide,
        PosP_DReck
    }

    class PredictionManager
    {
        private const int POOL_REFRESH_COUNT = 10;
        private static PredictionManager instance = null;


        private Type _Type;
        private Stack<PositionPredictor> _Pool;

        public PredictionManager(PREDICTION_TYPE predType)
        {
            _Type = Type.GetType(typeof(PREDICTION_TYPE).Namespace + "." + predType.ToString());

            _Pool = new Stack<PositionPredictor>();

            RefreshPool();

        }

        public static void Initialize(PREDICTION_TYPE predType)
        {
            instance = instance == null ? new PredictionManager(predType) : instance;
        }

        public static void ReturnPrediction(PositionPredictor p)
        {
            p.Clear();
            GetInstance()._Pool.Push(p);
        }

        public static PositionPredictor GetPrediction(PredictableObject subject)
        {
            PredictionManager inst = GetInstance();

            if (inst._Pool.Count <= 0)
            {
                inst.RefreshPool();
            }

            PositionPredictor ret = inst._Pool.Pop();
            ret.Set(subject);

            return ret;
        }

        private void RefreshPool()
        {
            while (_Pool.Count < POOL_REFRESH_COUNT)
            {
                _Pool.Push((PositionPredictor)Activator.CreateInstance(_Type));
            }
        }

        private static PredictionManager GetInstance()
        {
            return instance;
        }
    }
}
