using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum WORDTYPE
{
    NONE = 0,
    SMALL = 2 << 0,
    SMALLSMALL = 2 << 1,
    BIG = 2 << 2,
    BIGBIG = 2 << 3,
    TALL = 2 << 4,
    TALLTALL = 2 << 5,
    LONG = 2 << 6,
    LONGLONG = 2 << 7,
    // NO DOUBLE FOR THOSE?
    STAIRS = 2 << 8,
    STICKY = 2 << 9,
    BOUNCY = 2 << 10,
    BALL = 2 << 11,
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

    public static void AddBaseModifiers(WORDTYPE type, ref List<WordModifier> list)
    {
        if ((int)type == 0) return;

        if (type.HasFlag(WORDTYPE.SMALL))
            list.Add(new SmallModifier());

        if (type.HasFlag(WORDTYPE.SMALLSMALL))
        {
            list.Add(new SmallModifier());
            list.Add(new SmallModifier());
        }

        if (type.HasFlag(WORDTYPE.BIG))
            list.Add(new BigModifier());

        if (type.HasFlag(WORDTYPE.BIGBIG))
        {
            list.Add(new BigModifier());
            list.Add(new BigModifier());
        }

        if (type.HasFlag(WORDTYPE.TALL))
            list.Add(new TallModifier());

        if (type.HasFlag(WORDTYPE.TALLTALL))
        {
            list.Add(new TallModifier());
            list.Add(new TallModifier());
        }

        if (type.HasFlag(WORDTYPE.LONG))
            list.Add(new LongModifier());

        if (type.HasFlag(WORDTYPE.LONGLONG))
        {
            list.Add(new LongModifier());
            list.Add(new LongModifier());
        }
    }
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
