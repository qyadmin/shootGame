// ==================================================================
// 作    者：Pablo.风暴洋-宋杨
// 説明する：Xml操作类
// 作成時間：2019-05-24
// 類を作る：Xml_ShootingItem.cs
// 版    本：v 1.0
// 会    社：大连仟源科技
// QQと微信：731483140
// ==================================================================

using System.Xml;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Xml.Serialization;
using System.Collections;

public class Xml_ShootingItem : MonoBehaviour
{
    /// <summary>
    /// 文件名
    /// </summary>
    private static string ShootingItem = "/ShootingItem";
    /// <summary>
    /// 文件路径
    /// </summary>

    public static string path =
#if UNITY_ANDROID   //安卓  
    "jar:file://" + Application.dataPath + "!/assets/";  
#elif UNITY_IPHONE  //iPhone  
    Application.dataPath + "/Raw/";  
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR  //windows平台和web平台  
    Application.dataPath + ShootingItem + ".xml";
#else
        string.Empty;  
#endif
    public static bool existXml = false;

    public static void OnStart()
    {
        if (File.Exists(path))
            existXml = true;
        else
            existXml = false;
    }


    /// <summary>
    /// 创建Xml
    /// </summary>
    public static void CreateXml()
    {
        if (!File.Exists(path))
        {
            XmlDocument xml = new XmlDocument();
            //创建最上一层的节点
            XmlElement root = xml.CreateElement("ShootingItem");

            xml.AppendChild(root);
            xml.Save(path);
        }
    }

    /// <summary>
    /// 增加Xml数据
    /// </summary>
    /// <param name="sItem"></param>
    /// <param name="id"></param>
    public static void AddXmlData(ShootingArea sa, int id)
    {
        if (File.Exists(path))
        {

            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNode root = xml.SelectSingleNode("ShootingItem");

            XmlElement element = xml.CreateElement("data");

            //二进制序列化
            StringWriter sw = new StringWriter();
            XmlSerializer serlizer = new XmlSerializer(typeof(GetDate));
            serlizer.Serialize(sw, sa.getDates());//进行序列化,参数一  输出流,参数二 表示序列化的对象
            sw.Close();//完成后关闭这个流
            string itemnameListString = sw.ToString();
            Debug.Log(itemnameListString);


            //设置节点的属性
            element.SetAttribute("id", id.ToString());
            XmlElement xelData = xml.CreateElement("info");
            xelData.InnerText = itemnameListString;
            root.AppendChild(element);
            element.AppendChild(xelData);
            xml.AppendChild(root);
            //最后保存文件
            xml.Save(path);
        }
    }


    public static void addSceneid(string sceneName)
    {
        if (File.Exists(path))
        {

            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlNode root = xml.SelectSingleNode("ShootingItem");

            XmlElement xelSenceName = xml.CreateElement("scene");
            xelSenceName.InnerText = sceneName;

            root.AppendChild(xelSenceName);
            xml.AppendChild(root);
            //最后保存文件
            xml.Save(path);
        }
    }

    /// <summary>
    /// 删除单个条目数据
    /// </summary>
    public static void DeleteSingleXmlData(string index)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        XmlElement xe = doc.DocumentElement;
        string strPath = string.Format("/ShootingItem/data[@id=\"{0}\"]", index); //Xpath表达式
        XmlElement selectXe = (XmlElement)xe.SelectSingleNode(strPath);
        selectXe.ParentNode.RemoveChild(selectXe);
        doc.Save(path);
    }

    /// <summary>
    /// 获取所有的Xml数据
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<XElement> getAllXmlData()
    {
        XElement xe = XElement.Load(path);
        IEnumerable<XElement> elements = from ele in xe.Elements("data") select ele;
        return elements;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    public static void DeleteXmlByPath()
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// 读取
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ArrayList GetXmlData(string id)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        XmlElement xe = doc.DocumentElement;
        string strPath = string.Format("/ShootingItem/data[@id=\"{0}\"]", id); //Xpath表达式
        XmlElement selectXe = (XmlElement)xe.SelectSingleNode(strPath);
        XmlNodeList xmlNodeList = selectXe.ChildNodes;
        ArrayList al = new ArrayList();
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            al.Add(xmlNodeList.Item(i).InnerText);
            Debug.Log("xmlNodeList.Item(i).InnerText" + xmlNodeList.Item(i).InnerText);
        }
        return al;
    }

    public static string GetSceneName()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(path);
        XmlElement xe = doc.DocumentElement;
        XmlElement selectXe = (XmlElement)xe.SelectSingleNode("/ShootingItem/scene");
        XmlNodeList xmlNodeList = selectXe.ChildNodes;
        return xmlNodeList.Item(0).InnerText;
    }
}
