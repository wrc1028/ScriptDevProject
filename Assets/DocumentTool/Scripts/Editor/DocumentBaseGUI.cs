#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

namespace TopJoy.Document
{
    
    public class DocumentBaseGUI : OdinEditor
    {
        
        protected GUIStyle titleStyle;
        protected GUIStyle contentStyle;
        protected GUIStyle numStyle;
        protected GUIStyle lineStyle;
        // 更新字体
        protected virtual void UpdateGUIStyle()
        {
            // base.DrawDefaultInspector();
            // 标题风格
            titleStyle = GetDocumentConfig.TitleStyle;
            // 内容风格
            contentStyle = GetDocumentConfig.ContentStyle;
            contentStyle.wordWrap = true;
            // 编号的风格
            numStyle = new GUIStyle();
            numStyle.fontSize = contentStyle.fontSize;
            numStyle.normal.textColor = contentStyle.normal.textColor;
            numStyle.alignment = TextAnchor.MiddleRight;
            // 线条的风格
            lineStyle = new GUIStyle();
            lineStyle.normal.background = Texture2D.grayTexture;
        }
        // 绘制内容
        protected void DrawContent(List<Sentence> sentences, GUIStyle contentStyle, GUIStyle numStyle)
        {
            int numInt = 1, charInt = 97, numStrWidth = contentStyle.fontSize;
            string numStr = String.Empty;
            for (int i = 0; i < sentences.Count; i++)
            {
                // 确认编号
                switch (sentences[i].titleFormat)
                {
                    case TitleFormat.一级: 
                        if (numInt < 9) numStrWidth = contentStyle.fontSize;
                        else numStrWidth = contentStyle.fontSize * 2;
                        numStr = numInt.ToString() + ". ";
                        numInt ++;
                        break;
                    case TitleFormat.二级:
                        if (sentences[i].titleFormat > sentences[i == 0 ? i : i - 1].titleFormat) charInt = 97;
                        numStr = ((char)charInt).ToString() + ". ";
                        numStrWidth = contentStyle.fontSize * 2;
                        charInt ++;
                        break;
                    case TitleFormat.三级: 
                        numStr = "· ";
                        numStrWidth = contentStyle.fontSize * 3;
                        break;
                    default:
                        numStr = String.Empty;
                        numStrWidth = 0;
                        break;
                }
                // 绘制内容
                switch (sentences[i].contentFormat)
                {
                    case ContentFormat.普通:
                        DrawNormalContent(numStr, numStrWidth, numStyle, sentences[i].content, contentStyle);
                        break;
                    case ContentFormat.复制:
                        DrawCopyContent(numStr, numStrWidth, numStyle, sentences[i].content, contentStyle, sentences[i].copyContent);
                        break;
                    case ContentFormat.图片:
                        DrawTexContent(numStr, numStrWidth, numStyle, sentences[i].content, contentStyle, sentences[i].texPath);
                        break;
                    default:
                        DrawOpenContent(numStr, numStrWidth, numStyle, sentences[i].content, contentStyle, sentences[i].fileUrl);
                        break;
                }
            }
        }
        // 绘制普通内容
        private void DrawNormalContent(string numStr, int numStrWidth, GUIStyle numStyle, string content, GUIStyle contentStyle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(numStr, numStyle, GUILayout.Width(numStrWidth));
            GUILayout.Label(content, contentStyle);
            GUILayout.EndHorizontal();
        }
        // 绘制可复制
        private void DrawCopyContent(string numStr, int numStrWidth, GUIStyle numStyle, string content, GUIStyle contentStyle, string copyContent)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(numStr, numStyle, GUILayout.Width(numStrWidth));
            GUILayout.Label(content, contentStyle);
            if (GUILayout.Button("复制", GUILayout.Width(40)))
            {
                if (!string.IsNullOrEmpty(copyContent))
                {
                    UnityEngine.GUIUtility.systemCopyBuffer = copyContent;
                    UnityEditor.EditorUtility.DisplayDialog("成功", "已复制内容:" + copyContent, "确定");
                }
                else
                    UnityEditor.EditorUtility.DisplayDialog("失败", "无内容可复制!", "确定");
            }
            GUILayout.EndHorizontal();
        }
        // 绘制图片
        private void DrawTexContent(string numStr, int numStrWidth, GUIStyle numStyle, string content, GUIStyle contentStyle, string texPath)
        {
            GUILayout.BeginHorizontal();
            if (!(string.IsNullOrEmpty(numStr) && string.IsNullOrEmpty(content)))
            {
                GUILayout.Label(numStr, numStyle, GUILayout.Width(numStrWidth));
                GUILayout.Label(content, contentStyle);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            float texIndentation = (numStrWidth - contentStyle.fontSize) <= 0 ? 0 : numStrWidth - contentStyle.fontSize;
            GUILayout.Label(string.Empty, numStyle, GUILayout.Width(texIndentation));
            if (File.Exists(texPath) && AssetDatabase.LoadAssetAtPath<Texture>(texPath) != null)
            {
                Texture targetTex = AssetDatabase.LoadAssetAtPath<Texture>(texPath);
                GUILayout.Label(targetTex);
            }
            else
            {
                // 加载当前路径下没有图片的提示图片
                GUIStyle warningStyle = new GUIStyle();
                warningStyle.fontSize = contentStyle.fontSize;
                warningStyle.normal.textColor = Color.yellow;
                GUILayout.Label(string.Format("目标路径:{0} 下无图片", texPath), warningStyle);
            }
            GUILayout.EndHorizontal();
        }
        // 绘制启动
        private void DrawOpenContent(string numStr, int numStrWidth, GUIStyle numStyle, string content, GUIStyle contentStyle, string fileUrl)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(numStr, numStyle, GUILayout.Width(numStrWidth));
            GUILayout.Label(content, contentStyle);
            if (GUILayout.Button("打开", GUILayout.Width(40)))
            {
                if (!string.IsNullOrEmpty(fileUrl))
                {
                    if (Regex.IsMatch(fileUrl, @"https://")) Application.OpenURL(fileUrl);
                    else
                    {
                        fileUrl.Replace("/", "\\");
                        System.Diagnostics.Process.Start("explorer.exe", fileUrl);
                    }
                }
                else
                    UnityEditor.EditorUtility.DisplayDialog("失败", "打开失败，路径或网站为空!", "确定");
            }
            GUILayout.EndHorizontal();
        }
    }
}
#endif