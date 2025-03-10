using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum WORDTYPE
{
    NONE         = 0,
    SMALL        = 2 << 0,
    SMALLSMALL   = 2 << 1,
    BIG          = 2 << 2,
    BIGBIG       = 2 << 3,
    TALL         = 2 << 4,
    TALLTALL     = 2 << 5,
    LONG         = 2 << 6,
    LONGLONG     = 2 << 7,

    BOUNCY       = 2 << 8,

    STICKY       = 2 << 9,

    STAIRS       = 2 << 10,

    BALL         = 2 << 11,
}

[Serializable]
public abstract class WordModifier
{
    [field: SerializeField] public WordUI WordUI { get; set; }
    [field: SerializeField, ReadOnly] public WordBase Owner { get; set; }

    [field: SerializeField, ReadOnly] protected string name = "WordModifier";

    abstract public void DebugName();

    public string GetName()
    {
        return name;
    }

    abstract public void Apply(WordObject wordObject);

    public WordModifier(WordBase owner)
    {
        this.Owner = owner;
    }

    public static void AddBaseModifiers(WORDTYPE type, ref List<WordModifier> list, WordBase owner)
    {
        if ((int)type == 0) return;

        if (type.HasFlag(WORDTYPE.SMALL))
            list.Add(new SmallModifier(owner));

        if (type.HasFlag(WORDTYPE.SMALLSMALL))
        {
            list.Add(new SmallModifier(owner));
            list.Add(new SmallModifier(owner));
        }

        if (type.HasFlag(WORDTYPE.BIG))
            list.Add(new BigModifier(owner));

        if (type.HasFlag(WORDTYPE.BIGBIG))
        {
            list.Add(new BigModifier(owner));
            list.Add(new BigModifier(owner));
        }

        if (type.HasFlag(WORDTYPE.TALL))
            list.Add(new TallModifier(owner));

        if (type.HasFlag(WORDTYPE.TALLTALL))
        {
            list.Add(new TallModifier(owner));
            list.Add(new TallModifier(owner));
        }

        if (type.HasFlag(WORDTYPE.LONG))
            list.Add(new LongModifier(owner));

        if (type.HasFlag(WORDTYPE.LONGLONG))
        {
            list.Add(new LongModifier(owner));
            list.Add(new LongModifier(owner));
        }

        if (type.HasFlag(WORDTYPE.BOUNCY))
            list.Add(new BouncyModifier(owner));


        if (type.HasFlag(WORDTYPE.STICKY))
            list.Add(new StickyModifier(owner));

        if (type.HasFlag(WORDTYPE.BALL))
            list.Add(new BallModifier(owner));

        if (type.HasFlag(WORDTYPE.STAIRS))
            list.Add(new StairsModifier(owner));
    }
}

#region Scale

[Serializable]
public abstract class ScaleModifier : WordModifier
{
    public Vector3 scale;
    public float appliedTimer = 0;

    protected ScaleModifier(WordBase owner) : base(owner)
    {
    }

    public override void Apply(WordObject wordObject)
    {
        wordObject.TargetScale = Vector3.Scale(scale, wordObject.TargetScale);
    }

    public Vector3 GetScale()
    {
        return scale;
    }

    public bool IsGreatScaleX() { return scale.x > 1; }
    public bool IsGreatScaleY() { return scale.y > 1; }
}

[Serializable]
public class SmallModifier : ScaleModifier
{
    public SmallModifier(WordBase owner) : base(owner)
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
    public BigModifier(WordBase owner) : base(owner)
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
    public TallModifier(WordBase owner) : base(owner)
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
    public LongModifier(WordBase owner) : base(owner)
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

#region NonScale
public abstract class NonScaleModifier : WordModifier
{
    protected NonScaleModifier(WordBase owner) : base(owner)
    {
    }

    
}

#region Effect

public abstract class EffectModifier : NonScaleModifier
{
    protected EffectModifier(WordBase owner) : base(owner)
    {
    }
}

[Serializable]
public class StickyModifier : EffectModifier
{
    public StickyModifier(WordBase owner) : base(owner)
    {
        name = "Sticky";
    }

    public override void Apply(WordObject wordObject)
    {
        wordObject.BlockIsSticky = true;
    }

    public override void DebugName()
    {
        Debug.Log("StickyModifier");
    }
}

[Serializable]
public class BouncyModifier : EffectModifier
{
    public BouncyModifier(WordBase owner) : base(owner)
    {
        name = "Bouncy";
    }

    public override void Apply(WordObject wordObject)
    {
        wordObject.BlockIsBouncy = true;
    }

    public override void DebugName()
    {
        Debug.Log("BouncyModifier");
    }
}

#endregion

#region Shape
public abstract class ShapeModifier : NonScaleModifier
{
    protected ShapeModifier(WordBase owner) : base(owner)
    {
    }
}

public class StairsModifier : ShapeModifier
{
    public StairsModifier(WordBase owner) : base(owner)
    {
        name = "Stairs";
        
    }

    public override void Apply(WordObject wordObject)
    {
        
    }

    public override void DebugName()
    {
        Debug.Log("StairsModifier");
    }
}

public class BallModifier : ShapeModifier
{
    public BallModifier(WordBase owner) : base(owner)
    {
        name = "Ball";

    }

    public override void Apply(WordObject wordObject)
    {
        
    }

    public override void DebugName()
    {
        Debug.Log("BallModifier");
    }
}

#endregion

#endregion NonScale