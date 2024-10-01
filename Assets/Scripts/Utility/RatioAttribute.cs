using UnityEngine;

public class RatioAttribute : PropertyAttribute
{
    public float Min { get; }
    public float Max { get; }
    public bool AllowEdit { get; }

    public RatioAttribute(float min, float max, bool allowEdit = true)
    {
        Min = min;
        Max = max;
        AllowEdit = allowEdit;
    }
}
