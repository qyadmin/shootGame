using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class IPManager
{
    //单例模式  
    private static IPManager _instance;

    public static IPManager GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new IPManager();
            }

            return _instance;
        }
    }

    private const string iptext = "IP";

    //选择自已需要的本地语言  
    public string Ip = iptext;

    private Dictionary<string, string> dic = new Dictionary<string, string>();
    /// <summary>  
    /// 读取配置文件，将文件信息保存到字典里  
    /// </summary>  
    public void GetMasseage()
    {
        dic.Clear();
        string[] strs = File.ReadAllLines(Application.dataPath + "/"+Ip + ".txt");

        //string[] lines = text.Split('\n');
        foreach (string line in strs)
        {
            if (line == null)
            {
                continue;
            }
            string[] keyAndValue = line.Split('=');
            dic.Add(keyAndValue[0], keyAndValue[1].Split('\r')[0]);
        }
        
    }

    public string GetValue(string key)
    {
        if (dic.ContainsKey(key) == false)
        {
            return null;
        }
        string value = null;
        dic.TryGetValue(key, out value);
        return value;
    } 

    public string UTF8String(string input)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        return utf8.GetString(utf8.GetBytes(input));
    }
} 