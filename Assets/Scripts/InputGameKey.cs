using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace SimpleRpg
{
    /// <summary>
    /// ゲーム内のキー入力を定義するクラスです。
    /// </summary>
    public static class InputGameKey
    {
#if ENABLE_INPUT_SYSTEM
        const float StickThreshold = 0.3f;
        static bool _gamepadUpHeld;
        static bool _gamepadDownHeld;
        static bool _gamepadLeftHeld;
        static bool _gamepadRightHeld;

        static bool GetGamepadEdge(bool current, ref bool heldFlag)
        {
            var pressed = current && !heldFlag;
            heldFlag = current;
            return pressed;
        }

        /// <summary>
        /// 決定ボタンが押されたかどうかを取得します。
        /// </summary>
        public static bool ConfirmButton()
        {
            var pressed = Keyboard.current != null
                && (Keyboard.current.enterKey.wasPressedThisFrame
                    || Keyboard.current.spaceKey.wasPressedThisFrame
                    || Keyboard.current.zKey.wasPressedThisFrame);

            if (!pressed && Gamepad.current != null)
            {
                pressed = Gamepad.current.buttonEast.wasPressedThisFrame;
            }

            return pressed;
        }

        /// <summary>
        /// キャンセルボタンが押されたかどうかを取得します。
        /// </summary>
        public static bool CancelButton()
        {
            var pressed = Keyboard.current != null
                && (Keyboard.current.escapeKey.wasPressedThisFrame
                    || Keyboard.current.xKey.wasPressedThisFrame);

            if (!pressed && Gamepad.current != null)
            {
                pressed = Gamepad.current.buttonSouth.wasPressedThisFrame;
            }

            return pressed;
        }

        /// <summary>
        /// 上方向の入力があったかどうかを取得します。
        /// </summary>
        public static bool MenuUp()
        {
            var pressed = Keyboard.current != null
                && Keyboard.current.upArrowKey.wasReleasedThisFrame;

            if (!pressed && Gamepad.current != null)
            {
                pressed = Gamepad.current.dpad.up.wasPressedThisFrame;
                if (!pressed)
                {
                    var stick = Gamepad.current.leftStick.ReadValue();
                    var current = stick.y > StickThreshold;
                    pressed = GetGamepadEdge(current, ref _gamepadUpHeld);
                }
            }

            return pressed;
        }

        /// <summary>
        /// 下方向の入力があったかどうかを取得します。
        /// </summary>
        public static bool MenuDown()
        {
            var pressed = Keyboard.current != null
                && Keyboard.current.downArrowKey.wasReleasedThisFrame;

            if (!pressed && Gamepad.current != null)
            {
                pressed = Gamepad.current.dpad.down.wasPressedThisFrame;
                if (!pressed)
                {
                    var stick = Gamepad.current.leftStick.ReadValue();
                    var current = stick.y < -StickThreshold;
                    pressed = GetGamepadEdge(current, ref _gamepadDownHeld);
                }
            }

            return pressed;
        }

        /// <summary>
        /// 左方向の入力があったかどうかを取得します。
        /// </summary>
        public static bool MenuLeft()
        {
            var pressed = Keyboard.current != null
                && Keyboard.current.leftArrowKey.wasReleasedThisFrame;

            if (!pressed && Gamepad.current != null)
            {
                pressed = Gamepad.current.dpad.left.wasPressedThisFrame;
                if (!pressed)
                {
                    var stick = Gamepad.current.leftStick.ReadValue();
                    var current = stick.x < -StickThreshold;
                    pressed = GetGamepadEdge(current, ref _gamepadLeftHeld);
                }
            }

            return pressed;
        }

        /// <summary>
        /// 右方向の入力があったかどうかを取得します。
        /// </summary>
        public static bool MenuRight()
        {
            var pressed = Keyboard.current != null
                && Keyboard.current.rightArrowKey.wasReleasedThisFrame;

            if (!pressed && Gamepad.current != null)
            {
                pressed = Gamepad.current.dpad.right.wasPressedThisFrame;
                if (!pressed)
                {
                    var stick = Gamepad.current.leftStick.ReadValue();
                    var current = stick.x > StickThreshold;
                    pressed = GetGamepadEdge(current, ref _gamepadRightHeld);
                }
            }

            return pressed;
        }
#else
        /// <summary>
        /// 決定ボタンが押されたかどうかを取得します。
        /// </summary>
        public static bool ConfirmButton()
        {
            return Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Z);
        }

        /// <summary>
        /// キャンセルボタンが押されたかどうかを取得します。
        /// </summary>
        public static bool CancelButton()
        {
            return Input.GetKeyDown(KeyCode.Escape)
                || Input.GetKeyDown(KeyCode.X);
        }

        /// <summary>
        /// 上方向の入力があったかどうかを取得します。
        /// </summary>
        public static bool MenuUp()
        {
            return Input.GetKeyUp(KeyCode.UpArrow);
        }

        /// <summary>
        /// 下方向の入力があったかどうかを取得します。
        /// </summary>
        public static bool MenuDown()
        {
            return Input.GetKeyUp(KeyCode.DownArrow);
        }

        /// <summary>
        /// 左方向の入力があったかどうかを取得します。
        /// </summary>
        public static bool MenuLeft()
        {
            return Input.GetKeyUp(KeyCode.LeftArrow);
        }

        /// <summary>
        /// 右方向の入力があったかどうかを取得します。
        /// </summary>
        public static bool MenuRight()
        {
            return Input.GetKeyUp(KeyCode.RightArrow);
        }
#endif
    }
}
