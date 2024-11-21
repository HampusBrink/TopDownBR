using System;
using UnityEngine;

public class FloatParameter : ItemParameter<object>
{
    public float value;

    public FloatParameter()
    {
        parameterValue = value;
    }

    private void Awake()
    {
        parameterValue = value;
    }
}
