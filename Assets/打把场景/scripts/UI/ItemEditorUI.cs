using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEditorUI : MonoBehaviour
{
    public Toggle CanThought, ProhibitShooting, InvalidItem;
    public Text ItemName;


    private void Start()
    {
        this.gameObject.SetActive(false);
    }
}
