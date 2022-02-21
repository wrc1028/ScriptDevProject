#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix;
using Sirenix.OdinInspector;
namespace TopJoy.Document
{
    [Serializable]
    public struct Catalogue
    {
        [HorizontalGroup(Width = 200, LabelWidth = 28)]
        [LabelText("名字")]
        public string name;
        
        [HorizontalGroup(LabelWidth = 28)]
        [LabelText("地址")]
        public string path;

        // 0为文件夹，1为文件
        // [HideInInspector]
        [HorizontalGroup(Width = 20, LabelWidth = 28)]
        [HideLabel]
        public int type;
    }
    // 显示当前节点下的结构
    public class NodeDocument : DocumentBase
    {
        // 存储子集的第一层文件夹和文件，以及它们的排序，共用一个顺序
        // 如果没有节点文档的，按照原本位置排序，如果有就按照指定顺序排序，既可以显示全部文档，又可以单独设置
        [HorizontalGroup("Catalogue", Order = 4)]
        [LabelText("目录")]
        public List<Catalogue> catalogues = new List<Catalogue>();

        // 获得同级目录下的文件夹和文件
        private List<Catalogue> GetCatalogues(string savePath)
        {
            if (string.IsNullOrEmpty(savePath)) return null;
            string fileDirectory = Path.GetDirectoryName(savePath);
            List<Catalogue> tempCatalogues = new List<Catalogue>();
            string[] folderPaths = Directory.GetDirectories(fileDirectory);
            foreach (var folderPath in folderPaths)
            {
                tempCatalogues.Add(new Catalogue() { name = Path.GetFileName(folderPath), path = folderPath, type = 0 });
            }
            string[] filePaths = Directory.GetFiles(fileDirectory, "*.asset");
            foreach (var filePath in filePaths)
            {
                if (AssetDatabase.LoadAssetAtPath<DocumentBase>(filePath) == null || 
                    string.Compare(Path.GetFileNameWithoutExtension(filePath), documentName) == 0) continue;
                tempCatalogues.Add(new Catalogue() { name = Path.GetFileNameWithoutExtension(filePath), path = filePath, type = 1 });
            }
            return tempCatalogues;
        }

        // [HorizontalGroup("UpdateCatalogue", Order = 3)]
        // [Button("更新目录")]
        public void UpdateCatalogue()
        {
            if (catalogues.Count == 0)
                catalogues = GetCatalogues(documentSavePath);
            else
            {
                // 检查是否有添加新的文件夹或者文档
                if (string.IsNullOrEmpty(documentSavePath)) documentSavePath = AssetDatabase.GetAssetPath(this);
                string fileDirectory = Path.GetDirectoryName(documentSavePath);
                string[] folderPaths = Directory.GetDirectories(fileDirectory);
                string[] filePaths = Directory.GetFiles(fileDirectory, "*.asset");
                List<Catalogue> tempCatalogues = new List<Catalogue>();
                foreach (var catalogue in catalogues)
                {
                    if (catalogue.type == 0)
                    {
                        for (int i = 0; i < folderPaths.Length; i++)
                        {
                            if (string.Compare(catalogue.path, folderPaths[i]) != 0) continue;
                            folderPaths[i] = string.Empty;
                            tempCatalogues.Add(new Catalogue() { name = catalogue.name, path = catalogue.path, type = 0 });
                            break;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < filePaths.Length; i++)
                        {
                            if (string.Compare(catalogue.path, filePaths[i]) != 0) continue;
                            filePaths[i] = string.Empty;
                            tempCatalogues.Add(new Catalogue() { name = catalogue.name, path = catalogue.path, type = 1 });
                            break;
                        }
                    }
                }
                foreach (var folderPath in folderPaths)
                {
                    if (string.IsNullOrEmpty(folderPath)) continue;
                    tempCatalogues.Add(new Catalogue() { name = Path.GetFileName(folderPath), path = folderPath, type = 0 });
                }
                foreach (var filePath in filePaths)
                {
                    if (string.IsNullOrEmpty(filePath) || string.Compare(Path.GetFileNameWithoutExtension(filePath), documentName) == 0) continue;
                    tempCatalogues.Add(new Catalogue() { name = Path.GetFileNameWithoutExtension(filePath), path = filePath, type = 1 });
                }
                catalogues.Clear();
                catalogues = tempCatalogues;
            }
        }
    }
}
#endif