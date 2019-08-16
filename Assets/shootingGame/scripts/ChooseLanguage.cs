using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseLanguage : MonoBehaviour
{
    [SerializeField]
    Button continueclick;
    Text buttontext;

    bool isfinish = false;
    bool Isfinish
    {
        get { return isfinish; }
        set {
            if (value)
                language = !language;

            isfinish = value;
        }
    }

    bool language = false;
    string chinese;
   
    string english;
   
    // Start is called before the first frame update
    void Start()
    {
        buttontext = continueclick.transform.GetChild(0).GetComponent<Text>();
        if (Xml_ShootingItem._Instance.existXml)
        {
            chinese = "选择语言和版本后点此继续。";
            english = "After selecting the language and version,click here to continue.";
        }
        else
        {
            chinese = "选择语言后点此继续。";
            english = "After selecting the language,click here to continue.";
        }
    }

    // Update is called once per frame
    float a = 1;
    void Update()
    {
        if (buttontext.color.a > 0.05f && !isfinish)
        {
            a = a - Time.deltaTime;

        }
        else
            if(!isfinish)
            Isfinish = true;

        if (buttontext.color.a < 0.95f && isfinish)
        {
            a = a + Time.deltaTime;

        }
        else
        {
            if(isfinish)
                Isfinish = false;          
        }
            

        if(language)
            buttontext.text = english;
        else
            buttontext.text = chinese;

        buttontext.color = new Color(1, 1, 1,a);
    }
}
