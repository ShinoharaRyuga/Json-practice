using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumeratorTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var savePath = Application.dataPath + "/PlayData" + "/PlayData.json";
        StartCoroutine(JsonProcess.EnumeratorTest(savePath, out IAwaiter<Data> awiter));

        Debug.Log($"HP {awiter.Result._hp}");
        Debug.Log($"MP {awiter.Result._mp}");
        Debug.Log($"AttackPower {awiter.Result._attackPower}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
