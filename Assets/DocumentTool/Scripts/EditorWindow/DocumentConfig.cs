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
            // TODO: 遍历资源进行查找
            return AssetDatabase.LoadAssetAtPath<DocumentConfig>(DocumentConfigPath);
        }
    }
    [CreateAssetMenu(fileName = "DocumentConfig", menuName = "文档/文档配置")]
    public class DocumentConfig : ScriptableObject
    {
        // 开启编辑
        [TitleGroup("基础设置")]
        [LabelText("开启编辑")]
        [LabelWidth(80)]
        public bool isEditMode = false;
        [TitleGroup("路径设置")]
        [LabelText("图标路径")]
        [FolderPath(AbsolutePath = false)]
        [LabelWidth(80)]
        public string iconsPath;
        
        [TitleGroup("路径设置")]
        [LabelText("文档保存路径")]
        [FolderPath(AbsolutePath = false)]
        [LabelWidth(80)]
        public string documentRootPath;
        
        [TitleGroup("路径设置")]
        [LabelText("文档收藏路径")]
        [FolderPath(AbsolutePath = false)]
        [LabelWidth(80)]
        public string documentCollectPath;

        [TitleGroup("路径设置")]
        [LabelText("回收站路径")]
        [FolderPath(AbsolutePath = false)]
        [LabelWidth(80)]
        public string recycleBinPath;
        // ------------------------------------------------
        [TitleGroup("文档字体设置")]
        [HorizontalGroup("文档字体设置/title", LabelWidth = 60)]
        [LabelText("标题大小")]
        [Range(8, 28)]
        public int titleSize = 18;
        [HorizontalGroup("文档字体设置/title", LabelWidth = 60)]
        [LabelText("标题颜色")]
        public Color titleColor = Color.green;
        [HorizontalGroup("文档字体设置/content", LabelWidth = 60)]
        [LabelText("正文大小")]
        [Range(2, 22)]
        public int contentSize = 12;
        [HorizontalGroup("文档字体设置/content", LabelWidth = 60)]
        [LabelText("正文颜色")]
        public Color contentColor = Color.white;
    }
}
#endif