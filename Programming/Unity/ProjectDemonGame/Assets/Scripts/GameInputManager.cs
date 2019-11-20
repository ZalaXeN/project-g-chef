using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class GameInputManager : MonoBehaviour
    {
        public static bool GetKey(KeyCode keyCode)
        {
            return Input.GetKey(keyCode);
        }

        public static void MappingInputKeyCodeWithCurrentKeyCode(KeyCode keyCode)
        {
            keyCode = Event.current.keyCode;
        }
    }
}
