using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting;

public interface IAwaiter<T> // 結果を受け取る側のためのインターフェイス
{
    /// <summary>
    /// 処理が終了したかどうか。
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// 処理の結果。
    /// </summary>
    T Result { get; }
}

public class Awaiter<T> : IAwaiter<T> // 結果を設定する側の実装
{
    public bool IsCompleted { get; private set; }

    public T Result { get; private set; }

    /// <summary>
    /// 処理を終了して結果を設定する。
    /// </summary>
    public void SetResult(T result)
    {
        Result = result;
        IsCompleted = true;
    }
}


public class JsonProcess : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreateJsonFile();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            var data = Load();
            Debug.Log($"HP {data._hp}");
            Debug.Log($"MP {data._mp}");
            Debug.Log($"AttackPower {data._attackPower}");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            TestDataSave();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            var testData = TestDataLoad();
            Debug.Log(testData._boolArray.Length);
            Debug.Log(testData._boolList.Count);
            Debug.Log($"list {testData._boolList[8]}");
            Debug.Log($"array {testData._boolArray[8]}");
        }
    }

    /// <summary>データを保存 </summary>
    /// <param name="encrypto">暗号化するか</param>
    void Save(bool encrypto)
    {
        Debug.Log("保存");
        var path = Application.persistentDataPath + "PlayData.json";
        var data = new Data(10, 10, 10);
        var json = JsonUtility.ToJson(data);


        if (encrypto)
        {
            var jsonByte = Encoding.UTF8.GetBytes(json);
            jsonByte = AesEncrypt(jsonByte);

            File.WriteAllBytes(path, jsonByte);
        }
        else
        {
            File.WriteAllText(path, json);
        }
    }

    void TestDataSave()
    {
        var data = new TestData(10);
        var path = Application.dataPath + "/TestData.json";

        var json = JsonUtility.ToJson(data);

        File.WriteAllText(path, json);
    }

    TestData TestDataLoad()
    {
        var path = Application.dataPath + "/TestData.json";
        var json = File.ReadAllText(path);

        return JsonUtility.FromJson<TestData>(json);
    }

    /// <summary>データを保存する </summary>
    public void CreateJsonFile()
    {
        Debug.Log("保存");
        var savePath = Application.persistentDataPath + "PlayData.json";     //保存先のパス
        var saveData = JsonUtility.ToJson(new Data(30, 30, 30));                  //保存するデータを作成
        File.WriteAllText(savePath, saveData);  //データを書き込む
    }

    /// <summary>データを読み込み</summary>
    Data Load()
    {
        Debug.Log("読み込み");
        var path = Application.persistentDataPath + "PlayData.json";
        var byteData = File.ReadAllBytes(path);
        byteData = AesDecrypt(byteData); //復号化

        var json = Encoding.UTF8.GetString(byteData);
        // var json = File.ReadAllText(path);
       
        return JsonUtility.FromJson<Data>(json);
    }

    /// <summary> AES暗号化 </summary>
    byte[] AesEncrypt(byte[] byteText)
    {
        // AES設定値　aesIv・aesKeyは暗号化と複合化で同じものを使用する
        //===================================
        int aesKeySize = 128;
        int aesBlockSize = 128;
        string aesIv = "6KGhH66PeU3cSLS7";
        string aesKey = "R38FYEzPyjxv0HrE";
        //===================================

        // AESマネージャー取得
        var aes = GetAesManager(aesKeySize, aesBlockSize, aesIv, aesKey);
        // 暗号化
        byte[] encryptText = aes.CreateEncryptor().TransformFinalBlock(byteText, 0, byteText.Length);

        return encryptText;
    }

    /// <summary>
    /// AES復号化
    /// </summary>
    byte[] AesDecrypt(byte[] byteText)
    {
        // AES設定値
        //===================================
        int aesKeySize = 128;
        int aesBlockSize = 128;
        string aesIv = "6KGhH66PeU3cSLS7";
        string aesKey = "R38FYEzPyjxv0HrE";
        //===================================

        // AESマネージャー取得
        var aes = GetAesManager(aesKeySize, aesBlockSize, aesIv, aesKey);
        // 復号化
        byte[] decryptText = aes.CreateDecryptor().TransformFinalBlock(byteText, 0, byteText.Length);

        return decryptText;
    }

    /// <summary>
    /// AesManagedを取得
    /// </summary>
    /// <param name="keySize">暗号化鍵の長さ</param>
    /// <param name="blockSize">ブロックサイズ</param>
    /// <param name="iv">初期化ベクトル(半角X文字（8bit * X = [keySize]bit))</param>
    /// <param name="key">暗号化鍵 (半X文字（8bit * X文字 = [keySize]bit）)</param>
    AesManaged GetAesManager(int keySize, int blockSize, string iv, string key)
    {
        AesManaged aes = new AesManaged();
        aes.KeySize = keySize;
        aes.BlockSize = blockSize;
        aes.Mode = CipherMode.CBC;
        aes.IV = Encoding.UTF8.GetBytes(iv);
        aes.Key = Encoding.UTF8.GetBytes(key);
        aes.Padding = PaddingMode.PKCS7;
        return aes;
    }

    public static IEnumerator EnumeratorTest<T>(string savePath, out IAwaiter<T> awaiter)
    {
        var impl = new Awaiter<T>();
        var e = EnumeratorTest(savePath, impl);
        awaiter = impl;
        return e;
    }

    static IEnumerator EnumeratorTest<T>(string savePath, Awaiter<T> awaiter)
    {
        var json = File.ReadAllText(savePath);
        awaiter.SetResult(JsonConvert.DeserializeObject<T>(json));

        yield return null;
    }


    /// <summary>テストデータを読む込む </summary>
    /// <param name="savePath">保存先のパス</param>
    public static T TestLoadData<T>(string savePath)
    {
        var json = File.ReadAllText(savePath);

        return JsonConvert.DeserializeObject<T>(json);
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

[Serializable]
public class TestData
{
    public List<bool> _boolList;

    public bool[] _boolArray;

    public TestData(int lenght)
    {
        _boolList = new List<bool>();
        _boolArray = new bool[lenght];

        for (var i = 0; i < lenght; i++)
        {
            _boolList.Add(false);
        }
    }
}