using UnityEngine;
using System.Collections;

namespace SimpleRpg
{
    /// <summary>
    /// タイトル画面のつづきからのメニューを制御するクラスです。
    /// </summary>
    public class TitleContinueController : MonoBehaviour
    {
        /// <summary>
        /// タイトル画面のメニューを管理するクラスへの参照です。
        /// </summary>
        TitleMenuManager _titleMenuManager;

        /// <summary>
        /// メニューのセーブ画面のUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        MenuSaveUIController _uiController;

        /// <summary>
        /// セーブデータの管理を行うクラスへの参照です。
        /// </summary>
        [SerializeField]
        SaveDataManager _saveDataManager;

        /// <summary>
        /// 現在選択中のセーブ枠です。
        /// </summary>
        int _selectedSlot;

        /// <summary>
        /// セーブ枠の選択が可能かどうかのフラグです。
        /// </summary>
        bool _canSelect;

        /// <summary>
        /// セーブ枠が空の場合の名前です。
        /// </summary>
        readonly string EmptySlotName = "NO DATA";

        /// <summary>
        /// セーブ画面を開いた時の説明文です。
        /// </summary>
        readonly string LoadDescription = "ロードするファイルを選択してください。";

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        public void SetUpController(TitleMenuManager titleMenuManager)
        {
            _titleMenuManager = titleMenuManager;
        }

        /// <summary>
        /// セーブ枠の情報をセットします。
        /// </summary>
        public void SetUpSlotInfo()
        {
            // セーブ枠の情報をセットします。
            _uiController.ClearAllSlot();

            for (int i = 1; i <= SaveSettings.SlotNum; i++)
            {
                var saveSlot = _saveDataManager.GetSaveSlot(i);
                if (saveSlot == null)
                {
                    // セーブ枠が空の場合は、空欄の表示を行います。
                    _uiController.SetSlotInfoAsEmpty(i, EmptySlotName);
                    continue;
                }

                var statusInfo = saveSlot.saveInfoStatus;
                if (statusInfo == null)
                {
                    SimpleLogger.Instance.LogWarning($"セーブ枠のステータス情報が見つかりませんでした。 セーブ枠ID: {i}");
                    _uiController.SetSlotInfoAsEmpty(i, EmptySlotName);
                    continue;
                }

                int characterId = statusInfo.partyCharacter[0];
                string characterName = CharacterDataManager.GetCharacterName(characterId);

                int level = 1;
                var status = statusInfo.characterStatuses.Find(s => s.characterId == characterId);
                if (status != null)
                {
                    level = status.level;
                }

                int mapId = 0;
                if (saveSlot.saveInfoMap != null)
                {
                    mapId = saveSlot.saveInfoMap.mapId;
                }
                string place = MapDataManager.GetMapName(mapId);

                _uiController.SetSlotInfo(i, characterName, level, place);
            }
        }

        void Update()
        {
            CheckKeyInput();
        }

        /// <summary>
        /// キー入力を検知します。
        /// </summary>
        void CheckKeyInput()
        {
            if (_titleMenuManager == null)
            {
                return;
            }

            if (!_canSelect)
            {
                return;
            }

            if (InputGameKey.MenuUp())
            {
                SelectUpperItem();
            }
            else if (InputGameKey.MenuDown())
            {
                SelectLowerItem();
            }
            else if (InputGameKey.ConfirmButton())
            {
                OnPressedConfirmButton();
            }
            else if (InputGameKey.CancelButton())
            {
                OnPressedCancelButton();
            }
        }

        /// <summary>
        /// ひとつ上の項目を選択します。
        /// </summary>
        void SelectUpperItem()
        {
            int newIndex = _selectedSlot - 1;
            if (newIndex < 1)
            {
                newIndex = SaveSettings.SlotNum;
            }
            _selectedSlot = newIndex;
            PostSelection();
        }

        /// <summary>
        /// ひとつ下の項目を選択します。
        /// </summary>
        void SelectLowerItem()
        {
            int newIndex = _selectedSlot + 1;
            if (newIndex > SaveSettings.SlotNum)
            {
                newIndex = 1;
            }
            _selectedSlot = newIndex;
            PostSelection();
        }

        /// <summary>
        /// 選択後の処理を行います。
        /// </summary>
        void PostSelection()
        {
            ShowSelectionCursor();
        }

        /// <summary>
        /// 選択中の位置に応じたカーソルを表示します。
        /// </summary>
        void ShowSelectionCursor()
        {
            _uiController.ShowCursor(_selectedSlot);
        }

        /// <summary>
        /// 選択されたセーブ枠がロード可能かどうかを確認します。
        /// </summary>
        bool CanLoadSlot()
        {
            var saveSlot = _saveDataManager.GetSaveSlot(_selectedSlot);
            if (saveSlot == null)
            {
                return false;
            }

            var statusInfo = saveSlot.saveInfoStatus;
            if (statusInfo == null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 決定ボタンが押された時の処理です。
        /// </summary>
        void OnPressedConfirmButton()
        {
            // セーブ枠がロードできない場合は処理を抜けます。
            if (!CanLoadSlot())
            {
                return;
            }

            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);

            _canSelect = false;
            _titleMenuManager.OnSelectedSlotId(_selectedSlot);
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        void OnPressedCancelButton()
        {
            // キャンセル時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Cancel);
            StartCoroutine(HideProcess());
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        IEnumerator HideProcess()
        {
            _canSelect = false;
            yield return null;

            _titleMenuManager.OnLoadCanceled();
            HideWindow();
        }

        /// <summary>
        /// セーブ画面を表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
            _uiController.SetUpControllerList();
            _uiController.SetDescription(LoadDescription);
            _canSelect = false;

            _selectedSlot = 1;
            ShowSelectionCursor();
            SetUpSlotInfo();

            // セーブ画面を開いたフレームでボタンの検知をしないようにします。
            StartCoroutine(SetSelectStateDelay());
        }

        /// <summary>
        /// ステータス画面を閉じられるフラグをセットします。
        /// </summary>
        IEnumerator SetSelectStateDelay()
        {
            yield return null;
            _canSelect = true;
        }

        /// <summary>
        /// 選択ウィンドウを非表示にします。
        /// </summary>
        public void HideWindow()
        {
            _uiController.Hide();
        }
    }
}
