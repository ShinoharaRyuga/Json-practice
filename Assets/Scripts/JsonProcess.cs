using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

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
    }

    /// <summary>�f�[�^��ۑ� </summary>
    void Save(bool flag)
    {
        Debug.Log("�ۑ�");
        var path = Application.persistentDataPath + "PlayData.json";
        var data = new Data(10, 10, 10);
        var json = JsonUtility.ToJson(data);


        if (flag)
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

    /// <summary>�f�[�^��ۑ����� </summary>
    public void CreateJsonFile()
    {
        Debug.Log("�ۑ�");
        var savePath = Application.persistentDataPath + "PlayData.json";     //�ۑ���̃p�X
        var saveData = JsonUtility.ToJson(new Data(30, 30, 30));                  //�ۑ�����f�[�^���쐬
        File.WriteAllText(savePath, saveData);  //�f�[�^����������
    }

    /// <summary>�f�[�^��ǂݍ���</summary>
    Data Load()
    {
        Debug.Log("�ǂݍ���");
        var path = Application.persistentDataPath + "PlayData.json";
        var byteData = File.ReadAllBytes(path);
        byteData = AesDecrypt(byteData);

        var json = Encoding.UTF8.GetString(byteData);
        // var json = File.ReadAllText(path);
       
        return JsonUtility.FromJson<Data>(json);
    }

    /// <summary>
    /// AES�Í���
    /// </summary>
    byte[] AesEncrypt(byte[] byteText)
    {
        // AES�ݒ�l�@aesIv�EaesKey�͈Í����ƕ������œ������̂��g�p����
        //===================================
        int aesKeySize = 128;
        int aesBlockSize = 128;
        string aesIv = "6KGhH66PeU3cSLS7";
        string aesKey = "R38FYEzPyjxv0HrE";
        //===================================

        // AES�}�l�[�W���[�擾
        var aes = GetAesManager(aesKeySize, aesBlockSize, aesIv, aesKey);
        // �Í���
        byte[] encryptText = aes.CreateEncryptor().TransformFinalBlock(byteText, 0, byteText.Length);

        return encryptText;
    }

    /// <summary>
    /// AES������
    /// </summary>
    byte[] AesDecrypt(byte[] byteText)
    {
        // AES�ݒ�l
        //===================================
        int aesKeySize = 128;
        int aesBlockSize = 128;
        string aesIv = "6KGhH66PeU3cSLS7";
        string aesKey = "R38FYEzPyjxv0HrE";
        //===================================

        // AES�}�l�[�W���[�擾
        var aes = GetAesManager(aesKeySize, aesBlockSize, aesIv, aesKey);
        // ������
        byte[] decryptText = aes.CreateDecryptor().TransformFinalBlock(byteText, 0, byteText.Length);

        return decryptText;
    }

    /// <summary>
    /// AesManaged���擾
    /// </summary>
    /// <param name="keySize">�Í������̒���</param>
    /// <param name="blockSize">�u���b�N�T�C�Y</param>
    /// <param name="iv">�������x�N�g��(���pX�����i8bit * X = [keySize]bit))</param>
    /// <param name="key">�Í����� (��X�����i8bit * X���� = [keySize]bit�j)</param>
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