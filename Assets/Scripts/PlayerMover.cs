using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 操作キャラの移動制御を行うクラスです。
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader))]
    public class PlayerMover : CharacterMover
    {
        /// <summary>
        /// 操作キャラの移動に伴うイベントの確認を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        PlayerEventChecker _playerEventChecker;

        /// <summary>
        /// 敵キャラクターとのエンカウントを管理するクラスへの参照です。
        /// </summary>
        EncounterManager _encounterManager;

        /// <summary>
        /// 新Input Systemの入力を受け取るクラスへの参照です。
        /// </summary>
        [SerializeField]
        PlayerInputReader _inputReader;

        protected override void Start()
        {
            base.Start();
            _playerEventChecker.SetUpReference(this);
        }

        void Update()
        {
            CheckMoveInput();
            _playerEventChecker.CheckEventInput();
        }

        /// <summary>
        /// 参照を取得します。
        /// </summary>
        void GetReference()
        {
            if (_inputReader == null)
            {
                _inputReader = GetComponent<PlayerInputReader>();
            }

            if (_encounterManager == null)
            {
                _encounterManager = FindAnyObjectByType<EncounterManager>();
            }
        }

        /// <summary>
        /// キー入力を確認します。
        /// </summary>
        void CheckMoveInput()
        {
            // イベントや戦闘中は移動入力を受け付けません。
            if (GameStateManager.CurrentState != GameState.Moving)
            {
                return;
            }

            // 既に移動中の場合は移動キーの入力を確認せず抜けます。
            if (_isMoving)
            {
                return;
            }

            // 移動のポーズフラグがtrueなら処理を抜けます。
            if (_isMovingPaused)
            {
                return;
            }

            var moveInput = GetMoveInput();
            if (moveInput == Vector2.zero)
            {
                // 移動キーが押されていない場合は処理を抜けます。
                return;
            }

            // 斜め移動は行わないため、上下左右のいずれかを移動対象とします。
            if (Mathf.Abs(moveInput.y) > Mathf.Abs(moveInput.x))
            {
                _animationDirection = moveInput.y > 0 ? MoveAnimationDirection.Back : MoveAnimationDirection.Front;
            }
            else
            {
                _animationDirection = moveInput.x > 0 ? MoveAnimationDirection.Right : MoveAnimationDirection.Left;
            }

            var moveDirection = GetMoveDirection(_animationDirection);
            MoveCharacter(moveDirection, _animationDirection);
        }

        /// <summary>
        /// 入力デバイスから移動入力を取得します。
        /// </summary>
        Vector2 GetMoveInput()
        {
            if (_inputReader == null)
            {
                _inputReader = GetComponent<PlayerInputReader>();
            }

            if (_inputReader == null)
            {
                SimpleLogger.Instance.LogError("PlayerInputReaderが見つからず、移動入力を取得できません。プレイヤーにコンポーネントが付いているか確認してください。");
                return Vector2.zero;
            }

            return _inputReader.MoveInput;
        }

        /// <summary>
        /// キャラクター移動後の処理です。
        /// </summary>
        protected override void PostMove()
        {
            // イベント中など、移動後の処理を行わない場合は処理を抜けます。
            if (!_isCheckPostMove)
            {
                return;
            }

            // 移動後のマスにイベントがあるかどうかを確認します。
            if (_playerEventChecker.CheckOnTileEvent())
            {
                return;
            }

            // イベントがない場合は、エンカウントの確認を行います。
            CheckEncounter();
        }

        /// <summary>
        /// エンカウントが発生するかどうかを確認します。
        /// </summary>
        void CheckEncounter()
        {
            GetReference();
            if (_encounterManager != null)
            {
                // エンカウントの確認を行います。
                _encounterManager.CheckEncounter();
            }
            else
            {
                SimpleLogger.Instance.LogError("EncounterManagerが見つかりませんでした。");
            }
        }
    }
}
