using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetDirection : MonoBehaviour {
    private bool isDraging = false;
    private RectTransform tr;
    
    //공격방향
    [HideInInspector]
    public Vector3 attackDirection=Vector3.zero;
    //공격이 나가는 포지션
    public GameObject attackPos;
    //싱글톤
    public static TargetDirection instance=null;

	void Start () {
        tr = GetComponent<RectTransform>();

        //싱글톤할당
        instance = this;
	}
	
	void Update () {
        Vector3 targetPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(tr , tr.position , Camera.main , out targetPoint);

        attackDirection = (targetPoint - attackPos.transform.position).normalized;
        
	}

    public void OnDragDown() {
        isDraging = true;    
    }

    public void OnDrag() {
        if (isDraging == true) {
            tr.position = Input.mousePosition;
        }
    }

    public void OnDragUp() {
        isDraging = false;
    }

}
