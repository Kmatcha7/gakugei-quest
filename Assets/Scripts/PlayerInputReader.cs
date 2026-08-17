using UnityEngine;
using UnityEngine.InputSystem;

namespace SimpleRpg
{
    /// <summary>
    /// 新Input Systemでプレイヤーの移動入力を受け取るクラスです。
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class PlayerInputReader : MonoBehaviour
    {
        /// <summary>
        /// 入力のデッドゾーンです。
        /// </summary>
        [SerializeField]
        float _deadZone = 0.2f;

        /// <summary>
        /// 移動入力を取得するアクションです。
        /// </summary>
        InputAction _moveAction;

        /// <summary>
        /// 取得した移動入力です。
        /// </summary>
        public Vector2 MoveInput { get; private set; }

        void OnEnable()
        {
            if (_moveAction == null)
            {
                _moveAction = new InputAction("Move", InputActionType.Value);
                _moveAction.AddBinding("<Gamepad>/leftStick");
                _moveAction.AddBinding("<Gamepad>/dpad");
                _moveAction.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/w")
                    .With("Down", "<Keyboard>/s")
                    .With("Left", "<Keyboard>/a")
                    .With("Right", "<Keyboard>/d");
                _moveAction.AddCompositeBinding("2DVector")
                    .With("Up", "<Keyboard>/upArrow")
                    .With("Down", "<Keyboard>/downArrow")
                    .With("Left", "<Keyboard>/leftArrow")
                    .With("Right", "<Keyboard>/rightArrow");
            }

            _moveAction.Enable();
        }

        void OnDisable()
        {
            _moveAction?.Disable();
        }

        void OnDestroy()
        {
            _moveAction?.Dispose();
        }

        void Update()
        {
            var value = _moveAction.ReadValue<Vector2>();
            MoveInput = value.sqrMagnitude < _deadZone * _deadZone ? Vector2.zero : value;
        }
    }
}
