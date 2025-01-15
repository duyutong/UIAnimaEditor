using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public ETest eTest;
    public void OnTest()
    {
        Debug.Log("触发函数 ---- Test.OnTest()");
    }
}
public enum ETest
{
    None,
    One,
    Two,
    Three,
    Four,
}
