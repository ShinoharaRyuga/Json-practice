using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonProcess : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            var data = Load();
            Debug.Log($"HP {data._hp}");
            Debug.Log($"MP {data._mp}");
            Debug.Log($"AttackPower {data._attackPower}");
        }
    }

    /// <summary>�f�[�^��ۑ� </summary>
    void Save()
    {
        Debug.Log("�ۑ�");
        var path = Application.persistentDataPath + "/PlayData/" + "PlayData.json";
        var data = new Data(10, 10, 10);
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(path, json);
    }

    /// <summary>�f�[�^��ǂݍ���</summary>
    Data Load()
    {
        Debug.Log("�ǂݍ���");
        var path = Application.dataPath + "/PlayData/" + "PlayData.json";
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<Data>(json);

        return new Data(10, 10, 10);
    }
}

[SerializeField]
public class Data
{
    public int _hp;
    public int _mp;
    public int _attackPower;

    public Data(int hp, int mp, int attackPower)
    {
        _hp = hp;
        _mp = mp;
        _attackPower = attackPower;
    }
}