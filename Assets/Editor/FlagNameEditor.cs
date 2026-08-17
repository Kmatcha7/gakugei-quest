using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace SimpleRpg
{
    /// <Summary>
    /// 定義されたフラグ名をリスト化するエディタ拡張です。
    /// </Summary>
    [CustomEditor(typeof(FlagNameData))]
    public class FlagNameEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var data = target as FlagNameData;

            // ボタンを押されたら処理を開始します。
            if (GUILayout.Button("フラグ名データリストの作成"))
            {
                SetFlagNameList(data);
            }

            GUILayout.Space(10);

            // 通常のインスペクターを描画します。
            DrawDefaultInspector();
        }

        /// <Summary>
        /// 定義クラスからフラグ名の一覧をセットするメソッドです。
        /// </Summary>
        /// <param name="flagNameData">フラグ名データを保持するクラスのインスタンス</param>
        void SetFlagNameList(FlagNameData flagNameData)
        {
            // フラグ名のリストを初期化します。
            flagNameData.flagNames = new List<string>();

            // FlagNamesクラスからフラグ名を取得し、リストに追加します。
            var fieldInfoArray = typeof(FlagNames).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo field in fieldInfoArray)
            {
                if (field.IsLiteral)
                {
                    var fieldValue = field.GetRawConstantValue();
                    flagNameData.flagNames.Add(fieldValue.ToString());
                }
            }

            // データを保存します。
            EditorUtility.SetDirty(flagNameData);
            AssetDatabase.SaveAssets();
        }
    }
}