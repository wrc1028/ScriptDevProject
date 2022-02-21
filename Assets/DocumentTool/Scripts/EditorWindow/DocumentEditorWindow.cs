#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace TopJoy.Document
{
    public class DocumentEditorWindow : OdinMenuEditorWindow
    {
        private GUIContent createNewDocumentContent;
        private GUIContent editDocumentContent;
        private GUIContent moveDocumentContent;
        private GUIContent deleteDocumentContent;
        private GUIContent saveDocumentContent;
        private GUIContent collectDocumentContent;
        private GUIContent webDocumentContent;
        private GUIContent toolContent;
        private static OdinMenuEditorWindow window;
        private string originDocumentPath;
        private string collectDocumentPath;
        [MenuItem("Tools/文档")]
        public static void ShowWindow()
        {
            window = (DocumentEditorWindow)OdinMenuEditorWindow.GetWindow<DocumentEditorWindow>("文档", false);
            window.minSize = new Vector2(800, 540);
            window.Show();
        }

        private new void OnEnable()
        {
            createNewDocumentContent = new GUIContent(GetTextureByIconName("CreateNewDocument"), "创建新文档");
            editDocumentContent = new GUIContent(GetTextureByIconName("EditDocument"), "编辑文档");
            moveDocumentContent = new GUIContent(GetTextureByIconName("MoveDocument"), "移动文档");
            deleteDocumentContent = new GUIContent(GetTextureByIconName("DeleteDocument"), "删除文档");
            saveDocumentContent = new GUIContent(GetTextureByIconName("SaveDocument"), "保存文档");
            collectDocumentContent = new GUIContent(GetTextureByIconName("CollectDocument"), "收藏文档");
            webDocumentContent = new GUIContent(GetTextureByIconName("WebDocument"), "访问网页端文档");
            toolContent = new GUIContent(GetTextureByIconName("Tool"), "打开工具");
        }
        private Texture GetTextureByIconName(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Texture>(GetDocumentConfig.IconsPath + "/" + name + ".png");
        }
        protected override OdinMenuTree BuildMenuTree()
        { 
            OdinMenuTree tree = new OdinMenuTree();
            tree.Config.DrawSearchToolbar = true;
            tree.Config.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
            tree.AddAllAssetsAtPath("收藏", GetDocumentConfig.DocumentCollectPath, typeof(DocumentBase));
            UpdeDocumentTree(ref tree, GetDocumentConfig.DocumentRootPath);
            return tree;
        }
        // 遍历更新目录
        private void UpdeDocumentTree(ref OdinMenuTree tree, string rootPath)
        {
            string[] filePaths = Directory.GetFiles(rootPath, "*.asset");
            string[] folderPaths = Directory.GetDirectories(rootPath);
            NodeDocument nodeDocument = null;
            // 优先查找节点文档(优先第一个)
            foreach (var filePath in filePaths)
            {
                if (AssetDatabase.LoadAssetAtPath<DocumentBase>(filePath) == null || 
                    AssetDatabase.LoadAssetAtPath<DocumentBase>(filePath).GetType() != typeof(NodeDocument)) continue;
                nodeDocument = AssetDatabase.LoadAssetAtPath<NodeDocument>(filePath) as NodeDocument;
                break;
            }
            // 如果存在目录，使用目录上的排序
            if (nodeDocument != null)
            {
                nodeDocument.UpdateCatalogue();
                tree.Add(nodeDocument.documentContentsPage, nodeDocument);
                foreach (var catalogue in nodeDocument.catalogues)
                {
                    if (catalogue.type == 0) UpdeDocumentTree(ref tree, catalogue.path);
                    else
                    {
                        if (AssetDatabase.LoadAssetAtPath<DocumentBase>(catalogue.path) == null) continue;
                        DocumentBase documentAsset = AssetDatabase.LoadAssetAtPath<DocumentBase>(catalogue.path) as DocumentBase;
                        tree.Add(documentAsset.documentContentsPage + "/" + documentAsset.documentName, documentAsset);
                    }
                }
            }
            // 否则使用Unity的排序
            else
            {
                foreach (var folderPath in folderPaths) UpdeDocumentTree(ref tree, folderPath);
                foreach (var filePath in filePaths)
                {
                    if (AssetDatabase.LoadAssetAtPath<DocumentBase>(filePath) == null) continue;
                    DocumentBase documentAsset = AssetDatabase.LoadAssetAtPath<DocumentBase>(filePath) as DocumentBase;
                    tree.Add(documentAsset.documentContentsPage + "/" + documentAsset.documentName, documentAsset);
                }
            }
        }
        
        // 功能按键
        protected override void OnBeginDrawEditors()
        {
            var selected = this.MenuTree.Selection.FirstOrDefault();
            var toolbarHeight = this.MenuTree.Config.SearchToolbarHeight;
            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null && selected.Value != null)
                {
                    GUILayout.Label(selected.Name);
                    if (selected.Value.GetType().BaseType == typeof(DocumentBase))
                    {
                        DocumentBase documentBase = selected.Value as DocumentBase;

                        if (GetDocumentConfig.IsEditMode && documentBase.isEditing)
                        {
                            if (SirenixEditorGUI.ToolbarButton(saveDocumentContent))
                            {
                                // TODO: 每次增加文档类型就要在这加一条，也许会有更好的方法
                                documentBase.isEditing = false;
                                if (string.IsNullOrEmpty(documentBase.documentName))
                                    documentBase.documentName = documentBase.name;
                                if (string.IsNullOrEmpty(documentBase.documentSavePath))
                                    documentBase.documentSavePath = AssetDatabase.GetAssetPath(documentBase);
                                if (string.IsNullOrEmpty(documentBase.documentContentsPage))
                                    documentBase.documentContentsPage = Regex.Match(documentBase.documentSavePath, string.Format("{0}/(.+)/{1}.asset", GetDocumentConfig.DocumentRootPath, documentBase.documentName)).Groups[1].Value;
                                // 保留备份
                                string backupSavePath = DocumentBackupCreator.TransformAssetPathToBackupPath(documentBase.documentSavePath);
                                if (selected.Value.GetType() == typeof(BlankDocument))
                                    DocumentBackupCreator.CreateAssetBackup<BlankDocument>(selected.Value as BlankDocument, backupSavePath);
                                else if (selected.Value.GetType() == typeof(ToolDocument))
                                    DocumentBackupCreator.CreateAssetBackup<ToolDocument>(selected.Value as ToolDocument, backupSavePath);
                                else if (selected.Value.GetType() == typeof(NodeDocument))
                                    DocumentBackupCreator.CreateAssetBackup<NodeDocument>(selected.Value as NodeDocument, backupSavePath);
                                else
                                    DocumentBackupCreator.CreateAssetBackup<DocumentBase>(selected.Value as DocumentBase, backupSavePath);
                            }
                        }
                        else if (GetDocumentConfig.IsEditMode)
                        {
                            if (selected.Value.GetType() == typeof(NodeDocument) && SirenixEditorGUI.ToolbarButton(createNewDocumentContent))
                            {
                                DocumentCreator.ShowDialog<DocumentBase>(Path.GetDirectoryName(documentBase.documentSavePath), obj =>
                                {
                                    // TODO: 这里进行文档内容中的命名、相对路径与绝对路径设置，以及文档的类型
                                    obj.isEditing = true;
                                    obj.documentName = obj.name;
                                    obj.documentSavePath = AssetDatabase.GetAssetPath(obj);
                                    obj.documentContentsPage = Regex.Match(obj.documentSavePath, string.Format("{0}/(.+)/{1}.asset", GetDocumentConfig.DocumentRootPath, obj.documentName)).Groups[1].Value;
                                    base.TrySelectMenuItemWithObject(obj);
                                });
                            }
                            if (SirenixEditorGUI.ToolbarButton(editDocumentContent))
                            {
                                documentBase.isEditing = true;
                            }
                            if (SirenixEditorGUI.ToolbarButton(moveDocumentContent))
                            {
                                string destPath = EditorUtility.SaveFilePanel("移动至", Path.GetDirectoryName(documentBase.documentSavePath), documentBase.documentName, "asset");
                                if (!string.IsNullOrEmpty(destPath))
                                {
                                    destPath = destPath.Substring(Application.dataPath.Length - 6);
                                    AssetDatabase.MoveAsset(documentBase.documentSavePath, destPath);
                                    documentBase.documentName = Path.GetFileNameWithoutExtension(destPath);
                                    documentBase.documentSavePath = destPath;
                                    documentBase.documentContentsPage = Regex.Match(destPath, string.Format("{0}/(.+)/{1}.asset", GetDocumentConfig.DocumentRootPath, documentBase.documentName)).Groups[1].Value;
                                    base.TrySelectMenuItemWithObject(documentBase);
                                }
                            }
                            if (SirenixEditorGUI.ToolbarButton(deleteDocumentContent))
                            {
                                if (UnityEditor.EditorUtility.DisplayDialog("删除", string.Format("是否删除:{0}?", selected.Name), "确定", "取消"))
                                {
                                    // 删除当前文件，将备份文件移动到回收站
                                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(documentBase));
                                    DocumentBackupCreator.TransformBackupToRecycleBin(documentBase.documentSavePath);
                                    AssetDatabase.Refresh();
                                }
                            }
                        }
                        if (!documentBase.isEditing)
                        {
                            // 所有文档都有的功能
                            if (selected.Value.GetType() == typeof(ToolDocument))
                            {
                                if (SirenixEditorGUI.ToolbarButton(toolContent))
                                {
                                    if (!string.IsNullOrEmpty((documentBase as ToolDocument).toolName)) EditorApplication.ExecuteMenuItem((documentBase as ToolDocument).toolName);
                                    else UnityEditor.EditorUtility.DisplayDialog("警告", "当前文档未指定工具名!", "确定");
                                }
                            }
                            if (SirenixEditorGUI.ToolbarButton(webDocumentContent))
                            {
                                if (!string.IsNullOrEmpty(documentBase.documentUrl)) Application.OpenURL(documentBase.documentUrl);
                                else UnityEditor.EditorUtility.DisplayDialog("警告", "当前文档未指定地址!", "确定");
                            }
                            documentBase.isCollect = SirenixEditorGUI.ToolbarToggle(documentBase.isCollect, collectDocumentContent);
                            collectDocumentPath = GetDocumentConfig.DocumentCollectPath + "/" + documentBase.documentName + ".asset";
                            if (documentBase.isCollect && !File.Exists(collectDocumentPath) && File.Exists(documentBase.documentSavePath))
                            {
                                AssetDatabase.CopyAsset(documentBase.documentSavePath, collectDocumentPath);
                                DocumentBase collectDocument = AssetDatabase.LoadAssetAtPath<DocumentBase>(collectDocumentPath);
                                collectDocument.isCollect = true;
                            }
                            else if (!documentBase.isCollect && File.Exists(collectDocumentPath))
                            {
                                AssetDatabase.DeleteAsset(collectDocumentPath);
                                DocumentBase originDocument = AssetDatabase.LoadAssetAtPath<DocumentBase>(documentBase.documentSavePath);
                                originDocument.isCollect = false;
                            }
                        }
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}
#endif