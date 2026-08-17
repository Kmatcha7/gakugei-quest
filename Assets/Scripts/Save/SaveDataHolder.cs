using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace SimpleRpg
{
    /// <summary>
    /// セーブデータを保持するクラスです。
    /// </summary>
    public static class SaveDataHolder
    {
        /// <summary>
        /// 読み込んだセーブファイルのキャッシュです。
        /// </summary>
        static SaveFile _saveFile;

        /// <summary>
        /// 読み込んだセーブファイルのキャッシュです。
        /// </summary>
        public static SaveFile SaveFile
        {
            get { return _saveFile; }
        }

        /// <summary>
        /// セーブファイルを初期化します。
        /// </summary>
        public static void InitializeSaveFile()
        {
            _saveFile = new()
            {
                saveSlots = new List<SaveSlot>()
            };
        }

        /// <summary>
        /// ファイルをディスクからロードします。
        /// </summary>
        public static void Load()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            // WebGL環境ではPlayerPrefsからデータをロードします。
            LoadFromPlayerPrefs();
            return;
#endif
            // セーブファイルのパスを取得し、存在しない場合はフィールドを初期化します。
            string path = SaveDataUtil.GetSaveFilePath();
            if (!File.Exists(path))
            {
                SimpleLogger.Instance.Log($"指定したパスにデータが存在しないため、セーブファイルを初期化します。 path : {path}");
                InitializeSaveFile();
                return;
            }

            try
            {
                string loadedText = File.ReadAllText(path);
                _saveFile = JsonUtility.FromJson<SaveFile>(loadedText);
                SimpleLogger.Instance.Log($"loadedText : {loadedText}");
            }
            catch (Exception e)
            {
                SimpleLogger.Instance.LogError($"ロードに失敗しました。 エラー内容 : {e}");
                InitializeSaveFile();
            }
        }

        /// <summary>
        /// ゲーム内の実行中データをセーブファイルに保存します。
        /// </summary>
        public static void Save()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            // WebGL環境ではPlayerPrefsにデータを保存します。
            SaveToPlayerPrefs();
            return;
#endif
            var json = JsonUtility.ToJson(_saveFile);
            var path = SaveDataUtil.GetSaveFilePath();
            try
            {
                File.WriteAllText(path, json);
                SimpleLogger.Instance.Log($"セーブしたjson : {json}");
            }
            catch (Exception e)
            {
                SimpleLogger.Instance.LogError($"セーブに失敗しました。エラー内容 : {e}");
                return;
            }
        }

        /// <summary>
        /// ファイルをPlayerPrefsからロードします。
        /// </summary>
        static void LoadFromPlayerPrefs()
        {
            try
            {
                string json = PlayerPrefs.GetString(SaveSettings.WebGLSaveKey, "{}");
                _saveFile = JsonUtility.FromJson<SaveFile>(json);
                SimpleLogger.Instance.Log($"loadedText : {json}");
            }
            catch (Exception e)
            {
                SimpleLogger.Instance.LogError($"ロードに失敗しました。 エラー内容 : {e}");
                InitializeSaveFile();
            }
        }

        /// <summary>
        /// ゲーム内の実行中データをPlayerPrefsに保存します。
        /// </summary>
        public static void SaveToPlayerPrefs()
        {
            var json = JsonUtility.ToJson(_saveFile);
            PlayerPrefs.SetString(SaveSettings.WebGLSaveKey, json);
            PlayerPrefs.Save();
        }
    }
}