using UnityEngine;

[CreateAssetMenu]
public class TestScriptableObject : ScriptableObject
{
    [SerializeField]
    int _test = 0;

    public int Test { get => _test; set => _test = value; }
}
