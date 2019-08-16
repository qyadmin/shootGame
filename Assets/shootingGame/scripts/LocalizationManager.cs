using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class LocalizationManager
{
    //单例模式  
    private static LocalizationManager _instance;

    public static LocalizationManager GetInstance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LocalizationManager();
            }

            return _instance;
        }
    }

    private const string chinese = "Chinese";
    private const string english = "English";

    //选择自已需要的本地语言  
    public string language = chinese;


    public void setlanguage(string language)
    {
        Localization(language);
    }

    private Dictionary<string, string> dic = new Dictionary<string, string>();
    /// <summary>  
    /// 读取配置文件，将文件信息保存到字典里  
    /// </summary>  
    public void Localization(string language)
    {
        TextAsset ta = Resources.Load<TextAsset>(language);
        string text = ta.text;

        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (line == null)
            {
                continue;
            }
            string[] keyAndValue = line.Split('=');
            dic.Add(keyAndValue[0], keyAndValue[1].Split('\r')[0]);
        }
        
    }

    /// <summary>  
    /// 获取value  
    /// </summary>  
    /// <param name="key"></param>  
    /// <returns></returns>  
    public void SetValue()
    {
        SetTextValue();
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

    void SetTextValue()
    {
        GameObject gamemanager = GameObject.Find("GameController");
        foreach (var key in dic)
        {
            try
            {
                if (gamemanager.transform.Find(key.Key))
                {
                    if(gamemanager.transform.Find(key.Key).GetComponent<Text>())
                        gamemanager.transform.Find(key.Key).GetComponent<Text>().text = key.Value;
                    if(gamemanager.transform.Find(key.Key).GetComponent<Image>())
                    {                       
                        gamemanager.transform.Find(key.Key).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Image/" + key.Value);
                    }
                }
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }

    public string UTF8String(string input)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        return utf8.GetString(utf8.GetBytes(input));
    }
} 