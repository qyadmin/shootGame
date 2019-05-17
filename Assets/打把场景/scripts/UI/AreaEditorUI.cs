using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreaEditorUI : MonoBehaviour
{
    [SerializeField]
    public InputField m_time;
    [SerializeField]
    public InputField m_shootNum;


    private void Start()
    {
        this.gameObject.SetActive(false);
    }
    public void disable()
    {
        m_time.interactable = false;
        m_shootNum.interactable = false;
    }

    public void enable()
    {
        m_time.interactable = true;
        m_shootNum.interactable = true;
    }
    public void setvalue(int areaTime,int areaShootNum)
    {
        m_time.text = areaTime.ToString();
        m_shootNum.text = areaShootNum.ToString();
    }

}
