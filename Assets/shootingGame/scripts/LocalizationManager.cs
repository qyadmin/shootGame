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
    public string Language;
    class Fonts
    {
        public string values;
        public int fontsize;
    }

    public void setlanguage(string language)
    {
        Language = language;
        CleanDic();
        Localization();
    }

    private Dictionary<string, object> dic = new Dictionary<string, object>();
    /// <summary>  
    /// 读取配置文件，将文件信息保存到字典里  
    /// </summary>  
    public void Localization()
    {
        TextAsset ta = Resources.Load<TextAsset>(Language);
        string text = ta.text;

        string[] lines = text.Split('\n');
        foreach (string line in lines)
        {
            if (line == null)
            {
                continue;
            }
            string[] keyAndValue = line.Split('=');
            if (keyAndValue.Length > 2)
            {
                Fonts font = new Fonts();
                font.values = keyAndValue[1];
                Debug.Log(keyAndValue[1]);
                font.fontsize = int.Parse(keyAndValue[2].Split('\r')[0]);
                dic.Add(keyAndValue[0], font);
            }
                
            else
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
    void CleanDic()
    {
        dic.Clear();
    }

    public string GetValue(string key)
    {
        if (dic.ContainsKey(key) == false)
        {
            return null;
        }
        object value = null;
        dic.TryGetValue(key, out value);
        return value.ToString();
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
                    if (gamemanager.transform.Find(key.Key).GetComponent<Text>())
                    {
                        Fonts font = new Fonts();
                        if (key.Value.GetType() == typeof(Fonts))
                        {
                            font = (Fonts)key.Value;                         
                            gamemanager.transform.Find(key.Key).GetComponent<Text>().text = font.values;                           
                            gamemanager.transform.Find(key.Key).GetComponent<Text>().fontSize = font.fontsize;
                        }
                        else 
                            gamemanager.transform.Find(key.Key).GetComponent<Text>().text = key.Value.ToString();
                    }
                       
                    if(gamemanager.transform.Find(key.Key).GetComponent<Image>())
                    {                       
                        gamemanager.transform.Find(key.Key).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Image/" + key.Value.ToString());
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