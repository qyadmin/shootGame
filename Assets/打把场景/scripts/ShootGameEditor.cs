using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Battlehub.RTHandles.Demo;
using Battlehub.RTCommon;
using System.Linq;
using Battlehub.RTHandles;
using System.Text.RegularExpressions;

[DefaultExecutionOrder(-10)]
public class ShootGameEditor : SimpleEditor
{
    public static ShootGameEditor _Instance;

    private List<ShootingArea> m_Arealist = new List<ShootingArea>();

    [SerializeField]
    private GameObject AreaCount = null;

    bool Lock = false;


    public bool IsCreated
    {
        get { return true; }
    }

    private GameObject EditorArea;


    #region IRTEState implementation
    public event System.Action<object> Created;
    public event System.Action<object> Destroyed;
    private void Use()
    {
        Created(null);
        Destroyed(null);
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();
        _Instance = this;
    }

    protected override void Start()
    {
        base.Start();

        EditorUI._Instance.areaListUI.Add.onClick.AddListener(delegate () { Add_Arealist(); });
        EditorUI._Instance.areaListUI.Subtract.onClick.AddListener(delegate () { Subtract_Arealist(Editor.Selection.gameObjects); });
        EditorUI._Instance.runtimeToolUI.Move_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Move); });
        EditorUI._Instance.runtimeToolUI.Rotate_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Rotate); });
        EditorUI._Instance.runtimeToolUI.Scale_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Scale); });
        EditorUI._Instance.runtimeToolUI.Lock_Button.onClick.AddListener(delegate () { SetRuntimeTool(RuntimeTool.Lock); });
        //Editor.Selection.SelectionChanged += test;
        Editor.Selection.Selectioned += ToolsUIChange;
        Editor.Mask = (1 << 10);
        Editor.Tools.LockAxes = new LockObject
        {
            PositionY = true,
            ScaleY = true,
            RotationX = true,
            RotationZ = true
        };
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    //增加Area
    void Add_Arealist()
    {
        ShootingArea newArea = new ShootingArea();
        int num = 1;
        if (m_Arealist.Count != 0)
            m_Arealist.ForEach(item =>
            {
                if (item.m_Number == num)
                    ++num;
            });
        newArea.Instantiate_obj(Resources.Load<GameObject>("ShootingArea")
                                , EditorUI._Instance.areaListUI.AreaPerfabUI
                                , AreaCount, EditorUI._Instance.areaListUI.AreaUICount
                                , delegate () { UIClickEvent(newArea); }
                                , num);
        newArea.ItemList = new ShootingArea.ShootingItemList();
        newArea.ItemList.AddItem(Resources.Load<GameObject>("纸靶子"), Resources.Load<Sprite>("UI/Sprite/纸靶子"));
        newArea.ItemList.AddItem(Resources.Load<GameObject>("纸靶子1"), Resources.Load<Sprite>("UI/Sprite/纸靶子"));
        newArea.ItemList.AddItem(Resources.Load<GameObject>("纸靶子2"), Resources.Load<Sprite>("UI/Sprite/纸靶子"));


        m_Arealist.Add(newArea);
    }
    //增加Area的Item
    public void Add_AreaItemList(GameObject prefabInstance,int number)
    {        
        GetEditorArea().ItemList.m_ShootingItem.ForEach(item =>
        {
            if (item.Number == number)
            {
                ShootingItem newItem = new ShootingItem();
                newItem = item;
                newItem.Prefab = prefabInstance;
                GetEditorArea().m_ShootingItem.Add(newItem);
                Debug.Log(item.Name);
            }
        });
    }
    //获取锁定Area的GameObject类型
    GameObject[] GetEditorAreaObj()
    {
        GameObject[] selection = new GameObject[1];
        selection[0] = EditorArea;
        return selection;
    }
    void test(Object[] unselected)
    {
        if (unselected != null)
            for (int i = 0; i < unselected.Length; ++i)
            {
                GameObject unselectedObj = unselected[i] as GameObject;
                Debug.Log(unselectedObj.name);
            }
    }
    //监听选中物体的的UI变化
    void ToolsUIChange(Object[] selected)
    {
        bool Postion = true;
        bool Rotation = true;
        bool Scale = true;

        if (!Lock)
            CleanItemList();

        if (selected != null && selected.Length != 0)
        {
            GameObject[] m_selected = new GameObject[1];
            if (EditorArea != null)
                m_selected[0] = EditorArea;
            else
                m_selected[0] = selected[0] as GameObject;
            if (m_selected.Length == 1)
                m_Arealist.ForEach(item =>
                {
                    if (item.Perfab == m_selected[0])
                    {
                        item.ItemList.Instantiate_obj(EditorUI._Instance.itemListUI.Prefab, EditorUI._Instance.itemListUI.ItemListUICount.gameObject);
                        item.Instantiate_Item(EditorUI._Instance.areaItemListUI.AreaItemListPrefabUI, EditorUI._Instance.areaItemListUI.AreaItemListUICount,Editor);
                    }
                });


            for (int i = 0; i < selected.Length; ++i)
            {
                GameObject selectedObj = selected[i] as GameObject;
                ExposeToEditor ObjEditor = selectedObj.GetComponent<ExposeToEditor>();
                if (ObjEditor)
                {
                    foreach (DisabledToolsType item in ObjEditor.DisabledToolsType)
                    {
                        if (item == DisabledToolsType.Position && Postion)
                            Postion = false;
                        if (item == DisabledToolsType.Roation && Rotation)
                            Rotation = false;
                        if (item == DisabledToolsType.Scale && Scale)
                            Scale = false;
                    }
                }
            }
        }
        else
        {
            Postion = false;
            Rotation = false;
            Scale = false;
        }
        EditorUI._Instance.runtimeToolUI.Move_Button.interactable = Postion;
        EditorUI._Instance.runtimeToolUI.Rotate_Button.interactable = Rotation;
        EditorUI._Instance.runtimeToolUI.Scale_Button.interactable = Scale;
        Set_LockItemList(Lock);
    }


    //清除itemList和AreaItemList
    void CleanItemList()
    {
        for (int i = 0; i < EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.childCount; i++)
        {
            Destroy(EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.childCount; i++)
        {
            Destroy(EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.GetChild(i).gameObject);
        }
    }
    //设置锁定状态下itemList是否可拖动
    void Set_LockItemList(bool value)
    {
        for (int i = 0; i < EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.GetChild(i).GetComponent<PrefabSpawnPoint>().CanDrag = value;
        }
        for (int i = 0; i < EditorUI._Instance.areaListUI.AreaUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.areaListUI.AreaUICount.gameObject.transform.GetChild(i).GetComponent<Button>().interactable = !value;
        }
        for (int i = 0; i < EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.GetChild(i).GetComponent<Button>().interactable = value;
        }
        EditorUI._Instance.areaListUI.Add.interactable = !value;

        EditorUI._Instance.areaListUI.Subtract.interactable = !value;
    }
    //ShootingArea列表的点击事件
    void UIClickEvent(ShootingArea value)
    {
        List<Object> selection;

        selection = new List<Object>();

        selection.Insert(0, value.Perfab);

        Editor.Undo.Select(selection.ToArray(), value.Perfab);
    }

  

    //设置Tools种类
    void SetRuntimeTool(RuntimeTool runtimeTool)
    {
        switch (runtimeTool)
        {
            case RuntimeTool.Move:
                Editor.Tools.Current = RuntimeTool.Move;
                break;
            case RuntimeTool.Rotate:
                Editor.Tools.Current = RuntimeTool.Rotate;
                break;
            case RuntimeTool.Scale:
                Editor.Tools.Current = RuntimeTool.Scale;
                break;
            case RuntimeTool.Lock:
                if (!Lock)
                    LockAcitiveArea();
                else
                    UnLockAcitiveArea();
                break;
        }
    }

    //锁定Area
    void LockAcitiveArea()
    {
        GameObject[] selected = Editor.Selection.gameObjects;     
        Lock = true;
        if (selected != null)
        {
            for (int i = 0; i < selected.Length; ++i)
            {
                GameObject selectedObj = selected[i];
                EditorArea = selectedObj;
                Debug.Log(selectedObj.name);
                ExposeToEditor exposeToEditor = EditorArea.GetComponent<ExposeToEditor>();
                SelectionGizmo selectionGizmo = EditorArea.AddComponent<SelectionGizmo>();

                if (exposeToEditor.Selected != null)
                {
                    exposeToEditor.Selected.Invoke(exposeToEditor);
                }
            }
        }
        //Editor.Selection.activeObject = null;

        Editor.Mask = Editor.Mask = (1 << 11);
    }

    //解锁Area
    void UnLockAcitiveArea()
    {
        Lock = false;
        List<Object> selection;

        selection = new List<Object>();

        selection.Insert(0, EditorArea);

        Editor.Undo.Select(selection.ToArray(), EditorArea);

        Editor.Mask = Editor.Mask = (1 << 10);

        //Set_LockItemList(false);

        
        EditorArea = null;
    }

    //ShootAreaList选定删除
    void Subtract_Arealist(GameObject[] area)
    {
        if (area.Length == 0)
            return;

        List<int> m_Area = getShootingArea(area);

        foreach (int i in m_Area)
        {
            m_Arealist[i].Destroy_obj();
            m_Arealist.RemoveAt(i);
        }
    }
    //获取参数中物体在ShootAreaList中的标记
    List<int> getShootingArea(GameObject[] values)
    {
        List<int> m_Area = new List<int>();
        for (int i = 0; i < m_Arealist.Count; i++)
        {
            foreach (GameObject j in values)
            {
                if (m_Arealist[i].Perfab.name == j.name)
                {
                    m_Area.Add(i);
                }
            }
        }
        return m_Area;
    }
    //获取锁定Area的ShootingArea属性结构
    ShootingArea GetEditorArea()
    {
        return m_Arealist[getShootingArea(GetEditorAreaObj())[0]];
    }


    //


    void refreshUI()
    {

    }
}
