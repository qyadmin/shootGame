using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum worningType
{
    msg,
    handle
}
public class WorningUI : MonoBehaviour
{
   

    public Text tital;
    public Text msg;
    public Button exitbutton;
    public Button cancel;
    public Button determine;
    private worningType type;

    public worningType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
            if (value == worningType.handle)
                handletype();
            else
                msgtype();
        }
    }
    private void Start()
    {
        exitbutton.onClick.AddListener(delegate() {
            this.gameObject.SetActive(false);
        });
    }

    void msgtype()
    {
        cancel.gameObject.SetActive(false);
        determine.gameObject.SetActive(false);
    }
    void handletype()
    {
        cancel.gameObject.SetActive(true);
        determine.gameObject.SetActive(true);
    }
}
