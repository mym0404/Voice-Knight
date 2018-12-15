using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class CanvasMgr : MonoBehaviour,IPointerDownHandler,IPointerUpHandler {
	public static bool isUIClicking = false;
	void Start () {
		
	}
	
	void Update () {
        if (Input.GetMouseButtonDown(0) && isUIClicking == false) {
            GameObject.FindGameObjectWithTag("TARGET").transform.position = Input.mousePosition;
        }
        if (Input.GetMouseButton(0) && isUIClicking == false) {
            GameObject.FindGameObjectWithTag("TARGET").transform.position = Input.mousePosition;
        }
	}

    public void OnPointerDown(PointerEventData eventData) {
        Debug.Log(eventData.pressPosition);

        isUIClicking = true;
      
    
    }

    public void OnPointerUp(PointerEventData eventData) {
        Debug.Log("OnPointerUp");
        isUIClicking = false;
    }
}
