using System;
using UnityEngine;

[Flags]
public enum WORDTYPE{
    NONE   = 0,
    SMALL  = 2 << 0,
    BIG    = 2 << 1,
    TALL   = 2 << 2,
    LONG   = 2 << 3, 
    STAIRS = 2 << 4,
    BALL   = 2 << 5,
    STICKY = 2 << 6,
    BOUNCY = 2 << 7,
}

[Serializable]
public abstract class WordModifier
{
    protected string name = "WordModifier";
    abstract public void DebugName();

    public string GetName()
    {
        return name;
    }

    abstract public void Apply(Transform transform);
    abstract public void DeApply(Transform transform);
}

#region Scale

[Serializable]
public abstract class ScaleModifier : WordModifier
{
    public Vector3 scale;

    public override void Apply(Transform transform)
    {
        transform.localScale = new(transform.localScale.x * scale.x, transform.localScale.y * scale.y);
    }

    public override void DeApply(Transform transform)
    {
        transform.localScale = new(transform.localScale.x / scale.x, transform.localScale.y / scale.y);
    }
}

[Serializable]
public class SmallModifier : ScaleModifier
{
    public SmallModifier()
    {
        name = "Small";
        scale = new Vector2(0.5f, 0.5f);
    }

    public override void DebugName()
    {
        Debug.Log("SmallModifier");
    }
}

[Serializable]
public class BigModifier : ScaleModifier
{
    public BigModifier()
    {
        name = "Big";
        scale = new Vector2(2, 2);
    }

    public override void DebugName()
    {
        Debug.Log("BigModifier");
    }
}

[Serializable]
public class TallModifier : ScaleModifier
{
    public TallModifier()
    {
        name = "Tall";
        scale = new Vector2(1, 2);
    }

    public override void DebugName()
    {
        Debug.Log("TallModifier");
    }
}

[Serializable]
public class LongModifier : ScaleModifier
{
    public LongModifier()
    {
        name = "Long";
        scale = new Vector2(2, 1);
    }

    public override void DebugName()
    {
        Debug.Log("LongModifier");
    }
}

#endregion

#region Effect

public abstract class EffectModifier : WordModifier
{
   
}

#endregion

#region Shape
public abstract class ShapeModifier : WordModifier
{
    
}

#endregion
