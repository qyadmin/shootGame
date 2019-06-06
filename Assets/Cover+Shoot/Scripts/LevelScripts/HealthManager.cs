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
    DoorOrWindow
}
// This is a template script for in-game object health manager.
// Any in-game entity that reacts to a shot must have this script with the public function TakeDamage().
public class HealthManager : MonoBehaviour
{
    // This is the mandatory function that receives damage from shots.
    // You may remove the 'virtual' keyword before coding the content.

    public bool Can_Through;
    public HealthManager LinkObj;
    public virtual void TakeDamage(Vector3 location, Vector3 direction, float damage)
	{
	}

    public virtual void TargetAwake()
    {

    }

    public virtual void TargetResets()
    {

    }
}
