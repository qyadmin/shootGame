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

    public GameObject m_editorArea
    {
        get
        {
            return EditorArea;
        }
    }

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
        EditorUI._Instance.runtimeToolUI.View_Button.onClick.AddListener(delegate () { OnPlayClick(); });
        //Editor.Selection.SelectionChanged += test;
        Editor.Selection.Selectioned += ToolsUIChange;
        Editor.Mask = (1 << 10);
        Set_LockItemList(Lock, null);
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
        newArea.AddItem(Resources.Load<GameObject>("区域射击点"), Resources.Load<Sprite>("UI/Sprite/纸靶子"));
        newArea.ItemList.AddItem(Resources.Load<GameObject>("钢靶(红)"), Resources.Load<Sprite>("UI/Sprite/纸靶子"));
        newArea.ItemList.AddItem(Resources.Load<GameObject>("钢靶(黄)"), Resources.Load<Sprite>("UI/Sprite/纸靶子"));
        newArea.ItemList.AddItem(Resources.Load<GameObject>("钢靶(灰)"), Resources.Load<Sprite>("UI/Sprite/纸靶子"));



        m_Arealist.Add(newArea);
    }
    //增加Area的Item
    public void Add_AreaItemList(GameObject prefabInstance, int number)
    {
        GetEditorArea().ItemList.m_ShootingItem.ForEach(item =>
        {
            if (item.Number == number)
            {
                ShootingItem newItem = new ShootingItem();
                newItem = item;
                newItem.Prefab = prefabInstance;
                newItem.Prefab.transform.parent = GetEditorArea().Perfab.transform;
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

        if ((selected != null && selected.Length != 0) || EditorArea != null)
        {
            GameObject[] m_selected = new GameObject[1];

            if (EditorArea != null)
                m_selected[0] = EditorArea;
            else
                m_selected[0] = selected[0] as GameObject;
            if (m_selected[0].layer == 10)
                m_Arealist.ForEach(item =>
                {
                    if (item.Perfab == m_selected[0])
                    {
                        item.ItemList.Instantiate_obj(EditorUI._Instance.itemListUI.Prefab, EditorUI._Instance.itemListUI.ItemListUICount.gameObject);
                        item.Instantiate_Item(EditorUI._Instance.areaItemListUI.AreaItemListPrefabUI, EditorUI._Instance.areaItemListUI.AreaItemListUICount, Editor);
                    }
                });
            if(selected != null)
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

        Set_LockItemList(Lock, selected);
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
    void Set_LockItemList(bool value, Object[] selected)
    {
        EditorUI._Instance.itemListUI.LockUI.gameObject.SetActive(!value);

        EditorUI._Instance.areaListUI.LockUI.gameObject.SetActive(value);

        EditorUI._Instance.areaItemListUI.LockUI.gameObject.SetActive(!value);

        for (int i = 0; i < EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.itemListUI.ItemListUICount.gameObject.transform.GetChild(i).GetComponent<PrefabSpawnPoint>().CanDrag = value;

        }
        for (int i = 0; i < EditorUI._Instance.areaListUI.AreaUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.areaListUI.AreaUICount.gameObject.transform.GetChild(i).GetComponent<Button>().interactable = !value;
            EditorUI._Instance.areaListUI.AreaUICount.gameObject.transform.GetChild(i).transform.Find("Sprite").GetComponent<Image>().sprite =
                EditorUI._Instance.areaListUI.ordinary;
        }
        for (int i = 0; i < EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.childCount; i++)
        {
            EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.GetChild(i).GetComponent<Button>().interactable = value;
            EditorUI._Instance.areaItemListUI.AreaItemListUICount.gameObject.transform.GetChild(i).transform.Find("Sprite").GetComponent<Image>().sprite =
                EditorUI._Instance.areaItemListUI.ordinary;
        }
        EditorUI._Instance.areaListUI.Add.interactable = !value;
        EditorUI._Instance.areaListUI.Subtract.interactable = !value;

        if (EditorArea)
            GetEditorArea().PerfabUI.transform.Find("Sprite").GetComponent<Image>().sprite = EditorUI._Instance.areaListUI.active;


        if (selected != null && selected.Length > 0)
        {
            if ((selected[0] as GameObject).layer == 10)
            {
                GetActiveArea(selected[0] as GameObject).PerfabUI.transform.Find("Sprite").GetComponent<Image>().sprite = EditorUI._Instance.areaListUI.active;
            }
            if ((selected[0] as GameObject).layer == 11)
            {
                getActiveItem(selected[0] as GameObject).PrefabUI.transform.Find("Sprite").GetComponent<Image>().sprite = EditorUI._Instance.areaItemListUI.active;
            }
        }
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

    //list换位排序
    public void ShootAreaSorting()
    {
        List<ShootingArea> newlist = new List<ShootingArea>();

        foreach (Transform i in EditorUI._Instance.areaListUI.AreaUICount.transform)
        {
            m_Arealist.ForEach(item =>
            {
                if (i.gameObject == item.PerfabUI)
                    newlist.Add(item);
            });
        }

        m_Arealist = newlist;

        m_Arealist.ForEach(item =>
        {
            Debug.Log(item.m_Number);
        });
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
                //Debug.Log(selectedObj.name);
                //ExposeToEditor exposeToEditor = EditorArea.GetComponent<ExposeToEditor>();
                //SelectionGizmo selectionGizmo = EditorArea.AddComponent<SelectionGizmo>();

                //if (exposeToEditor.Selected != null)
                //{
                //    exposeToEditor.Selected.Invoke(exposeToEditor);
                //}
            }
        }
        //Editor.Selection.activeObject = null;

        Editor.Undo.Select(null, null);

        Editor.Tools.Current = RuntimeTool.Move;

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

        Editor.Tools.Current = RuntimeTool.Move;
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

    int getShootingArea(GameObject values)
    {
        int m_Area = new int();
        for (int i = 0; i < m_Arealist.Count; i++)
        {

            if (m_Arealist[i].Perfab.name == values.name)
            {
                m_Area = i;
                break;
            }

        }
        return m_Area;
    }
    //获取锁定Area的ShootingArea属性结构
    ShootingArea GetEditorArea()
    {
        return m_Arealist[getShootingArea(GetEditorAreaObj())[0]];
    }

    ShootingArea GetActiveArea(GameObject value)
    {
        return m_Arealist[getShootingArea(value)];
    }


    //获取选中item
    ShootingItem getActiveItem(GameObject value)
    {
        ShootingItem newItem = new ShootingItem();
        GetEditorArea().m_ShootingItem.ForEach(item =>
        {
            if (item.Prefab == value)
                newItem = item;
        });
        Debug.Log(newItem.PrefabUI.name);
        return newItem;
    }

    [SerializeField]
    bool isPlaying = false;
    private void OnPlayClick()
    {
        isPlaying = !isPlaying;
        ShootGame._Instance.isEditor = !isPlaying;
        if (isPlaying)
        {
            //Editor.Undo.Purge();
            CoPlay();
        }
        else
            OnStopClick();

        ShootGame._Instance.Start();
    }

    private void CoPlay()
    {
        Editor.Selection.Enabled = false;
        Editor.Undo.Select(null, null);
        EditorUI._Instance.playingModle();
        Associated();
        if (Lock)
            UnLockAcitiveArea();
    }
    private void OnStopClick()
    {
        Editor.Selection.Enabled = true;
        Editor.Undo.Select(null, null);
        EditorUI._Instance.editorModle();
    }

    //射击点与靶子关联
    void Associated()
    {
        m_Arealist.ForEach(item =>
        {
            //InteractiveSwitch Point = new InteractiveSwitch();
            List<TargetHealth> targets = new List<TargetHealth>();
            item.m_ShootingItem.ForEach(obj =>
            {
                if (obj.Prefab.GetComponent<InteractiveSwitch>())
                {
                    //Point = obj.Prefab.GetComponent<InteractiveSwitch>();
                    item.m_ShootPos = obj;
                }                   
                if (obj.Prefab.GetComponent<TargetHealth>())
                    targets.Add(obj.Prefab.GetComponent<TargetHealth>());
            });
            item.m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().targets = targets;
        });
        if (m_Arealist.Count == 1)
        {
            m_Arealist.First<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().startVisible = true;
            m_Arealist.First<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().levelEnd = true;
        }
        if (m_Arealist.Count > 1)
        {
            m_Arealist.First<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().startVisible = true;
            m_Arealist.First<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().levelEnd = false;
            m_Arealist.Last<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().startVisible = false;
            m_Arealist.Last<ShootingArea>().m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().levelEnd = true;
        }

        for (int i = 0; i < m_Arealist.Count; i++)
        {
            if (i + 1 < m_Arealist.Count)
                m_Arealist[i].m_ShootPos.Prefab.GetComponent<InteractiveSwitch>().nextStage =
                    m_Arealist[i + 1].m_ShootPos.Prefab.GetComponent<InteractiveSwitch>();
        }
    }





    void refreshUI()
    {

    }
}
