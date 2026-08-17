using UnityEngine;

namespace SimpleRpg
{
    /// <summary>
    /// フラグに関する機能をテストするためのクラスです。
    /// </summary>
    public class FlagTester : MonoBehaviour
    {
        [Header("フラグの内容を出力")]
        /// <summary>
        /// フラグの状態を出力するフラグです。
        /// Inspectorウィンドウからチェックを入れると、コンソールに出力します。
        /// </summary>
        [SerializeField]
        bool _outputAllFlagStates;

        [Header("フラグの内容を個別に出力")]
        /// <summary>
        /// 出力するフラグ名です。
        /// </summary>
        [SerializeField]
        string _outputFlagName;

        /// <summary>
        /// フラグの状態を出力するフラグです。
        /// </summary>
        [SerializeField]
        bool _outputFlagState;

        [Header("フラグの状態を変更")]
        /// <summary>
        /// 変更する対象のフラグ名です。
        /// </summary>
        [SerializeField]
        string _flagName;

        /// <summary>
        /// 変更する状態です。
        /// </summary>
        [SerializeField]
        bool _flagState;

        /// <summary>
        /// フラグの状態を変更するフラグです。
        /// </summary>
        [SerializeField]
        bool _setFlagStates;

        void Update()
        {
            // 定義データのロードを待つため、最初の5フレームは処理を抜けます。
            if (Time.frameCount < 5)
            {
                return;
            }

            OutputAllFlags();
            OutputFlag();
            SetFlagState();
        }

        /// <summary>
        /// フラグの状態をコンソールに出力します。
        /// </summary>
        void OutputAllFlags()
        {
            if (!_outputAllFlagStates)
            {
                return;
            }

            _outputAllFlagStates = false;
            SimpleLogger.Instance.Log($"フラグの一覧を出力します。");

            var flagStates = FlagManager.Instance.GetFlagStateList();
            foreach (var flagState in flagStates)
            {
                SimpleLogger.Instance.Log($"フラグ名: <b>{flagState.flagName}</b>, 状態: <b>{flagState.state}</b>");
            }
        }

        /// <summary>
        /// 指定した名前のフラグの状態をコンソールに出力します。
        /// </summary>
        void OutputFlag()
        {
            if (!_outputFlagState)
            {
                return;
            }

            if (string.IsNullOrEmpty(_outputFlagName))
            {
                SimpleLogger.Instance.LogWarning("フラグ名が空です。フラグの状態を出力できません。");
                return;
            }

            _outputFlagState = false;
            var flagState = FlagManager.Instance.GetFlagState(_outputFlagName);
            SimpleLogger.Instance.Log($"フラグ名: <b>{_outputFlagName}</b>, 状態: <b>{flagState}</b>");
        }

        /// <summary>
        /// フラグの状態を変更します。
        /// </summary>
        void SetFlagState()
        {
            if (!_setFlagStates)
            {
                return;
            }

            _setFlagStates = false;
            if (string.IsNullOrEmpty(_flagName))
            {
                SimpleLogger.Instance.LogWarning("フラグ名が空です。フラグの状態を変更できません。");
                return;
            }

            FlagManager.Instance.SetFlagState(_flagName, _flagState);
            SimpleLogger.Instance.Log($"フラグの状態を変更しました。フラグ名: <b>{_flagName}</b>, 状態: <b>{_flagState}</b>");
        }
    }
}