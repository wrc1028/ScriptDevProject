#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace TopJoy.Document
{
    public class ToolDocument : DocumentBase
    {
        [HorizontalGroup("tool", Order = 2)]
        [LabelText("工具名称")]
        [LabelWidth(58)]
        public string toolName;
        [HorizontalGroup("tool", Width = 122, Order = 2), GUIColor(0.7f, 1.5f, 0.7f)]
        [Button("尝试打开")]
        public void TryOpen()
        {
            if (!string.IsNullOrEmpty(toolName))
                EditorApplication.ExecuteMenuItem(toolName);
            else
                UnityEditor.EditorUtility.DisplayDialog("错误", "请填入工具名称!", "确定");
        }
    }
}
#endif