using UnityEngine;

public class BTRuntimeComponent : MonoBehaviour
{
    public BTContainer container;
    public BTRuntime runtime;
    public void OnEnable()
    {
        InitRuntime();
        runtime?.OnEnable();
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
        runtime?.OnUpdate();
    }
    public void OnDisable()
    {
        runtime?.OnDisable();
    }
    public void OnDestroy()
    {
        runtime?.OnDestroy();
    }
}
