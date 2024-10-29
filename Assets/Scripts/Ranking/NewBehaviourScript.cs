using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private RankingService service;

    // Start is called before the first frame update
    void Start()
    {
        service = new RankingService();
        service.ShowRanking(190);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
