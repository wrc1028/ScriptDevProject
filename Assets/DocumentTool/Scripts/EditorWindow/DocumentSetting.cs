#if UNITY_EDITOR
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace TopJoy.Document
{
    [CreateAssetMenu(fileName = "DocumentSetting", menuName = "文档/文档设置")]
    public class DocumentSetting : ScriptableObject
    {
        // ------------------------------------------------
        [TitleGroup("字体设置")]
        [HorizontalGroup("字体设置/title", LabelWidth = 60)]
        [LabelText("标题大小")]
        [Range(8, 28)]
        public int titleSize = 18;
        [HorizontalGroup("字体设置/title", LabelWidth = 60)]
        [LabelText("标题颜色")]
        public Color titleColor = Color.green;
        [HorizontalGroup("字体设置/content", LabelWidth = 60)]
        [LabelText("正文大小")]
        [Range(2, 22)]
        public int contentSize = 12;
        [HorizontalGroup("字体设置/content", LabelWidth = 60)]
        [LabelText("正文颜色")]
        public Color contentColor = Color.white;
    }
}
#endif