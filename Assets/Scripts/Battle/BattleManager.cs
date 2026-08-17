using System.Collections;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 戦闘に関する機能を管理するクラスです。
    /// </summary>
    public class BattleManager : MonoBehaviour, IMessageCallback
    {
        /// <summary>
        /// 戦闘開始の処理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleStarter _battleStarter;

        /// <summary>
        /// 戦闘関連のウィンドウ全体を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleWindowManager _battleWindowManager;

        /// <summary>
        /// 戦闘関連のスプライトを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleSpriteController _battleSpriteController;

        /// <summary>
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        EnemyStatusManager _enemyStatusManager;

        /// <summary>
        /// 敵キャラクターのコマンドを選択するクラスへの参照です。
        /// </summary>
        [SerializeField]
        EnemyCommandSelector _enemyCommandSelector;

        /// <summary>
        /// 戦闘中のアクションを登録するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionRegister _battleActionRegister;

        /// <summary>
        /// 戦闘中のアクションを処理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleActionProcessor _battleActionProcessor;

        /// <summary>
        /// 戦闘の結果処理を管理するクラスへの参照です。
        /// </summary>
        [SerializeField]
        BattleResultManager _battleResultManager;

        /// <summary>
        /// 戦闘完了の通知を行うインタフェースです。
        /// </summary>
        IPostBattle _postBattle;

        /// <summary>
        /// 戦闘のフェーズです。
        /// </summary>
        public BattlePhase BattlePhase { get; private set; }

        /// <summary>
        /// 選択されたコマンドです。
        /// </summary>
        public BattleCommand SelectedCommand { get; private set; }

        /// <summary>
        /// エンカウントした敵キャラクターのIDです。
        /// </summary>
        public int EnemyId { get; private set; }

        /// <summary>
        /// 戦闘開始からのターン数です。
        /// </summary>
        public int TurnCount { get; private set; }

        /// <summary>
        /// 戦闘が終了したかどうかのフラグです。
        /// </summary>
        public bool IsBattleFinished { get; private set; }

        /// <summary>
        /// 戦闘から逃げられるかどうかのフラグです。
        /// </summary>
        public bool CanRunaway { get; private set; }

        /// <summary>
        /// 逃走選択時のメッセージを処理するかどうかのフラグです。
        /// </summary>
        bool _handleRunnawayMessage;

        /// <summary>
        /// 戦闘のフェーズを変更します。
        /// </summary>
        /// <param name="battlePhase">変更後のフェーズ</param>
        public void SetBattlePhase(BattlePhase battlePhase)
        {
            BattlePhase = battlePhase;
        }

        /// <summary>
        /// 敵キャラクターのステータスをセットします。
        /// </summary>
        /// <param name="enemyId">敵キャラクターのID</param>
        public void SetUpEnemyStatus(int enemyId)
        {
            EnemyId = enemyId;
            _enemyStatusManager.SetUpEnemyStatus(enemyId);
        }

        /// <summary>
        /// 戦闘完了時のコールバックを登録します。
        /// </summary>
        public void RegisterCallback(IPostBattle postBattle)
        {
            _postBattle = postBattle;
        }

        /// <summary>
        /// 戦闘から逃げられるかどうかのフラグをセットします。
        /// </summary>
        /// <param name="canRunaway">逃げられるかどうかのフラグ</param>
        public void SetCanRunaway(bool canRunaway)
        {
            CanRunaway = canRunaway;
        }

        /// <summary>
        /// 戦闘の開始処理を行います。
        /// </summary>
        public void StartBattle()
        {
            SimpleLogger.Instance.Log("戦闘を開始します。");
            GameStateManager.ChangeToBattle();
            SetBattlePhase(BattlePhase.ShowEnemy);

            TurnCount = 1;
            IsBattleFinished = false;

            _battleWindowManager.SetUpWindowControllers(this);
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.HidePager();

            _battleActionProcessor.InitializeProcessor(this);
            _battleActionRegister.InitializeRegister(_battleActionProcessor);
            _enemyCommandSelector.SetReferences(this, _battleActionRegister);
            _battleResultManager.SetReferences(this);
            _battleStarter.StartBattle(this);
        }

        /// <summary>
        /// ウィンドウの管理を行うクラスへの参照を取得します。
        /// </summary>
        public BattleWindowManager GetWindowManager()
        {
            return _battleWindowManager;
        }

        /// <summary>
        /// 戦闘関連のスプライトを制御するクラスへの参照を取得します。
        /// </summary>
        public BattleSpriteController GetBattleSpriteController()
        {
            return _battleSpriteController;
        }

        /// <summary>
        /// 戦闘中の敵キャラクターの管理を行うクラスへの参照を取得します。
        /// </summary>
        public EnemyStatusManager GetEnemyStatusManager()
        {
            return _enemyStatusManager;
        }

        /// <summary>
        /// コマンド入力を開始します。
        /// </summary>
        public void StartInputCommandPhase()
        {
            SimpleLogger.Instance.Log($"コマンド入力のフェーズを開始します。現在のターン数: {TurnCount}");
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.HideWindow();
            BattlePhase = BattlePhase.InputCommand;
            _battleActionProcessor.InitializeActions();
            var commandWindow = _battleWindowManager.GetCommandWindowController();
            commandWindow.SetCanSelectFlag(true);
        }

        /// <summary>
        /// コマンドが選択された時のコールバックです。
        /// </summary>
        public void OnCommandSelected(BattleCommand selectedCommand)
        {
            SimpleLogger.Instance.Log($"コマンドが選択されました: {selectedCommand}");
            SelectedCommand = selectedCommand;
            HandleCommand();
        }

        /// <summary>
        /// コマンド入力に応じた処理を行います。
        /// </summary>
        void HandleCommand()
        {
            switch (SelectedCommand)
            {
                case BattleCommand.Attack:
                    SetAttackCommandAction();
                    break;
                case BattleCommand.Run:
                    SetRunCommandAction();
                    break;
                case BattleCommand.Magic:
                case BattleCommand.Item:
                    ShowSelectionWindow();
                    break;
            }
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        void ShowSelectionWindow()
        {
            SimpleLogger.Instance.Log($"ShowSelectionWindow()が呼ばれました。選択されたコマンド: {SelectedCommand}");
            StartCoroutine(ShowSelectionWindowProcess());
        }

         /// <summary>
        /// 選択ウィンドウを表示する処理です。
        /// </summary>
        IEnumerator ShowSelectionWindowProcess()
        {
            yield return null;
            BattlePhase = BattlePhase.SelectItem;
            var selectionWindowController = _battleWindowManager.GetSelectionWindowController();
            selectionWindowController.SetUpWindow();
            selectionWindowController.SetPageElement();
            selectionWindowController.ShowWindow();
            selectionWindowController.SetCanSelectState(true);
        }

        /// <summary>
        /// 選択ウィンドウで項目が選択された時のコールバックです。
        /// </summary>
        public void OnItemSelected(int itemId)
        {
            switch (SelectedCommand)
            {
                case BattleCommand.Magic:
                    SetMagicCommandAction(itemId);
                    break;
                case BattleCommand.Item:
                    SetItemCommandAction(itemId);
                    break;
            }
        }

        /// <summary>
        /// 選択ウィンドウでキャンセルボタンが押された時のコールバックです。
        /// </summary>
        public void OnItemCanceled()
        {
            BattlePhase = BattlePhase.InputCommand;
            var selectionWindowController = _battleWindowManager.GetSelectionWindowController();
            selectionWindowController.HideWindow();
        }

        /// <summary>
        /// 攻撃コマンドを選択した際の処理です。
        /// </summary>
        void SetAttackCommandAction()
        {
            // 1対1の戦闘のため、最初のキャラクターのIDを取得します。
            int actorId = CharacterStatusManager.partyCharacter[0];
            int targetId = _enemyStatusManager.GetEnemyStatusList()[0].enemyBattleId;
            _battleActionRegister.SetFriendAttackAction(actorId, targetId);

            SimpleLogger.Instance.Log($"攻撃するキャラクターのID: {actorId} || 攻撃対象のキャラクターのID: {targetId}");

            PostCommandSelect();
        }

        /// <summary>
        /// 魔法コマンドを選択した際の処理です。
        /// </summary>
        /// <param name="itemId">魔法のID</param>
        void SetMagicCommandAction(int itemId)
        {
            int actorId = CharacterStatusManager.partyCharacter[0];
            int targetId = _enemyStatusManager.GetEnemyStatusList()[0].enemyBattleId;
            _battleActionRegister.SetFriendMagicAction(actorId, targetId, itemId);

            PostCommandSelect();
        }

        /// <summary>
        /// アイテムコマンドを選択した際の処理です。
        /// </summary>
        /// <param name="itemId">アイテムのID</param>
        void SetItemCommandAction(int itemId)
        {
            SimpleLogger.Instance.Log($"SetItemCommandAction()が呼ばれました。選択されたアイテムのID : {itemId}");
            int actorId = CharacterStatusManager.partyCharacter[0];
            var itemData = ItemDataManager.GetItemDataById(itemId);
            if (itemData == null)
            {
                SimpleLogger.Instance.LogError($"選択されたIDのアイテムは見つかりませんでした。ID : {itemId}");
                return;
            }

            int targetId = _enemyStatusManager.GetEnemyStatusList()[0].enemyBattleId;
            _battleActionRegister.SetFriendItemAction(actorId, targetId, itemId);

            PostCommandSelect();
        }

        /// <summary>
        /// 逃げるコマンドを選択した際の処理です。
        /// </summary>
        void SetRunCommandAction()
        {
            // 逃げられない戦闘の場合はメッセージ表示後、コマンド入力フェーズに戻ります。
            if (!CanRunaway)
            {
                ShowCannotRunawayMessage();
                return;
            }

            int actorId = CharacterStatusManager.partyCharacter[0];
            _battleActionRegister.SetFriendRunAction(actorId);

            PostCommandSelect();
        }

        /// <summary>
        /// 逃げられない場合のメッセージを表示します。
        /// </summary>
        void ShowCannotRunawayMessage()
        {
            var commandWindow = _battleWindowManager.GetCommandWindowController();
            commandWindow.SetCanSelectFlag(false);

            _handleRunnawayMessage = true;
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.HidePager();
            messageWindowController.ShowWindow();
            messageWindowController.GenerateCannotRunawayMessage();
        }

        /// <summary>
        /// メッセージウィンドウでメッセージの表示が完了した時のコールバックです。
        /// </summary>
        public void OnFinishedShowMessage()
        {
            if (_handleRunnawayMessage)
            {
                _handleRunnawayMessage = false;
                if (!CanRunaway)
                {
                    SimpleLogger.Instance.Log("逃げられない戦闘のため、コマンド入力フェーズに戻ります。");
                    StartInputCommandPhase();
                    return;
                }
            }

            switch (BattlePhase)
            {
                case BattlePhase.ShowEnemy:
                    SimpleLogger.Instance.Log("敵の表示が完了しました。");
                    StartInputCommandPhase();
                    break;
                case BattlePhase.Action:
                    _battleActionProcessor.ShowNextMessage();
                    break;
                case BattlePhase.Result:
                    _battleResultManager.ShowNextMessage();
                    break;
            }
        }

        /// <summary>
        /// ターン内の行動が完了した時のコールバックです。
        /// </summary>
        public void OnFinishedActions()
        {
            if (IsBattleFinished)
            {
                SimpleLogger.Instance.Log("OnFinishedActions() || 戦闘が終了しているため、処理を中断します。");
                return;
            }

            SimpleLogger.Instance.Log("ターン内の行動が完了しました。");
            TurnCount++;
            StartInputCommandPhase();
        }

        /// <summary>
        /// コマンド選択が完了した後の処理です。
        /// </summary>
        void PostCommandSelect()
        {
            SimpleLogger.Instance.Log("敵のコマンド入力を行います。");
            _enemyCommandSelector.SelectEnemyCommand();
        }

        /// <summary>
        /// 敵キャラクターのコマンドが選択された時のコールバックです。
        /// </summary>
        public void OnEnemyCommandSelected()
        {
            StartAction();
        }

        /// <summary>
        /// 各キャラクターの行動を開始します。
        /// </summary>
        void StartAction()
        {
            SimpleLogger.Instance.Log("選択したアクションを実行します。");
            BattlePhase = BattlePhase.Action;
            var messageWindowController = _battleWindowManager.GetMessageWindowController();
            messageWindowController.ShowWindow();
            _battleActionProcessor.SetPriorities();
            _battleActionProcessor.StartActions();
        }

        /// <summary>
        /// ステータスの値が更新された時のコールバックです。
        /// </summary>
        public void OnUpdateStatus()
        {
            _battleWindowManager.GetStatusWindowController().UpdateAllCharacterStatus();
        }

        /// <summary>
        /// 敵を全て倒した時のコールバックです。
        /// </summary>
        public void OnEnemyDefeated()
        {
            SimpleLogger.Instance.Log("敵を全て倒しました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
            _battleResultManager.OnWin();
        }

        /// <summary>
        /// ゲームオーバーになった時のコールバックです。
        /// </summary>
        public void OnGameover()
        {
            SimpleLogger.Instance.Log("ゲームオーバーになりました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
            _battleResultManager.OnLose();
        }

        /// <summary>
        /// 味方が逃走に成功した時のコールバックです。
        /// </summary>
        public void OnRunaway()
        {
            StartCoroutine(RunawayProcess());
        }

        /// <summary>
        /// 味方が逃走に成功した時の処理です。
        /// </summary>
        IEnumerator RunawayProcess()
        {
            SimpleLogger.Instance.Log("逃走に成功しました。");
            IsBattleFinished = true;

            // 戦闘BGMを停止します。
            float fadeTime = 0.1f;
            AudioManager.Instance.StopAllBgm(fadeTime);
            yield return new WaitForSeconds(fadeTime);

            OnFinishBattle();
        }

        /// <summary>
        /// 敵が逃走に成功した時のコールバックです。
        /// </summary>
        public void OnEnemyRunaway()
        {
            SimpleLogger.Instance.Log("敵が逃走に成功しました。");
            BattlePhase = BattlePhase.Result;
            IsBattleFinished = true;
            _battleResultManager.OnWin();
        }

        /// <summary>
        /// 戦闘を終了する時のコールバックです。
        /// </summary>
        public void OnFinishBattle()
        {
            SimpleLogger.Instance.Log("戦闘に勝利して終了します。");

            _battleWindowManager.HideAllWindow();
            _battleSpriteController.HideBackground();
            _battleSpriteController.HideEnemy();
            _enemyStatusManager.InitializeEnemyStatusList();
            _battleActionProcessor.InitializeActions();
            _battleActionProcessor.StopActions();

            BattlePhase = BattlePhase.NotInBattle;
            _postBattle.OnFinishedBattle();
        }

        /// <summary>
        /// 戦闘を終了する時のコールバックです。
        /// </summary>
        public void OnFinishBattleWithGameover()
        {
            SimpleLogger.Instance.Log("ゲームオーバーとして戦闘を終了します。");
            _battleWindowManager.HideAllWindow();
            _battleSpriteController.HideBackground();
            _battleSpriteController.HideEnemy();
            _enemyStatusManager.InitializeEnemyStatusList();
            _battleActionProcessor.InitializeActions();
            _battleActionProcessor.StopActions();

            BattlePhase = BattlePhase.NotInBattle;
            _postBattle.OnLostBattle();
        }
    }
}