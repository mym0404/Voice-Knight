using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceUICtr : MonoBehaviour {
    //스킬포인트가 올라갈 수 있는지
    public static VoiceUICtr instance = null;
        [HideInInspector]
    public int skillPoint = 0;
    private bool CanGetSkillPoint = true;
    public Text skillPointText;

    public Image fillImage;

    public Text VolumeText;

    
    public Color[ ] voiceBarColor = new Color[3];

    public static int 역치0 = 5;
    public static int 역치1 = 50;
    public static int 역치2 = 95;

	void Start () {
        instance = this;
	}
	
	void Update () {
        CheckVolume();
        CheckGetSkillPoint();
	}

    void CheckVolume() {
        VolumeText.text = VoiceRecorder.loudness.ToString("0");
        fillImage.fillAmount = VoiceRecorder.loudness / 100f;

        if (VoiceRecorder.loudness >= 0 && VoiceRecorder.loudness < 역치1)
            fillImage.color = voiceBarColor[0];
        else if (VoiceRecorder.loudness >= 역치1 && VoiceRecorder.loudness < 역치2)
            fillImage.color = voiceBarColor[1];
        else
            fillImage.color = voiceBarColor[2];
    }

    //스킬 포인트 상승
    void CheckGetSkillPoint() {
        if (VoiceRecorder.loudness >= 역치2 && CanGetSkillPoint==true) {
            CanGetSkillPoint = false;
            
            if (skillPoint < 15) {
                skillPoint += 1;
                StartCoroutine(SkillPointCoolTimeCoroutine());
                skillPointCoolTime.fillAmount = 0.0f;
            }
            UpdateSkillPointText();
        }
    }

    

  
    //스킬포인트 업데이트
    public void UpdateSkillPointText() {
        skillPointText.text = skillPoint.ToString();
    }

    public Image skillPointCoolTime;
    IEnumerator SkillPointCoolTimeCoroutine() {
        float fill = 0.0f;


        while (fill<1.0f) {
                 fill += 0.01f;
            skillPointCoolTime.fillAmount = fill;
          
            yield return new WaitForSecondsRealtime(0.05f);
          
        }
     CanGetSkillPoint = true;


       
    }

    
}
