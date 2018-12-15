using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpUICtr : MonoBehaviour {
    public static HpUICtr instance = null;
    public Text hpText;
    public Image hpBar;

    float lastFillAmount;



	void Start () {

        hpText.text = PlayerInfo.playerMaxHP.ToString();
        instance = this;
	}
	
	void Update () {
        
	}

    
    public void UpdateHpUI(int currHp) {
        


        lastFillAmount = (float)currHp/(float)PlayerInfo.playerMaxHP;

        //만약 피가 줄어드는 로직이면
        if (lastFillAmount < hpBar.fillAmount) {
            StartCoroutine(hpBarDecrese());
        } else if (lastFillAmount > hpBar.fillAmount) { //피가 늘어나는 로직이면
            StartCoroutine(hpBarIncrease());
        }
        if (currHp <= 0) {
            hpText.text = 0.ToString();

        } else
            hpText.text = currHp.ToString();
    }

    
     //체력바 감소
    IEnumerator hpBarDecrese() {
        
        
        float firstFillAmount = hpBar.fillAmount;
        
       
        //헤롱헤롱이 1.5초
        while(hpBar.fillAmount>=lastFillAmount) {
            hpBar.fillAmount = firstFillAmount;
            firstFillAmount -= 0.01f;

         
            yield return new WaitForSeconds(0.01f);
        }

       
    }
     //체력바 증가
    IEnumerator hpBarIncrease() {
        
        
        float firstFillAmount = hpBar.fillAmount;
        
       
        //헤롱헤롱이 1.5초
        while(hpBar.fillAmount<=lastFillAmount) {
            hpBar.fillAmount = firstFillAmount;
            firstFillAmount += 0.01f;

         
            yield return new WaitForSeconds(0.01f);
        }

       
    }
    
}
