using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
// 网站Cookie
/*'__wpkreporterwid_=57175135-8f44-4360-aedb-7e69c7ad8f0c;
   lang=zh-cn; 
   UM_distinctid=17e09f8dfb0121-0c8b5ea4b78774-4c607a68-240000-17e09f8dfb1125e; 
   yuque_ctoken=eIE5FHfmLlDoynz3rYWvVMmQ; 
   _TRACERT_COOKIE__SESSION=7ea3d2d1-c3dd-4d2a-be24-33e61c7a3eee; 
   CNZZDATA1272061571=946899971-1625111365-%7C1640916245; 
   __wpkreporterwid_=3e60913d-42b8-4437-82c1-df79a31e5096; 
   tree=a385%01ea326c49-8949-4c59-a3ee-00f7aac5e5ce%0125'*/
public class CaptureContentFromWeb : MonoBehaviour
{
    [LabelText("网页地址")]
    public string webPath = "";
    [Button("尝试抓取")]
    public void Capture()
    {
        WebRequest request = WebRequest.Create(webPath);
        WebResponse response = request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        // CookieContainer cc = new CookieContainer();
        // cc.Add(new Cookie("__wpkreporterwid_", "57175135-8f44-4360-aedb-7e69c7ad8f0c"));
        // cc.Add(new Cookie("lang", "zh-cn"));
        // cc.Add(new Cookie("UM_distinctid", "17e09f8dfb0121-0c8b5ea4b78774-4c607a68-240000-17e09f8dfb1125e"));
        // cc.Add(new Cookie("yuque_ctoken", "eIE5FHfmLlDoynz3rYWvVMmQ"));
        // cc.Add(new Cookie("_TRACERT_COOKIE__SESSION", "7ea3d2d1-c3dd-4d2a-be24-33e61c7a3eee"));
        // cc.Add(new Cookie("CNZZDATA1272061571", "946899971-1625111365-%7C1640916245"));
        // cc.Add(new Cookie("__wpkreporterwid_", "3e60913d-42b8-4437-82c1-df79a31e5096"));
        // cc.Add(new Cookie("tree", "a385%01ea326c49-8949-4c59-a3ee-00f7aac5e5ce%0125"));
        // string content = SendDataByGET(webPath, "" ,ref cc);

        FileStream assetBackup = File.Open("Assets/webComtent.txt", FileMode.CreateNew, FileAccess.ReadWrite);
        StreamWriter sw = new StreamWriter(assetBackup, Encoding.UTF8);
        sw.Write(reader.ReadToEnd());
        sw.Close();
        assetBackup.Close();
        AssetDatabase.Refresh();
    }
    public string SendDataByGET(string Url, string postDataStr, ref CookieContainer cookie)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
        if (cookie.Count == 0)
        {
            request.CookieContainer = new CookieContainer();
            cookie = request.CookieContainer;
        }
        else
        {
            request.CookieContainer = cookie;
        }

        request.Method = "GET";
        request.ContentType = "text/html;charset=UTF-8";

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream myResponseStream = response.GetResponseStream();
        StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
        string retString = myStreamReader.ReadToEnd();
        myStreamReader.Close();
        myResponseStream.Close();

        return retString;
    }

}
