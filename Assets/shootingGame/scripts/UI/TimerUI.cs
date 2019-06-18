using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    public Text passedTime, remainingTime, shootNum;


    public void Open()
    {
        passedTime.gameObject.SetActive(true);
        remainingTime.gameObject.SetActive(true);
        shootNum.gameObject.SetActive(true);
    }
    public void Close()
    {
        passedTime.gameObject.SetActive(false);
        remainingTime.gameObject.SetActive(false);
        shootNum.gameObject.SetActive(false);
    }
}
