using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// キャラクターの移動を行うクラスの基底クラスです。
    /// </summary>
    public class CharacterMover : MonoBehaviour
    {
        /// <summary>
        /// キャラクターの移動にかかる時間です。
        /// </summary>
        [SerializeField]
        protected float _moveTime = 0.25f;

        /// <summary>
        /// Tilemapを管理するクラスへの参照です。
        /// </summary>
        protected TilemapManager _tilemapManager;

        /// <summary>
        /// 移動するキャラクターのAnimatorです。
        /// </summary>
        protected Animator _animator;

        /// <summary>
        /// 移動するキャラクターのColliderです。
        /// </summary>
        protected BoxCollider2D _boxCollider2d;

        /// <summary>
        /// Tilemap上における現在の論理的な座標です。
        /// </summary>
        protected Vector3Int _posOnTile = Vector3Int.zero;

        /// <summary>
        /// Tilemap上における現在の論理的な座標です。
        /// </summary>
        public Vector3Int PosOnTile
        {
            get {return _posOnTile;}
        }

        /// <summary>
        /// 移動中かどうかのフラグです。
        /// </summary>
        protected bool _isMoving;

        /// <summary>
        /// 移動中かどうかのフラグです。
        /// </summary>
        public bool IsMoving
        {
            get {return _isMoving;}
        }

        /// <summary>
        /// 移動できる状況かどうかのフラグです。
        /// </summary>
        protected bool _isMovingPaused;

        /// <summary>
        /// 移動できる状況かどうかのフラグです。
        /// </summary>
        public bool IsMovingPaused
        {
            get {return _isMovingPaused;}
        }

        /// <summary>
        /// 現在向いている方向です。
        /// </summary>
        protected MoveAnimationDirection _animationDirection;

        /// <summary>
        /// 現在向いている方向です。
        /// </summary>
        public MoveAnimationDirection AnimationDirection
        {
            get {return _animationDirection;}
        }

        /// <summary>
        /// 移動後に実行する処理を確認するフラグです。
        /// </summary>
        protected bool _isCheckPostMove = true;

        /// <summary>
        /// 移動後に実行する処理を確認するコールバックです。
        /// </summary>
        protected ICharacterMoveCallback _moveCallback;

        protected virtual void Start()
        {
            CheckComponents();
            GetCurrentPositionOnTilemap();
            RegisterComponent();
        }

        protected void OnEnable()
        {
            CheckComponents();
            GetCurrentPositionOnTilemap();
            RegisterComponent();
        }

        void Update()
        {
            
        }

        /// <summary>
        /// 必要なコンポーネントへの参照を確認します。
        /// </summary>
        protected virtual void CheckComponents()
        {
            if (_animator == null)
            {
                _animator = gameObject.GetComponent<Animator>();
            }

            if (_boxCollider2d == null)
            {
                _boxCollider2d = gameObject.GetComponent<BoxCollider2D>();
            }

            if (_tilemapManager == null)
            {
                _tilemapManager = FindAnyObjectByType<TilemapManager>();
            }
        }

        /// <summary>
        /// Tilemap上の論理的な座標を取得します。
        /// </summary>
        protected void GetCurrentPositionOnTilemap()
        {
            if (_tilemapManager == null)
            {
                return;
            }

            _posOnTile = _tilemapManager.GetPositionOnTilemap(gameObject.transform.position);
        }

        /// <summary>
        /// 管理クラスにこのコンポーネントを登録します。
        /// </summary>
        protected void RegisterComponent()
        {
            var manager = FindAnyObjectByType<CharacterMoverManager>();
            if (manager != null)
            {
                manager.RegisterCharacterMover(this);
            }
        }

        /// <summary>
        /// 引数のアニメーションの方向から移動方向を取得します。
        /// </summary>
        /// <param name="animDirection">アニメーションの方向</param>
        public virtual Vector2Int GetMoveDirection(MoveAnimationDirection animDirection)
        {
            var moveDirection = Vector2Int.zero;
            switch (animDirection)
            {
                case MoveAnimationDirection.Front:
                    moveDirection = Vector2Int.down;
                    break;
                case MoveAnimationDirection.Right:
                    moveDirection = Vector2Int.right;
                    break;
                case MoveAnimationDirection.Back:
                    moveDirection = Vector2Int.up;
                    break;
                case MoveAnimationDirection.Left:
                    moveDirection = Vector2Int.left;
                    break;
            }
            return moveDirection;
        }

        /// <summary>
        /// キャラクターを外部から移動させます。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        /// <param name="animDirection">アニメーションの方向</param>
        public virtual void ForceMoveCharacter(MoveAnimationDirection direction, int steps, bool checkPostMove, ICharacterMoveCallback moveCallback)
        {
            _moveCallback = moveCallback;
            ResumeAnimation();
            StartCoroutine(ForceMoveCharacterProcess(direction, steps, checkPostMove));
        }

        /// <summary>
        /// キャラクターを外部から移動させます。
        /// </summary>
        /// <param name="direction">移動方向</param>
        /// <param name="steps">移動するステップ数</param>
        /// <param name="checkPostMove">移動後の処理を確認するかどうか</param>
        IEnumerator ForceMoveCharacterProcess(MoveAnimationDirection direction, int steps, bool checkPostMove)
        {
            _isCheckPostMove = checkPostMove;
            var moveDirection = GetMoveDirection(direction);
            for (int i = 0; i < steps; i++)
            {
                while (_isMoving)
                {
                    // キャラクターが移動中の場合は待機します。
                    yield return null;
                }

                // キャラクターを移動させます。
                _isMovingPaused = false;
                MoveCharacter(moveDirection, direction);
            }
            _isCheckPostMove = true;
            _isMovingPaused = false;

            if (_moveCallback != null)
            {
                _moveCallback.OnFinishedMove();
            }
        }

        /// <summary>
        /// キャラクターを移動させます。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        /// <param name="animDirection">アニメーションの方向</param>
        protected virtual void MoveCharacter(Vector2Int moveDirection, MoveAnimationDirection animDirection)
        {
            if (moveDirection == Vector2Int.zero)
            {
                return;
            }

            // 移動機能が一時停止されている場合は処理を抜けます。
            if (_isMovingPaused)
            {
                return;
            }

            // 移動方向に応じたアニメーションに切り替えます。
            SetCharacterDirection(animDirection);

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

            // 論理的な座標から移動先の座標を計算し、ワールド座標に変換します。
            _posOnTile = targetPos;
            var targetWorldPos = _tilemapManager.GetWorldPosition(_posOnTile);

            // コルーチンを使ってキャラクターのゲームオブジェクトを移動させます。
            _isMoving = true;
            _tilemapManager.ReservePosition(GetInstanceID(), targetPos);
            StartCoroutine(MovePlayerProcess(sourcePos, targetWorldPos));
        }

        /// <summary>
        /// キャラクターを移動させるコルーチンです。
        /// </summary>
        /// <param name="sourcePos">移動元の位置</param>
        /// <param name="targetPos">移動先の位置</param>
        protected IEnumerator MovePlayerProcess(Vector3 sourcePos, Vector3 targetPos)
        {
            // 完了後の時間を算出して、それまでの間、毎フレームLerpメソッドで位置を計算します。
            var animFinishTime = Time.time + _moveTime;
            var startedTime = Time.time;
            var pausedTime = 0.0f;
            while (Time.time < animFinishTime)
            {
                if (_isMovingPaused)
                {
                    pausedTime += Time.deltaTime;
                    animFinishTime += Time.deltaTime;
                    yield return null;
                    continue;
                }

                // 開始時刻からの経過時間を計算します。移動が停止している場合は経過時間から除外します。
                var elapsedTime = Time.time - startedTime - pausedTime;
                var rate = Mathf.Clamp01(elapsedTime / _moveTime);
                Vector3 pos = Vector3.Lerp(sourcePos, targetPos, rate);
                gameObject.transform.position = pos;
                yield return null;
            }

            // 最終的な位置を設定して移動中フラグをfalseにします。
            gameObject.transform.position = targetPos;
            _isMoving = false;
            PostMove();
        }

        /// <summary>
        /// キャラクター移動後の処理です。
        /// </summary>
        protected virtual void PostMove()
        {

        }

        /// <summary>
        /// 移動先に他のキャラクターがいるかどうか確認します。
        /// いる場合にTrueを返します。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        /// <param name="worldPos">移動元のワールド座標</param>
        protected virtual bool ExistsOtherCharacter(Vector2Int moveDirection, Vector3 worldPos)
        {
            if (_boxCollider2d == null)
            {
                return false;
            }

            List<RaycastHit2D> raycastHits = GetOtherCharacter(moveDirection);

            // 返ってきた結果からColliderを取得してタグを確認します。
            bool exist = false;
            foreach (var hit in raycastHits)
            {
                var collider = hit.collider;
                if (collider == null)
                {
                    continue;
                }

                if (IsCollisionTargetTag(collider))
                {
                    exist = true;
                    break;
                }
            }

            return exist;
        }

        /// <summary>
        /// 移動先にいるキャラクターのコライダーを取得します。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        public virtual List<RaycastHit2D> GetOtherCharacter(Vector2Int moveDirection)
        {
            // Rayを飛ばすにあたって、フィルター、結果を格納するリスト、距離を定義します。
            ContactFilter2D filter2D = new();
            List<RaycastHit2D> raycastHits = new();
            float distance = GetRayDistance(moveDirection);
            _boxCollider2d.Raycast(moveDirection, filter2D.NoFilter(), raycastHits, distance);
            return raycastHits;
        }

        /// <summary>
        /// レイを飛ばす距離を取得します。
        /// </summary>
        /// <param name="moveDirection">移動方向</param>
        protected virtual float GetRayDistance(Vector2Int moveDirection)
        {
            // 向いている方向のタイルがイベントの実行時にひとつ先まで確認する対象のタイルかどうかを確認します。
            Vector3Int targetPos = _posOnTile + (Vector3Int)moveDirection;
            bool isThroughTile = _tilemapManager.IsThroughTile(targetPos);

            // イベントの実行時にひとつ先まで確認する対象のタイルならもう1マス先までレイを飛ばします。
            return isThroughTile ? 2.0f : 1.0f;
        }

        /// <summary>
        /// Colliderのタグが、移動しない対象のものか確認します。
        /// </summary>
        /// <param name="collider">RayがHitしたCollider</param>
        protected virtual bool IsCollisionTargetTag(Collider2D collider)
        {
            bool isTargetTag = collider.CompareTag(ObjectTagSettings.Npc)
                            || collider.CompareTag(ObjectTagSettings.Player);
            return isTargetTag;
        }

        /// <summary>
        /// キャラクターのアニメーションの方向を更新します。
        /// </summary>
        /// <param name="animDirection">アニメーションの方向</param>
        public virtual void SetCharacterDirection(MoveAnimationDirection animDirection)
        {
            if (_animator != null)
            {
                _animator.SetInteger(AnimationSettings.DirectionParameterName, (int)animDirection);
            }
        }

        /// <summary>
        /// アニメーションを停止します。
        /// </summary>
        public virtual void StopAnimation()
        {
            if (_animator != null)
            {
                _animator.speed = 0;
            }
        }

        /// <summary>
        /// アニメーションを再開します。
        /// </summary>
        public virtual void ResumeAnimation()
        {
            if (_animator != null)
            {
                _animator.speed = 1;
            }
        }

        /// <summary>
        /// キャラクターの移動を停止します。
        /// </summary>
        public virtual void StopMoving()
        {
            _isMovingPaused = true;
        }

        /// <summary>
        /// キャラクターの移動を再開します。
        /// </summary>
        public virtual void ResumeMoving()
        {
            _isMovingPaused = false;
        }

        /// <summary>
        /// タイル上の位置を再取得します。
        /// </summary>
        public virtual void ResetPosition()
        {
            GetCurrentPositionOnTilemap();
        }

        /// <summary>
        /// タイル上の位置からワールド座標を設定します。
        /// </summary>
        /// <param name="pos">タイル上の座標</param>
        public virtual void SetPosition(Vector3Int pos)
        {
            _posOnTile = pos;
            var targetWorldPos = _tilemapManager.GetWorldPosition(_posOnTile);
            gameObject.transform.position = targetWorldPos;
        }
    }
}