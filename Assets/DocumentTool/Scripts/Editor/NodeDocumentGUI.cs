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
    [CustomEditor(typeof(NodeDocument))]
    public class NodeDocumentGUI : DocumentBaseGUI
    {
        private Texture fileIcon;
        private Texture folderIcon;
        private GUIStyle fileStyle;
        private GUIStyle folderStyle;
        private NodeDocument nodeDocument;
        private new void OnEnable()
        {
            nodeDocument = (NodeDocument)target;
            fileIcon = GetTextureByIconName("FileIcon");
            folderIcon = GetTextureByIconName("FolderIcon");
            fileStyle = new GUIStyle();
            fileStyle.alignment = TextAnchor.MiddleLeft;
            fileStyle.fontSize = 16;
            fileStyle.normal.textColor = new Color(1, 1, 1, 1);
            folderStyle = new GUIStyle();
            folderStyle.alignment = TextAnchor.MiddleLeft;
            folderStyle.fontSize = 16;
            folderStyle.normal.textColor = new Color(0.4f, 1, 0.6f, 1);
            UpdateGUIStyle();
        }
        private Texture GetTextureByIconName(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Texture>(GetDocumentConfig.IconsPath + "/" + name + ".png");
        }

        public override void OnInspectorGUI()
        {
            if (nodeDocument.isEditing)
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
                        for (int i = 0; i < nodeDocument.paragraphs.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(nodeDocument.paragraphs[i].title))
                            {
                                GUILayout.Label(nodeDocument.paragraphs[i].title, titleStyle);
                                GUILayout.Space(10);  // 标题与内容的间距
                            }
                            DrawContent(nodeDocument.paragraphs[i].sentences, contentStyle, numStyle);
                            if (i == nodeDocument.paragraphs.Count - 1) continue;
                            GUILayout.Space(8);
                            GUILayout.Label("", lineStyle, GUILayout.Height(2));  // 段落与段落的间距和线段
                            GUILayout.Space(8);
                        }
                        // int subNum = Regex.Matches(nodeDocument.documentSavePath, "/").Count;
                        // DrawParentCatalogue(nodeDocument.catalogues, ref subNum);
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
        
        // 绘制当前层级
        private void DrawParentCatalogue(List<Catalogue> catalogues, ref int subNum)
        {
            foreach (var catalogue in catalogues)
            {
                if (catalogue.type == 0)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Empty, GUILayout.Width(16 * (Regex.Matches(catalogue.path, @"\\").Count - subNum)));
                    GUILayout.Label(catalogue.name, folderStyle);
                    GUILayout.EndHorizontal();
                    DrawChildCatalogue(catalogue.path, ref subNum);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Empty, GUILayout.Width(16 * (Regex.Matches(catalogue.path, @"\\").Count - subNum)));
                    GUILayout.Label(catalogue.name, fileStyle);
                    GUILayout.EndHorizontal();
                }
            }
        }
        // 绘制子目录
        private void DrawChildCatalogue(string path, ref int subNum)
        {
            string[] filePaths = Directory.GetFiles(path, "*.asset");
            string[] folderPaths = Directory.GetDirectories(path);
            NodeDocument currentDocument = null;
            // 优先查找节点文档(优先第一个)
            foreach (var filePath in filePaths)
            {
                if (AssetDatabase.LoadAssetAtPath<DocumentBase>(filePath) == null || 
                    AssetDatabase.LoadAssetAtPath<DocumentBase>(filePath).GetType() != typeof(NodeDocument)) continue;
                currentDocument = AssetDatabase.LoadAssetAtPath<NodeDocument>(filePath) as NodeDocument;
                break;
            }
            // 如果存在目录，使用目录上的排序
            if (currentDocument != null) DrawParentCatalogue(currentDocument.catalogues, ref subNum);
            // 否则使用Unity的排序
            else
            {
                foreach (var folderPath in folderPaths)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Empty, GUILayout.Width(16 * (Regex.Matches(folderPath, @"\\").Count - subNum)));
                    GUILayout.Label(Path.GetFileName(folderPath), folderStyle);
                    GUILayout.EndHorizontal();
                    DrawChildCatalogue(folderPath, ref subNum);
                }
                foreach (var filePath in filePaths)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(String.Empty, GUILayout.Width(16 * (Regex.Matches(filePath, @"\\").Count - subNum)));
                    GUILayout.Label(Path.GetFileNameWithoutExtension(filePath), fileStyle);
                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}
#endif