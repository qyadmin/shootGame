using System.Collections.Generic;
using UnityEngine;


public enum TargetType
{
    IDPA,
    IPSC,
    UDMove,
    LRMove,
    BarSteel,
    SliceSteel,
    Rotate_single,
    Rotate_double,
    Slide_single,
    Slide_double,
    DoorOrWindow,
    MolotovCocktails,
    balloon,
    coke,
    watermelon
}
// This is a template script for in-game object health manager.
// Any in-game entity that reacts to a shot must have this script with the public function TakeDamage().
public class HealthManager : MonoBehaviour
{
    // This is the mandatory function that receives damage from shots.
    // You may remove the 'virtual' keyword before coding the content.

    public bool Can_Through;
    public bool ProhibitShooting;//禁射
    public bool InvalidItem;//无效

    public GameObject[] RotatePos;
    public List<GameObject> EffectGroup = new List<GameObject>();
    public HealthManager LinkObj;
    public virtual void TakeDamage(Ray ray, RaycastHit hit, float damage)
	{
	}
    public virtual void TakeDamage(Ray ray, RaycastHit hit, float damage,bool isthough)
    {
    }

    public virtual void TargetLink()
    {

    }

    public virtual void TargetAwake()
    {

    }

    public virtual void TargetResets()
    {

    }

    public virtual void TargetEnd()
    {

    }

    public virtual void Hiteffect(Ray ray,RaycastHit hit)
    {

    }
}
