using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangeCheck : MonoBehaviour {
    private SpriteRenderer sr;

    private Color firstColor;
    public Color lastColor;
	
    void Start () {
        sr = GetComponent<SpriteRenderer>();

        firstColor = sr.color;
	}

    

    void ColorChange(Color color) {
        StartCoroutine(ColorChangeCoroutine(color));
    }

    IEnumerator ColorChangeCoroutine(Color color) {
        
        
        

        for (int i = 0 ; i < 20 ; i++) {
            sr.color = new Color(Mathf.Lerp(firstColor.r , lastColor.r , (float)i / 20f) , Mathf.Lerp(firstColor.g , lastColor.g , (float)i / 20f)
                , Mathf.Lerp(firstColor.b , lastColor.b , (float)i / 20f) , Mathf.Lerp(firstColor.a , lastColor.a , (float)i / 20f));
            yield return new WaitForSeconds(0.0015f);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 20 ; i >=0 ; i--) {
            sr.color = new Color(Mathf.Lerp(firstColor.r , lastColor.r , (float)i / 20f) , Mathf.Lerp(firstColor.g , lastColor.g , (float)i / 20f)
                , Mathf.Lerp(firstColor.b , lastColor.b , (float)i / 20f) , Mathf.Lerp(firstColor.a , lastColor.a , (float)i / 20f));
            yield return new WaitForSeconds(0.0020f);
        }
        sr.color = firstColor;

        
    }
    
}
