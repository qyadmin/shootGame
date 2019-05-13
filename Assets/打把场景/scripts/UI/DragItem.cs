using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, ICanvasRaycastFilter
{
    private static Transform canvasTra;
    private Transform nowParent;//物品是格子的子物体，nowParent记录的是当前物品属于哪个格子（格子1）
    private bool isRaycastLocationValid = true;//默认射线不能穿透物品
    private Camera canvasCamera = null;
    void Start()
    {

    }

    public void OnBeginDrag(PointerEventData eventData)//开始拖拽
    {
        if (canvasTra == null) canvasTra = GameObject.Find("EditorHUD").transform;
        canvasCamera = GameObject.Find("EditorCamera").GetComponent<Camera>();
        //Debug.Log(Input.mousePosition);
        nowParent = transform.parent;//nowparent为被拖拽物体的原始父物体
        transform.SetParent(canvasTra);//将当前拖拽的物品放在canvas下
        isRaycastLocationValid = false;//ui穿透：置为可以穿透  【拖拽物体移动的时候鼠标下是有物体一直跟随遮挡的，如果不穿透就获取不到放置位置（OnEndDrag中判断是空格子，物体，还是无效位置）】

    }

    public void OnDrag(PointerEventData eventData)//拖拽过程中
    {
        //transform.position = canvasCamera.ScreenToWorldPoint(Input.mousePosition);//鼠标左键按住拖拽的时候，物体跟着鼠标移动
                                                                                  transform.position = Input.mousePosition;//鼠标左键按住拖拽物体的时候不显示物体
        Debug.Log(canvasCamera.ScreenToWorldPoint(Input.mousePosition));
    }

    public void OnEndDrag(PointerEventData eventData)//
    {
        GameObject go = eventData.pointerCurrentRaycast.gameObject;//获取到鼠标终点位置下 可能的物体


        if (go != null)
        {
            Debug.Log(go.name);
            if (go.tag.Equals("Grid"))//鼠标终点位置下是： 空格子（所以直接放进去）
            {
                SetParentAndPosition(transform, go.transform);
            }
            else if (go.tag.Equals("PlayerPetItem"))//鼠标终点位置下是： 也是一个物体（所以需要交换位置）
            {//交换位置要注意可能需要把物品下的子物体的Raycast Target关掉（不去掉可能无法交换）
                //SetParentAndPosition(transform, go.transform.parent);//将被拖拽的物体1放到鼠标终点下的格子2里面
                //SetParentAndPosition(go.transform, nowParent);//将鼠标终点格子2里面物体2 放到 原来物体1的格子1里面
                SetChild(go.transform, nowParent);
                    
            }
            else//鼠标终点是：无效位置（所以物体放回原来的位置）
            {
                SetParentAndPosition(transform, nowParent);
            }
        }
        else//
        {
            SetParentAndPosition(transform, nowParent);
        }
        ShootGameEditor._Instance.ShootAreaSorting();
        isRaycastLocationValid = true;//ui事件穿透：置为不能穿透
    }

    private void SetParentAndPosition(Transform child, Transform parent)//将child放到parent下作为子物体
    {
        child.SetParent(parent);
        child.position = parent.position;
    }

    private void SetChild(Transform change_child, Transform paren)
    {
        int changeChildNum;
        for (int i = 0; i < change_child.parent.childCount; i++)
        {
            if (change_child.parent.GetChild(i) == change_child)
            {
                changeChildNum = i;

                transform.SetParent(paren);
                transform.SetSiblingIndex(changeChildNum);
                break;
            }
        }
        
    }



    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)//UI事件穿透：如置为false即可以穿透，被图片覆盖的按钮可以被点击到
    {
        return isRaycastLocationValid;
    }
}