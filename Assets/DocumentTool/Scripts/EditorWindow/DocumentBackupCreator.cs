#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TopJoy.Document
{
    public static class DocumentBackupCreator
    {
        public static void CreateAssetBackup<T>(T asset, string savePath) where T : DocumentBase
        {
            if (File.Exists(savePath)) File.Delete(savePath);
            FileStream assetBackup = File.Open(savePath, FileMode.CreateNew, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(assetBackup, Encoding.UTF8);
            sw.WriteLine("<DocumentName>" + asset.documentName + "</DocumentName>");
            sw.WriteLine("<DocumentUrl>" + asset.documentUrl + "</DocumentUrl>");
            sw.WriteLine("<DocumentSavePath>" + asset.documentSavePath + "</DocumentSavePath>");
            // 写入工具名称
            if (asset.GetType() == typeof(ToolDocument)) WriteToolDocumentInfo(ref sw, asset as ToolDocument);
            foreach (var paragraph in asset.paragraphs)
            {
                sw.WriteLine("\n<Title>" + paragraph.title + "</Title>");
                foreach (var sentence in paragraph.sentences)
                {
                    sw.WriteLine("<ContentFormat>" + (int)sentence.contentFormat + "</ContentFormat>" + 
                        "<TitleFormat>" + (int)sentence.titleFormat + "</TitleFormat>" + 
                        "<Content>" + sentence.content + "</Content>" + 
                        "<Copy>" + sentence.copyContent + "</Copy>" + 
                        "<TexPath>" + sentence.texPath + "</TexPath>" + 
                        "<FilePath>" + sentence.fileUrl + "</FilePath>");
                }
            }
            sw.Close();
            assetBackup.Close();
        }
        // 给工具文档添加工具名
        private static void WriteToolDocumentInfo(ref StreamWriter sw, ToolDocument asset)
        {
            if (asset == null) return;
            sw.WriteLine("<ToolName>" + (asset as ToolDocument).toolName + "</ToolName>");
        }

        // 由资源路径转换成备份路径
        public static string TransformAssetPathToBackupPath(string documentPath)
        {
            string assetName = Path.GetFileNameWithoutExtension(documentPath);
            return Regex.Replace(documentPath, string.Format("{0}.asset", assetName), string.Format(".{0}.txt", assetName));
        }
        // 由备份路径转换成资源路径
        public static string TransformBackupPathToAssetPath(string backupPath)
        {
            string documentName = Path.GetFileNameWithoutExtension(backupPath);
            return Regex.Replace(backupPath, string.Format("{0}.asset", documentName), string.Format(".{0}.txt", documentName));
        }
        // 移动文档到回收站功能
        public static void TransformBackupToRecycleBin(string documentPath)
        {
            string backupPath = TransformAssetPathToBackupPath(documentPath);
            if (File.Exists(backupPath))
            {    
                string deleteTime = DateTime.Now.ToString("yyyy-MM-dd hh mm ss");
                string redundantFileName = Path.GetFileNameWithoutExtension(backupPath).Substring(1);
                string recyclePath = GetDocumentConfig.RecycleBinPath + "/" + redundantFileName + deleteTime + ".txt";
                File.Move(backupPath, recyclePath);
                Debug.Log(string.Format("已经将多余备份文件\"{0}\"移动到回收站!", redundantFileName));
            }
            else Debug.Log("不存在备份文件!");
        }
    }
}
#endif