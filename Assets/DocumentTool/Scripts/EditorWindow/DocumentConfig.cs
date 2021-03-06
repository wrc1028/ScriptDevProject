#if UNITY_EDITOR
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

namespace TopJoy.Document
{
    public static class GetDocumentConfig
    {
        public static string DocumentConfigPath = "Assets/DocumentTool/Scripts/EditorWindow/DocumentConfig/DocumentConfig.asset";
        private static DocumentConfig config;
        public static bool IsEditMode
        {
            get
            {
                if (config == null) config = SearchDocumentConfig();
                return config.isEditMode;
            }
        }

        public static string DocumentRootPath
        {
            get
            {
                if (config == null) config = SearchDocumentConfig();
                return config.documentRootPath;
            }
        }

        public static string DocumentCollectPath
        {
            get
            {
                if (config == null) config = SearchDocumentConfig();
                return config.documentCollectPath;
            }
        }

        public static string RecycleBinPath
        {
            get
            {
                if (config == null) config = SearchDocumentConfig();
                return config.recycleBinPath;
            }
        }

        public static string IconsPath
        {
            get
            {
                if (config == null) config = SearchDocumentConfig();
                return config.iconsPath;
            }
        }

        private static GUIStyle titleStyle;
        public static GUIStyle TitleStyle
        {
            get
            {
                if (titleStyle == null)
                {
                    if (config == null) config = SearchDocumentConfig();
                    titleStyle = new GUIStyle();
                    titleStyle.fontSize = config.titleSize;
                    titleStyle.normal.textColor = config.titleColor;
                }
                return titleStyle;
            }
        }

        private static GUIStyle contentStyle;
        public static GUIStyle ContentStyle
        {
            get
            {
                if (contentStyle == null)
                {
                    if (config == null) config = SearchDocumentConfig();
                    contentStyle = new GUIStyle();
                    contentStyle.fontSize = config.contentSize;
                    contentStyle.normal.textColor = config.contentColor;
                }
                return contentStyle;
            }
        }

        public static DocumentConfig SearchDocumentConfig()
        {
            // TODO: ????????????????????????
            return AssetDatabase.LoadAssetAtPath<DocumentConfig>(DocumentConfigPath);
        }
    }
    [CreateAssetMenu(fileName = "DocumentConfig", menuName = "??????/????????????")]
    public class DocumentConfig : ScriptableObject
    {
        // ????????????
        [TitleGroup("????????????")]
        [LabelText("????????????")]
        [LabelWidth(80)]
        public bool isEditMode = false;
        [TitleGroup("????????????")]
        [LabelText("????????????")]
        [FolderPath(AbsolutePath = false)]
        [LabelWidth(80)]
        public string iconsPath;
        
        [TitleGroup("????????????")]
        [LabelText("??????????????????")]
        [FolderPath(AbsolutePath = false)]
        [LabelWidth(80)]
        public string documentRootPath;
        
        [TitleGroup("????????????")]
        [LabelText("??????????????????")]
        [FolderPath(AbsolutePath = false)]
        [LabelWidth(80)]
        public string documentCollectPath;

        [TitleGroup("????????????")]
        [LabelText("???????????????")]
        [FolderPath(AbsolutePath = false)]
        [LabelWidth(80)]
        public string recycleBinPath;
        // ------------------------------------------------
        [TitleGroup("??????????????????")]
        [HorizontalGroup("??????????????????/title", LabelWidth = 60)]
        [LabelText("????????????")]
        [Range(8, 28)]
        public int titleSize = 18;
        [HorizontalGroup("??????????????????/title", LabelWidth = 60)]
        [LabelText("????????????")]
        public Color titleColor = Color.green;
        [HorizontalGroup("??????????????????/content", LabelWidth = 60)]
        [LabelText("????????????")]
        [Range(2, 22)]
        public int contentSize = 12;
        [HorizontalGroup("??????????????????/content", LabelWidth = 60)]
        [LabelText("????????????")]
        public Color contentColor = Color.white;
    }
}
#endif