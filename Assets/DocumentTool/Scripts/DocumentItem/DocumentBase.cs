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
    [System.Flags]
    public enum Department 
    {
        角色 = 1 << 0, 动作 = 1 << 1, 场景 = 1 << 2,
        特效 = 1 << 3, 原画 = 1 << 4, TA = 1 << 5, 
        UI = 1 << 6, 视听 = 1 << 7, 其他 = 1 << 8,
        全体 = 角色 | 动作 | 场景 | 特效 | 原画 | TA | UI | 视听 | 其他, 
    }
    public enum ContentFormat { 普通, 复制, 图片, 启动, }
    public enum TitleFormat { 一级, 二级, 三级, 顶格, }

    #region 结构体
    // 单条内容的排版(格式)
    [Serializable]
    public struct Sentence
    {
        [HorizontalGroup("layer01", Width = 46)]
        [HideLabel]
        public ContentFormat contentFormat;

        [HorizontalGroup("layer01", Width = 46)]
        [HideLabel]
        public TitleFormat titleFormat;

        [HorizontalGroup("layer01")]
        [HideLabel]
        public string content;

        [ShowIf("contentFormat", ContentFormat.复制)]
        [HorizontalGroup("layer02", LabelWidth = 96)]
        [LabelText("复制内容")]
        public string copyContent;

        [ShowIf("contentFormat", ContentFormat.图片)]
        [Sirenix.OdinInspector.FilePath(AbsolutePath = false)]
        [HorizontalGroup("layer03", LabelWidth = 96)]
        [LabelText("图片地址")]
        public string texPath;

        [ShowIf("contentFormat", ContentFormat.启动)]
        [HorizontalGroup("layer04", LabelWidth = 96)]
        [LabelText("文件(或网页)地址")]
        public string fileUrl;
    }
    
    // 段落的排版(格式)
    [Serializable]
    public class Paragraph
    {
        [LabelText("标题"), GUIColor(0.5f, 1.1f, 0.5f)]
        [LabelWidth(34)]
        public string title;
        
        [LabelText("内容")]
        public List<Sentence> sentences;
    }

    
    #endregion

    public abstract class DocumentBase : ScriptableObject
    {
        [HideInInspector]
        public bool isCollect = false;
        [HideInInspector]
        public bool isEditing = false;

        // [HideInInspector]
        [LabelText("保存路径")]
        [LabelWidth(58)]
        public string documentSavePath = string.Empty;

        // [HideInInspector]
        [LabelText("目录结构")]
        [LabelWidth(58)]
        public string documentContentsPage = string.Empty;
        
        [HorizontalGroup("layer01", LabelWidth = 58, Order = 0)]
        [LabelText("文档名称")]
        public string documentName = string.Empty;

        [HorizontalGroup("layer01", Width = 122, LabelWidth = 58, Order = 0)]
        [HideLabel]
        public Department departments = Department.角色;

        [HorizontalGroup("url", Order = 1)]
        [LabelText("网页地址")]
        [LabelWidth(58)]
        public string documentUrl;
        [HorizontalGroup("url", Width = 122, Order = 1), GUIColor(0.7f, 0.7f, 1.5f)]
        [Button("尝试访问")]
        public void TryAccess()
        {
            if (!string.IsNullOrEmpty(documentUrl))
                Application.OpenURL(documentUrl);
            else
                UnityEditor.EditorUtility.DisplayDialog("无网站", "未设置访问网站!", "确定");
        }

        [HorizontalGroup("paragraph", Order = 5)]
        [LabelText("段落编辑")]
        public List<Paragraph> paragraphs = new List<Paragraph>();
    }
}
#endif