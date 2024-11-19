using UnityEngine;

public class FloatParameter : ItemParameter<object>
{
    public float value;

    public FloatParameter()
    {
        parameterValue = value;
    }
}
