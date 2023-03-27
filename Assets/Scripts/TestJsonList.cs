using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestJsonList : MonoBehaviour
{
    [SerializeField]
    TestScriptableObject scriptableObject;

    void Start()
    {
        scriptableObject.Test = 100;
        EditorUtility.SetDirty(scriptableObject);
        AssetDatabase.SaveAssets();
    }
}
