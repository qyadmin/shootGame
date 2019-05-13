using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour
{
    public static EditorUI _Instance;

    public RuntimeToolUI runtimeToolUI;

    public AreaListUI areaListUI;

    public ItemListUI itemListUI;

    public AreaItemListUI areaItemListUI;
    private void Awake()
    {
        _Instance = this;
    }

    public void editorModle()
    {
        runtimeToolUI.gameObject.SetActive(true);
        areaListUI.gameObject.SetActive(true);
        itemListUI.gameObject.SetActive(true);
        areaItemListUI.gameObject.SetActive(true);
    }

    public void playingModle()
    {
        runtimeToolUI.gameObject.SetActive(true);
        areaListUI.gameObject.SetActive(false);
        itemListUI.gameObject.SetActive(false);
        areaItemListUI.gameObject.SetActive(false);
    }
}
