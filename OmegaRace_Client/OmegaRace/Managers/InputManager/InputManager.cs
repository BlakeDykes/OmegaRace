using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OmegaRace
{
    public struct InputAxis
    {
        int _Horizontal;
        int _Vertical;

        public int Horizontal { get => _Horizontal; }
        public int Vertical { get => _Vertical; }


        public void Set(int horizontal, int vertical)
        {
            _Horizontal = horizontal;
            _Vertical = vertical;
        }
    }

    class InputManager
    {
        private static InputManager instance;
        private static InputManager Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new InputManager();
                }
                return instance;
            }
        }

        private Dictionary<Azul.AZUL_KEY, KeyState> Keys;
        private List<InputMap> InputMaps;

        private InputManager()
        {
            InputMaps = new List<InputMap>();
            Keys = new Dictionary<Azul.AZUL_KEY, KeyState>();

            Keys.Add(Azul.AZUL_KEY.KEY_P, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_0, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_1, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_2, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_A, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_D, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_W, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_S, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_SPACE, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_F, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_C, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_I, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_J, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_L, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_K, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_H, new KeyState());
            Keys.Add(Azul.AZUL_KEY.KEY_N, new KeyState());
        }
        public static void AddMap(InputMap map)
        {
            Instance.InputMaps.Add(map);
        }

        public static void Update()
        {
            Instance.KeyStateUpdate();

            Instance.HandleInputs();
        }

        public static int GetAxis(Azul.AZUL_KEY[] keys)
        {
            return CalculateAxis(Instance.Keys[keys[0]], Instance.Keys[keys[1]]);
        }

        public static bool GetButton(Azul.AZUL_KEY key)
        {
            return CalculateButton(Instance.Keys[key]);
        }

        public static bool GetButtonDown(Azul.AZUL_KEY key)
        {
            return CalculateButtonDown(Instance.Keys[key]);
        }

        public static bool GetButtonUp(Azul.AZUL_KEY key)
        {
            return CalculateButtonUp(Instance.Keys[key]);
        }

        private void KeyStateUpdate()
        {
            foreach(KeyValuePair<Azul.AZUL_KEY, KeyState> key in Keys)
            {
                key.Value.Update(key.Key);
            }
        }

        private void HandleInputs()
        {
            foreach(InputMap map in InputMaps)
            {
                map.HandleInputs(Keys);
            }
        }

        private static bool CalculateButton(MouseButtonState key)
        {
            return key.Pressed();
        }

        private static bool CalculateButton(KeyState key)
        {
            return key.Pressed();
        }

        private static bool CalculateButtonDown(MouseButtonState key)
        {
            return key.PressedDown();
        }
        private static bool CalculateButtonDown(KeyState key)
        {
            return key.PressedDown();
        }

        private static bool CalculateButtonUp(MouseButtonState key)
        {
            return key.PressedUp();
        }

        private static bool CalculateButtonUp(KeyState key)
        {
            return key.PressedUp();
        }

        private static int CalculateAxis(KeyState positiveKey, KeyState negativeKey)
        {
            return (positiveKey.Pressed() ? 1:0) - (negativeKey.Pressed()?1:0);
        }
    }

    


}
