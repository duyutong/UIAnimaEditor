using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRuntimeComponent : MonoBehaviour
{
    public BTContainer container;
    public BTRuntime runtime;
    public void OnEnable()
    {
        InitRuntime();
        runtime.OnEnable();
    }

    public void InitRuntime()
    {
        runtime ??= new BTRuntime();
        runtime.container = container;
        runtime.gameObject = gameObject;
        runtime.transform = transform;
    }

    private void Update()
    {
        if (runtime != null) { runtime.OnUpdate(); }
    }
    public void OnDisable()
    {
        if (runtime != null) runtime.OnDisable();
    }
    public void OnDestroy()
    {
        if (runtime != null) runtime.OnDestroy();
    }
}
