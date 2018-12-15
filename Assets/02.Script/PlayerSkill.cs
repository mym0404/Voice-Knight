using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSkill : MonoBehaviour {

    //스킬 쿨타임 fill 이미지
    public Image[] fill = new Image[3];
    //패시브스킬1
    public GameObject Skill1Before;
    public GameObject Skill1;
    private bool isDuringSkill1=false;
    private float SkillCoolTime1 = 10.0f;
    private bool isDuringActiveTriggerSkill1 = false;
    private GameObject beforeSkill1;
    //스킬2


    //스킬3
    public GameObject Skill3;
    public GameObject Skill3Side;
    private float SkillCoolTime3 = 0f;
    private bool isDuringSkill3 = false;
    private int Skill3Point = 0;
    //private int Skill3Damage = 10;
	void Start () {
        OnClickSkill3();
	}
	
	void Update () {
        //Skill1 Trigger
        if (VoiceRecorder.loudness >= VoiceUICtr.역치0 && VoiceRecorder.loudness < VoiceUICtr.역치1 && isDuringSkill1 == false) {
            isDuringSkill1 = true;
            StartCoroutine(PassiveSkill1());
        } 
        //소리 볼륨이 역치크기를 벗어나면 스킬 초기화
        if(VoiceRecorder.loudness<VoiceUICtr.역치0 || VoiceRecorder.loudness>=VoiceUICtr.역치1){
            isDuringActiveTriggerSkill1 = false;
            if(beforeSkill1!=null)
                Destroy(beforeSkill1);
        }
	}
    
    //자동회복 3초동안 하고있으면 발동
    IEnumerator PassiveSkill1() {
        isDuringActiveTriggerSkill1 = true;

        beforeSkill1 = Instantiate(Skill1Before , transform.position+Vector3.down*0.5f , Quaternion.identity);

        yield return new WaitForSeconds(1.5f);
        if(beforeSkill1!=null)
            Destroy(beforeSkill1);

        //만약 1.5초 후에 조건을 만족시키지 못하면 함수 종료
        if (isDuringActiveTriggerSkill1 == false) {
            isDuringSkill1 = false;
            yield break;
        }

        //조건 만족시 스킬 실행
        
        
        PlayerCtrl.instance.PlayerHp += PlayerInfo.HealPoint; //피 회복
        if (PlayerCtrl.instance.PlayerHp > PlayerInfo.playerMaxHP)
            PlayerCtrl.instance.PlayerHp = PlayerInfo.playerMaxHP;
        HpUICtr.instance.UpdateHpUI(PlayerCtrl.instance.PlayerHp);

        
        GameObject effect = Instantiate(Skill1 , transform.position+Vector3.down*0.5f , Quaternion.identity); //이펙트 생성

        StartCoroutine(SkillCoolTimeCoroutine(0,SkillCoolTime1));

        yield return new WaitForSeconds(1.0f);
        Destroy(effect); //이펙트 제거
        yield return new WaitForSeconds(SkillCoolTime1-1);
        isDuringSkill1 = false; //다시 스킬 트리거가 발생될 수 있게함
        
    }

    //액티브스킬2 트리거
    public void OnClickSkill2() {

    }

    //액티브스킬3 트리거
    public void OnClickSkill3() {
        print("Skill 3");
        if (VoiceUICtr.instance.skillPoint >= Skill3Point && isDuringSkill3==false) { //만약 스킬점수가 10점이상 있다면 발동
            isDuringSkill3 = true; //스킬 쿨타임중 다시 못쓰게하기
            
            VoiceUICtr.instance.skillPoint -= Skill3Point; //스킬점수 차감
            VoiceUICtr.instance.UpdateSkillPointText();
            
            StartCoroutine(SkillCoolTimeCoroutine(2 , SkillCoolTime3)); //쿨타임 돌리기 코루틴 실행


            StartCoroutine(Skill3CooltimeCoroutine());
            StartCoroutine(Skill3PhysicCoroutine());
        }
    }

    IEnumerator Skill3CooltimeCoroutine() {
       
        yield return new WaitForSeconds(SkillCoolTime3); //쿨타임만큼 기다리고
        isDuringSkill3 = false; //다시쓸수 있게하기
    }
    IEnumerator Skill3PhysicCoroutine() {
        Vector3 effectPos = new Vector3(0 , -2.5f , 0); //이펙트가 나오는 장소
        GameObject effect = Instantiate(Skill3 , effectPos , Quaternion.identity);
        GameObject effectSide = Instantiate(Skill3Side , effectPos , Quaternion.identity);
        
        for (int i = 0 ; i < 100 ; i++) {
            Collider[] monsters=Physics.OverlapSphere(effectPos , 5f);

            foreach (Collider coll in monsters) {
                if (coll.tag == "MONSTER") {
                    Vector3 ForceDirection = effectPos - coll.transform.position;

                    coll.gameObject.GetComponent<Rigidbody>().AddForce(ForceDirection * 50);
                }
                
            }

            yield return new WaitForSeconds(0.1f);
        }
        Destroy(effectSide);
        Destroy(effect);
        
    }
    
    
    
    
    
    
    
    
    
    
    
    //스킬 쿨타임 코루틴
    IEnumerator SkillCoolTimeCoroutine(int SkillIndex, float SkillCoolTime) {
        float deltaTime = SkillCoolTime;
        fill[SkillIndex].fillAmount = 1.0f;      
        for (int i = 20 ; i >= 0 ; i--) {   
            fill[SkillIndex].fillAmount = (float)i / 20f;
            yield return new WaitForSeconds(SkillCoolTime/20f);
        }
    }
}
