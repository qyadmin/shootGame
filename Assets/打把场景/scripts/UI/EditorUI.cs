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
}
