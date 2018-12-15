using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour {
    public Text text1;
    public Image image1;
	void Start () {
        StartCoroutine(TextFadeOut());
	}

    IEnumerator TextFadeOut() {
        yield return new WaitForSeconds(1.0f);

        float alpha = 255f/255f;
        while (alpha >0f) {
            yield return new WaitForSeconds(0.01f);
            alpha -= 0.05f;
            text1.color = new Color(1 , 1 , 1 , alpha);
           // image1.color = new Color(1 , 1 , 1 , alpha);
        }
        text1.text = "Use headphone for best control";
        yield return new WaitForSeconds(0.5f);
        
        while (alpha <= 1.0f) {
            yield return new WaitForSeconds(0.01f);
            alpha += 0.015f;
            text1.color = new Color(1 , 1 , 1 , alpha);
            image1.color = new Color(1 , 1 , 1 , alpha);
        }
        yield return new WaitForSeconds(2.0f);
        while (alpha >0f) {
            yield return new WaitForSeconds(0.01f);
            alpha -= 0.05f;
            text1.color = new Color(1 , 1 , 1 , alpha);
            image1.color = new Color(1 , 1 , 1 , alpha);
        }
        SceneManager.LoadScene("scPlay");

    }

	void Update () {
		
	}
}
