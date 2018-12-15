using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterGenerator : MonoBehaviour {

    public GameObject[ ] monsterDictionary = new GameObject[2];

    //private int MonsterZaxisIndex = 0;

	void Start () {
        StartCoroutine(GenerateTrrigerCoroutine());
	}
	
	void Update () {
		
	}
    IEnumerator GenerateTrrigerCoroutine() {
        while (true) {
            GenerateMonster(Random.Range(0, monsterDictionary.Length));
            yield return new WaitForSeconds(3.0f);

        }

    }
    void GenerateMonster(int MonsterIndex) {
        Instantiate(monsterDictionary[MonsterIndex] , new Vector3(Random.Range(13f,17f) , 3 , 0),Quaternion.identity);
        /*
        MonsterZaxisIndex += 5;
        if (MonsterZaxisIndex == 1000)
            MonsterZaxisIndex = 0;
            */
    }
}
