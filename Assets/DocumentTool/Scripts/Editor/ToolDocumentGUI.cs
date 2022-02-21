#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace TopJoy.Document
{
    [CustomEditor(typeof(ToolDocument))]
    public class ToolDocumentGUI : DocumentBaseGUI
    {
        private ToolDocument toolDocument;

        private new void OnEnable()
        {
            toolDocument = (ToolDocument)target;
            UpdateGUIStyle();
        }

        public override void OnInspectorGUI()
        {
            if (toolDocument.isEditing)
            {
                base.DrawDefaultInspector();
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("", GUILayout.Width(10));    // 左间距
                    GUILayout.BeginVertical();
                    {
                        GUILayout.Space(10);  // 上间距
                        for (int i = 0; i < toolDocument.paragraphs.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(toolDocument.paragraphs[i].title))
                            {
                                GUILayout.Label(toolDocument.paragraphs[i].title, titleStyle);
                                GUILayout.Space(10);  // 标题与内容的间距
                            }
                            DrawContent(toolDocument.paragraphs[i].sentences, contentStyle, numStyle);
                            if (i == toolDocument.paragraphs.Count - 1) continue;
                            GUILayout.Space(8);
                            GUILayout.Label("", lineStyle, GUILayout.Height(2));  // 段落与段落的间距和线段
                            GUILayout.Space(8);
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.Label("", GUILayout.Width(10));    // 右间距
                }
                GUILayout.EndHorizontal();
            }
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif