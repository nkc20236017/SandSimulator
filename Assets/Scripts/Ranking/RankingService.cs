using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RankingService : IRanking
{
    private List<int> rank = new List<int>();
    private string fileName = "Data.json";
    private string filepath;

    private RankingData data;

    private void Initialize()
    {
        data = new RankingData();
        filepath = Application.persistentDataPath + "/" + fileName;

        if (!File.Exists(filepath))
        {
            Save(data);
        }

        rank = Load(filepath).Ranks;

    }

    public void ShowRanking(int money)
    {
        Initialize();

        rank.Add(money);
        rank = rank.OrderByDescending(_ => _).ToList();   

        for (int i = 0; i < rank.Count; i++)
        {
            Debug.Log(rank[i] + "‡ˆÊ"+i+1);
        }

        var rankData = new RankingData();
        rankData.Ranks = rank;
        Save(rankData);
    }

    private void Save(RankingData data)
    {
        string jsonData = JsonUtility.ToJson(data);
        StreamWriter streamWriter = new StreamWriter(path:filepath,false);
        streamWriter.WriteLine(jsonData);
        streamWriter.Close();
    }

    private RankingData Load(string path)
    {
        rank.Clear();
        StreamReader rb = new StreamReader(path);
        string jsonData = rb.ReadToEnd();
        rb.Close();

        return JsonUtility.FromJson<RankingData>(jsonData);
    }

    public RankingData GetRank()
    {
        return Load(filepath);
    }

    public void DleteSaveData()
    {
        rank.Clear();
        Save(new RankingData());
    }

}
