using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackInfo {
    [HideInInspector]
    public int Damage=PlayerInfo.Attack1Damage;
    public GameObject HitEffect;
}

public class AttackCtrl : MonoBehaviour {
    Transform tr;

    public AttackInfo info;
    

	void Start () {
        tr = GetComponent<Transform>();
        info.Damage = PlayerInfo.Attack1Damage;

        SetSortingLayerRecursive(gameObject, "Effect");
	}
	
	void Update () {
		
	}

    private void SetSortingLayerRecursive(GameObject obj,string layerName) {

        for(int i = 0; i < obj.transform.childCount; i++) {
            SetSortingLayerRecursive(obj.transform.GetChild(i).gameObject, layerName);
        }

        var render = obj.GetComponent<ParticleSystemRenderer>();
        if(render != null) {
            render.sortingLayerName = layerName;
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "MONSTER" && other.gameObject.GetComponent<MonsterCtrl>().canHitted==true) {
             //이펙트들
            Vector3 effectPos = (other.gameObject.transform.position + tr.position)/2;

            Destroy(Instantiate(info.HitEffect , effectPos , Quaternion.identity),2.0f);
        }
    }
}
