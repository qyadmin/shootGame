using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemEditorUI : MonoBehaviour
{
    public Toggle CanThought, ProhibitShooting, InvalidItem;
    public Text ItemName;
    public Dropdown LinkList;
    public Image LinkItem;

    private void Start()
    {
        this.gameObject.SetActive(false);
        CanThought.gameObject.SetActive(false);
        ProhibitShooting.gameObject.SetActive(false);
        InvalidItem.gameObject.SetActive(false);
    }
}
