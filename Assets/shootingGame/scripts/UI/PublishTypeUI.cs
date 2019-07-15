using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublishTypeUI : MonoBehaviour
{
    [SerializeField]
    public Button PC_Button, Android_Button, exitbutton;
    [SerializeField]
    public InputField EditorSenceName;
    private void Start()
    {
        exitbutton.onClick.AddListener(delegate () {
            Clear();
            this.gameObject.SetActive(false);

        });
    }
    public void Clear()
    {
        EditorSenceName.text = string.Empty;
    }
}
