using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    public PlayerData playerData;
    private string filePath;
    private SaveLoadData saveLoadData;

    void Awake()
    {
        // 1. 构造保存路径
        filePath = Path.Combine(Application.dataPath, "PlayerData.json");
        Debug.Log(filePath);
        // 2. 尝试加载已有存档
        Load();
    }

    /// <summary>
    /// 保存当前 PlayerData 到 JSON 文件
    /// </summary>
    public void Save()
    {
        saveLoadData.gold = playerData.gold;
        saveLoadData.equippedSkillIDs = playerData.equippedSkillIDs;
        saveLoadData.ownedSkillIDs = playerData.ownedSkillIDs;

        string json = JsonUtility.ToJson(saveLoadData, prettyPrint: true);          
        File.WriteAllText(filePath, json);                                        
        Debug.Log("Saved PlayerData to " + filePath);
    }

    /// <summary>
    /// 从 JSON 文件读取 PlayerData；若不存在则创建新数据
    /// </summary>
    public void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            saveLoadData = JsonUtility.FromJson<SaveLoadData>(json);                   
            Debug.Log("Loaded PlayerData from " + filePath);
        }
        else
        {
            // 如果是第一次运行，或文件被删除，则初始化默认数据
            saveLoadData = new SaveLoadData
            {
                gold = 1,
                ownedSkillIDs = new List<string>(),
                equippedSkillIDs = new List<string>(),
            };

            saveLoadData.ownedSkillIDs.Add("Fireball");
            saveLoadData.equippedSkillIDs.Add("Fireball");

            Save(); // 首次保存空数据，确保文件存在
        }

        ChangeDate();
    }

    public void ChangeDate()
    {
        playerData.gold = saveLoadData.gold;
        playerData.ownedSkillIDs = saveLoadData.ownedSkillIDs;
        playerData.equippedSkillIDs = saveLoadData.equippedSkillIDs;
    }

    // 提供给其他脚本访问当前数据
    public PlayerData GetPlayerData() => playerData;
}
