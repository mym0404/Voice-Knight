using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour {
	//변수
    private Animator animator;
    private Transform tr;
    private SpriteRenderer sr;
    //공격 애니메이션
    public AnimationClip attack1;
    public AnimationClip Idle;
    //공격이펙트 생기는 위치
    public GameObject attackPos;
   
    //검기바람
    public GameObject AttackEffect1;
    //다시 공격을 할 수 있는지
    private bool CanAttack = true;

    //공격 역치 세기
    private int attackPower1 = VoiceUICtr.역치1;
    //캐릭터의 체력
    [HideInInspector]
    public int PlayerHp = 100;
    //음향들
    public AudioClip attackSfx1;
    //싱글톤
    public static PlayerCtrl instance = null;
    //피격받는중
    private bool isHitted = false;
    private bool isHittedCanAttack = true;
    //무적게임이펙트
    public GameObject invincibleEffect;
    //깜빡거리기 매터리얼
    public Material hurtMaterial;

    //피격당할시 소리
    public AudioClip hitSound;
    //피격시 데미지 텍스트 UI
    public GameObject DamageUI;
    public Transform DamageUIPos;
    
	void Start () {
        //싱글톤
        instance = this;
        //초기 hp할당
        PlayerHp = PlayerInfo.playerMaxHP;

        //변수할당
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();
    
        animator = GetComponent<Animator>();
	}
	


	void Update () {
        CheckAttack();

      
	}

   
 

    //캐릭터 피격받음
    public void PlayerHurt(int damage) {
        if (isHitted == false) {
             isHitted = true; //공격을 못받는 상태로 바꿈

            //게임의 진행을 멈춤
            GameDirector.ChangeProgressing(false);
            //GameDirector.isProgressing = false;

            //소리 재생
            SoundManager.PlaySfx(hitSound , false);

            //애니메이션 bool 실행
            animator.SetBool("isHitted" , true);

            animator.SetTrigger("isAttacked");

            //데미지를 난수로 산출함
            damage = Random.Range(damage - damage / 5 , damage + damage / 5+1);


            //피변수 깎임
            PlayerHp-=damage;


            //데미지 UI 뜨게하기
            StartCoroutine(DamageUICoroutine(damage));

            
            HpUICtr.instance.UpdateHpUI(PlayerHp);

           
            isHittedCanAttack = false;

            //플레이어의 주긍ㅁ
            if (PlayerHp <= 0) {
                PlayerDie();
                return;
            }

            //깜빡거리기, 무적시간 코루틴
            StartCoroutine(PlayerInvincible());
        
        }
    }
    //플레이어 죽음 함수
    void PlayerDie() {
        animator.SetBool("isHitted" , false);
        animator.SetTrigger("Die");
        
    }
    //무적시간 코루틴
    IEnumerator PlayerInvincible() {
        
        yield return new WaitForSeconds(0.6f);
         
        //애니메이션 바꿔주기
            animator.SetBool("isHitted" , false);

        //다시 게임진행
        GameDirector.ChangeProgressing(true);
        //GameDirector.isProgressing = true;

        //쉴드 생성
        GameObject effect = Instantiate(invincibleEffect , tr.position+Vector3.down*0.5f , Quaternion.identity);
        //깜빡거리는거 실행
        깜빡이 = true;
        StartCoroutine(PlayerHurtImage());

        //피격 후 다시 공격할 수 있게함
        isHittedCanAttack = true;
        

        yield return new WaitForSeconds(3.5f);
        

        깜빡이 = false;
         isHitted = false; //isHitted 변수는 다시 공격 받을 수 있는지 여부이다.
        Destroy(effect);
    }
    //깜빡거리기 코루틴
    private bool 깜빡이 = false;
    IEnumerator PlayerHurtImage() {
        Material beforeMaterial = sr.material;
        while (깜빡이==true) {
            sr.material = hurtMaterial;
            yield return new WaitForSeconds(0.1f);
            sr.material = beforeMaterial;
            yield return new WaitForSeconds(0.1f);
        }
    }
    //데미지 뜨고 없어지는 코루틴 함수
    IEnumerator DamageUICoroutine(int Damage) {


        GameObject ui = Instantiate(DamageUI ,new Vector3(-3000,-3000,0),Quaternion.identity,GameObject.Find("Canvas").GetComponent<RectTransform>());
        ui.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main ,DamageUIPos.transform.position);
        
        ui.GetComponentInChildren<Text>().text = "-"+Damage.ToString();

        RectTransform rectTr = ui.GetComponent<RectTransform>();
        

        

        for (int i = 0 ; i < 30 ; i++) {
            
            if (ui == null)
                break;
            rectTr.Translate(Vector3.up * 1f);
           
            
            
        

            yield return new WaitForSeconds(0.02f);
        }
        
        Destroy(ui);
    }



     //공격감지
    void CheckAttack() {
        //공격 1
        if ((Input.GetMouseButtonDown(1)|| VoiceRecorder.loudness >= attackPower1) && CanAttack == true &&CanvasMgr.isUIClicking==false &&
            isHittedCanAttack==true ) {
            GameDirector.ChangeProgressing(false);
            //GameDirector.isProgressing = false; //게임의 진행 멈춤
            
            CanAttack = false;
            StartCoroutine(CanAttackAgain(attack1.length));
            Attack1();
            
        }
    }
    //공격1
    void Attack1() {
        
        animator.SetTrigger("Attack");
        
        Vector3 attackDirection = TargetDirection.instance.attackDirection;
      
        if (attackDirection == Vector3.zero)
            attackDirection = Vector3.right;

        
        
        StartCoroutine(AttackFly(attackDirection));
        
    }
    //공격이 날아가는 코루틴함수
    IEnumerator AttackFly(Vector3 attackDirection) {
        yield return new WaitForSeconds(0.35f);
        //이펙트 생성
        GameObject attack = Instantiate(AttackEffect1 , attackPos.transform.position ,Quaternion.FromToRotation(Vector3.zero,attackDirection));
        //공격 사운드 재생
        SoundManager.PlaySfx(attackSfx1 , false);
        
        Transform tr = attack.transform;
        attack.GetComponent<Rigidbody>().AddForceAtPosition(attackDirection * 500f , attackPos.transform.position);
        
        yield return new WaitForSeconds(3f);
        Destroy(attack);
    }

    
    //공격 도중 실행되는 코루틴
    IEnumerator CanAttackAgain(float attackLength) {
        

        yield return new WaitForSeconds(attackLength); //공격 애니메이션 끝날 때 까지 기다린다.
        yield return new WaitForSeconds(Idle.length);
        yield return new WaitForSeconds(0.3f);
       //yield return new WaitForSeconds(1.0f); //공격에 딜레이를 조금 더준다 Idle Animation
        

       
        CanAttack = true;

        GameDirector.ChangeProgressing(true);
        //GameDirector.isProgressing = true; //게임의 진행 실행함
    }

    //스킬1
}
