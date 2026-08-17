using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SimpleRpg
{
    /// <summary>
    /// フラグを管理するクラスです。
    /// </summary>
    public class FlagManager : MonoBehaviour
    {
        /// <summary>
        /// フラグの一覧の定義データです。
        /// </summary>
        FlagNameData _flagNameData;

        /// <summary>
        /// フラグの状態を保持するリストです。
        /// </summary>
        List<FlagState> _flagStates = new();

        /// <summary>
        /// このクラスのインスタンスです。
        /// </summary>
        private static FlagManager _instance;

        /// <summary>
        /// このクラスのインスタンスです。
        /// </summary>
        public static FlagManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<FlagManager>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new();
                        _instance = singletonObject.AddComponent<FlagManager>();
                        singletonObject.name = typeof(FlagManager).ToString() + " (Singleton)";
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LoadFlagNames();
        }

        /// <summary>
        /// フラグ名のデータをロードします。
        /// </summary>
        public async void LoadFlagNames()
        {
            AsyncOperationHandle<IList<FlagNameData>> handle = Addressables.LoadAssetsAsync<FlagNameData>(AddressablesLabels.Flag, null);
            await handle.Task;
            _flagNameData = handle.Result[0];
        }

        /// <summary>
        /// フラグの状態を保持するリストを初期化します。
        /// </summary>
        public void InitializeFlagList()
        {
            _flagStates.Clear();
            foreach (var flagName in _flagNameData.flagNames)
            {
                var flagState = new FlagState
                {
                    flagName = flagName,
                    state = false
                };
                _flagStates.Add(flagState);
            }
        }

        /// <summary>
        /// 指定されたフラグ名の状態を取得します。
        /// </summary>
        /// <param name="flagName">フラグ名</param>
        public bool GetFlagState(string flagName)
        {
            bool state = false;
            var flagState = _flagStates.Find(fs => fs.flagName == flagName);
            if (flagState != null)
            {
                state = flagState.state;
            }
            else
            {
                SimpleLogger.Instance.LogWarning($"指定されたフラグ名の状態が見つかりませんでした。 flagName: {flagName}");
            }
            return state;
        }

        /// <summary>
        /// 指定されたフラグ名の状態をセットします。
        /// </summary>
        /// <param name="flagName">フラグ名</param>
        /// <param name="state">フラグの状態</param>
        public void SetFlagState(string flagName, bool state)
        {
            var flagState = _flagStates.Find(fs => fs.flagName == flagName);
            if (flagState != null)
            {
                flagState.state = state;
            }
            else
            {
                SimpleLogger.Instance.LogWarning($"指定されたフラグ名の状態が見つからなかったためリストに追加します。 flagName: {flagName}");
                flagState = new FlagState
                {
                    flagName = flagName,
                    state = state
                };
                _flagStates.Add(flagState);
            }
        }

        /// <summary>
        /// フラグの状態を保持するリストを取得します。
        /// </summary>
        public List<FlagState> GetFlagStateList()
        {
            return _flagStates;
        }
    }
}