using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// 選択肢ウィンドウの動作を制御するクラスです。
    /// </summary>
    public class OptionWindowController : MonoBehaviour
    {
        /// <summary>
        /// 選択肢ウィンドウのUIを制御するクラスへの参照です。
        /// </summary>
        [SerializeField]
        OptionUIController _uiController;

        /// <summary>
        /// 選択結果を通知する先です。
        /// </summary>
        IOptionCallback _callback;

        /// <summary>
        /// 選択肢を選べるかどうかのフラグです。
        /// </summary>
        bool _canSelect;

        /// <summary>
        /// 選択された選択肢のインデックスです。
        /// </summary>
        protected int _selectedIndex;

        /// <summary>
        /// コントローラの状態をセットアップします。
        /// </summary>
        /// <param name="callback">コールバック先</param>
        /// <param name="initialSelection">初期選択インデックス</param>
        public void SetUpController(IOptionCallback callback, int initialSelection = 0)
        {
            _callback = callback;
            _selectedIndex = initialSelection;
            PostSelection();
        }

        void Update()
        {
            CheckKeyInput();
        }

        /// <summary>
        /// キー入力を確認します。
        /// </summary>
        void CheckKeyInput()
        {
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
            int newIndex = _selectedIndex - 1;
            if (newIndex < 0)
            {
                newIndex = _uiController.OptionCount - 1;
            }
            _selectedIndex = newIndex;
            PostSelection();
        }

        /// <summary>
        /// ひとつ下の項目を選択します。
        /// </summary>
        void SelectLowerItem()
        {
            int newIndex = _selectedIndex + 1;
            if (newIndex >= _uiController.OptionCount)
            {
                newIndex = 0;
            }
            _selectedIndex = newIndex;
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
            _uiController.ShowCursor(_selectedIndex);
        }

        /// <summary>
        /// 決定ボタンが押された時の処理です。
        /// </summary>
        protected virtual void OnPressedConfirmButton()
        {
            if (_callback != null)
            {
                _callback.OnSelectedOption(_selectedIndex);
            }
            StartCoroutine(HideProcess());

            // 選択時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.OK);
        }

        /// <summary>
        /// キャンセルボタンが押された時の処理です。
        /// </summary>
        protected virtual void OnPressedCancelButton()
        {
            // キャンセルされた場合は、区別できるように-1を渡します。
            int canceledIndex = -1;
            if (_callback != null)
            {
                _callback.OnSelectedOption(canceledIndex);
            }
            StartCoroutine(HideProcess());

            // キャンセル時の効果音を再生します。
            AudioManager.Instance.PlaySe(SeNames.Cancel);
        }

        /// <summary>
        /// 選択ウィンドウを非表示にする処理です。
        /// </summary>
        protected IEnumerator HideProcess()
        {
            _canSelect = false;
            yield return null;
            HideWindow();
        }

        /// <summary>
        /// 選択ウィンドウを表示します。
        /// </summary>
        public void ShowWindow()
        {
            _uiController.Show();
            StartCoroutine(SetSelectStateDelay());
        }

        /// <summary>
        /// 選択ウィンドウを表示した後、選択可能になるまでの遅延を設けます。
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
