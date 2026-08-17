using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// NPCの移動制御を行うクラスです。
    /// </summary>
    public class NpcMover : CharacterMover
    {
        /// <summary>
        /// NPCの移動の頻度です。
        /// </summary>
        [SerializeField]
        MoveFrequency _moveFrequency;

        /// <summary>
        /// 移動してからの経過時間です。
        /// </summary>
        float _elapsedTime;

        /// <summary>
        /// 移動後のインターバル時間です。
        /// </summary>
        float _intervalTime;

        new void Start()
        {
            base.Start();
            SetIntervalTime();
        }

        void Update()
        {
            MoveNpc();
        }

        /// <summary>
        /// 移動頻度に応じてインターバル時間をセットします。
        /// </summary>
        void SetIntervalTime()
        {
            switch (_moveFrequency)
            {
                case MoveFrequency.Rarely:
                    _intervalTime = 5.0f;
                    break;
                case MoveFrequency.Sometimes:
                    _intervalTime = 3.0f;
                    break;
                case MoveFrequency.Often:
                    _intervalTime = 1.0f;
                    break;
                case MoveFrequency.Always:
                    _intervalTime = 0.0f;
                    break;
                default:
                    _intervalTime = -1.0f;
                    break;
            }
        }

        /// <summary>
        /// NPCの移動に関するメイン処理です。
        /// </summary>
        void MoveNpc()
        {
            // 移動のポーズフラグがtrueなら処理を抜けます。
            if (_isMovingPaused)
            {
                return;
            }

            if (!IsFinishedInterval())
            {
                return;
            }

            _animationDirection = GetDirection();
            Vector2Int moveDirection = GetMoveDirection(_animationDirection);
            MoveCharacter(moveDirection, _animationDirection);
        }

        /// <summary>
        /// 移動後のインターバルが完了したかどうかを返します。
        /// </summary>
        bool IsFinishedInterval()
        {
            if (_isMoving)
            {
                return false;
            }

            if (_moveFrequency == MoveFrequency.Never)
            {
                return false;
            }

            _elapsedTime += Time.deltaTime;
            bool isFinished = false;
            if (_elapsedTime >= _intervalTime)
            {
                isFinished = true;
                _elapsedTime = 0f;
                SetIntervalTime();
            }

            return isFinished;
        }

        /// <summary>
        /// 移動先の方向をランダムに取得します。
        /// </summary>
        MoveAnimationDirection GetDirection()
        {
            int direction = Random.Range(0, 4);
            MoveAnimationDirection animDirection = (MoveAnimationDirection) direction;
            return animDirection;
        }

        /// <summary>
        /// キャラクターを移動させます。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        /// <param name="animDirection">アニメーションの方向</param>
        protected override void MoveCharacter(Vector2Int moveDirection, MoveAnimationDirection animDirection)
        {
            if (moveDirection == Vector2Int.zero)
            {
                return;
            }

            // 移動できるかの確認を行います。
            var targetPos = _posOnTile + (Vector3Int)moveDirection;
            bool canMove = _tilemapManager.CanEntryTile(targetPos);
            if (!canMove)
            {
                return;
            }

            // 移動先にキャラクターがいる場合は処理を抜けます。
            var sourcePos = gameObject.transform.position;
            bool existsCharacter = ExistsOtherCharacter(moveDirection, sourcePos);
            if (existsCharacter)
            {
                return;
            }

            // 移動方向に応じたアニメーションに切り替えます。
            if (_animator != null)
            {
                _animator.SetInteger(AnimationSettings.DirectionParameterName, (int)animDirection);
            }

            // 論理的な座標から移動先の座標を計算し、ワールド座標に変換します。
            _posOnTile = targetPos;
            var targetWorldPos = _tilemapManager.GetWorldPosition(_posOnTile);

            // コルーチンを使ってキャラクターのゲームオブジェクトを移動させます。
            _isMoving = true;
            _tilemapManager.ReservePosition(GetInstanceID(), targetPos);
            StartCoroutine(MovePlayerProcess(sourcePos, targetWorldPos));
        }
    }
}