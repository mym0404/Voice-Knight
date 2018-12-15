using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MonsterInfo{
     //정보들
    public int Damage = 0;
    public float deltaJumpTime = 0;
    public int MonsterHP = 0;
    public float MonsterMoveSpeed = 0;
    public float MonsterJumpSpeed = 0;
    public float MonsterAttackDeltaTime = 0;
    public AudioClip attackSound = null;
    public AudioClip monsterDieSound = null;
    public GameObject dieEffect = null;
}

public class MonsterCtrl : MonoBehaviour {

    
    public MonsterInfo info;
    
    
    private Rigidbody rBody;
    private SpriteRenderer sr;
    private Transform tr;

    //헤롱헤롱 게임오브젝트
    private GameObject DizzyEffect;
    private float dizzyTime = 1.5f;
    public Sprite DizzyEye;
    //몬스터체력
    private int monsterHPMAX; 
    private int monsterCurrHP;
    //몬스터 체력바 UI
    public GameObject HpBar;
    public Transform HpBarPos;
    private GameObject hpBarUI;
    //몬스터에게 뜨는 데미지 UI
    public Transform DamageTextPos;
    public GameObject DamageUI;
    public GameObject CriticalDamageUI;
    
    //피격음
    public AudioClip hitSound;
    //공격을 받을 수 있는 상태
    public bool canHitted = true;
    //몬스터 스피드
    private float monsterSpeed = 1.0f;
    //몬스터 공격색상
    private Color color = Color.red;

    //몬스터가 공격하고 있는 상태
    private bool Attacking = false;
    //몬스터가 헤롱헤롱 하고있는 상태
    private bool Dizzing = false;
    //몬스터의 생사
    private bool isAlive = true;
    

	void Start () {
        rBody = GetComponent<Rigidbody>();
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();

        DizzyEffect = gameObject.GetComponentInChildren<ParticleSystem>().gameObject;
        DizzyEffect.SetActive(false);

        //몬스터 이동
        StartCoroutine(MonsterMoveCoroutine());

        //몬스터 초기 hp 설정
        monsterHPMAX= info.MonsterHP;
        monsterCurrHP = monsterHPMAX;
        //몬스터 hp바 생성
        
        hpBarUI = Instantiate(HpBar , RectTransformUtility.WorldToScreenPoint(Camera.main , HpBarPos.position),Quaternion.identity
            ,GameObject.Find("Canvas").GetComponent<RectTransform>());
	}
	

	void Update () {
       
        //hpBar UI가 몬스터를 따라다니게함
        hpBarUI.transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main , HpBarPos.position);

       
	}
    //몬스터이동함수

    IEnumerator MonsterMoveCoroutine() {
        while (true) {
            yield return new WaitForSeconds(0.2f);
            while (Dizzing==false && Attacking==false) {
                MonsterJump();
                yield return new WaitForSeconds(info.deltaJumpTime);
            }
        }
    }

    void MonsterJump() {
        if (isAlive == false)
            return;

        rBody.AddForce(Vector3.left * info.MonsterMoveSpeed);
        rBody.AddForce(Vector3.up * info.MonsterJumpSpeed);
    
    }

    //몬스터 공격개시
    private void OnCollisionEnter(Collision collision) {
        //벽에 닿았을때 공격개시
        if (collision.gameObject.tag == "WALL") {
            
            MonsterAttack();
        }
    }
    //몬스터 공격함수
    void MonsterAttack() {
        Attacking = true;
        
        StartCoroutine(StartMonsterAttack());
    }
    //몬스터가 주기마다 공격을 하게 해주는 함수
    IEnumerator StartMonsterAttack() {
     
        while (true) {
            if (isAlive == false)
                yield break;
      
            if (Attacking == false)
                break;

            //공격사운드 재생
            SoundManager.PlaySfx(info.attackSound , false);

            //공격할때마다 특정부위의 색이 변하게 만들기
            yield return new WaitForSeconds(0.5f);
            foreach (ColorChangeCheck child in GetComponentsInChildren<ColorChangeCheck>()) {
                child.gameObject.SendMessage("ColorChange",color);

            }
            yield return new WaitForSeconds(1.0f);
          
        
            if (Attacking == false)
                break;
            PlayerCtrl.instance.PlayerHurt(info.Damage);
            yield return new WaitForSeconds(info.MonsterAttackDeltaTime);
        }
    }



    //몬스터 피격함수
    private void OnTriggerEnter(Collider other) {
        if (isAlive == false)
            return;

        if (other.gameObject.tag == "ATTACK" && canHitted == true) {//만약 몬스터가 공격을 맞았다면
            bool isCritical = false;
            
            //몬스터의 데미지를 산출해주는 로직
            float Damagef = other.gameObject.GetComponent<AttackCtrl>().info.Damage;
            Damagef = Random.Range(Damagef - Damagef / 5f , Damagef + Damagef / 5f);
            int Damage = Mathf.RoundToInt(Damagef);

            //크리티컬 받았는지 산출해주는 로직
            if (Random.Range(0f , 100f) < PlayerInfo.CriticalPercent) {//만약크리티컬이라면
                isCritical = true;
                Damage *= 2;
            }
            //넉백
            rBody.AddForce(Vector3.right * 150);
            //데미지 텍스트 띄우기
            StartCoroutine(DamageUICoroutine(Damage,isCritical));

            //공격중 해제
            Attacking = false;
            //피 깎이는 로직
            StartCoroutine(hpBarDecrese(Damage));
            //공격못받게하기
            canHitted = false;
            //공격 이펙트 없애기
            foreach (Renderer R in other.gameObject.GetComponentsInChildren<Renderer>()) {
                R.enabled = false;
            }
            other.GetComponent<Collider>().enabled = false;

            //사운드 재생하기
            SoundManager.PlaySfx(hitSound , false);
            //헤롱헤롱
            StartCoroutine(MonsterDizzy());
        }
      

    }
    //데미지 뜨고 없어지는 코루틴 함수
    IEnumerator DamageUICoroutine(int Damage,bool isCritical) {
        if (isAlive == false)
            yield break;
        
        GameObject DamageUILocal;
        
        if (isCritical == true)
            DamageUILocal = Instantiate(CriticalDamageUI , new Vector2(-3000 , -3000),Quaternion.identity
            ,GameObject.Find("Canvas").GetComponent<RectTransform>());
        else
         DamageUILocal = Instantiate(DamageUI , RectTransformUtility.WorldToScreenPoint(Camera.main , DamageTextPos.position),Quaternion.identity
            ,GameObject.Find("Canvas").GetComponent<RectTransform>());

        DamageUILocal.GetComponent<RectTransform>().position = RectTransformUtility.WorldToScreenPoint(Camera.main , DamageTextPos.position);

        if (Damage > 5)
            DamageUILocal.GetComponentInChildren<Text>().text = Damage.ToString();
        else {
            if (Random.Range(0 , 6) <= 1)
                DamageUILocal.GetComponentInChildren<Text>().text = "MISS";
            else
                DamageUILocal.GetComponentInChildren<Text>().text = Damage.ToString();
        
            isCritical = false;
        }

        Destroy(DamageUILocal , 0.6f);

        RectTransform rectTr = DamageUILocal.GetComponent<RectTransform>();

        float alpha = 1.0f;

        for (int i = 0 ; i < 30 ; i++) {
            if (DamageUILocal != null || rectTr!=null) {
                

                rectTr.Translate(Vector3.up * 2f);
                DamageUILocal.GetComponentInChildren<Text>().color = new Color(1 , 1 , 1 , alpha);
                if (isCritical)
                    DamageUILocal.GetComponent<Image>().color = new Color(1 , 1 , 1 , alpha);

                alpha -= 0.03f;

                yield return new WaitForSeconds(0.02f);
            }
        }
        
        
    }
    
    //몬스터 헤롱헤롱
    IEnumerator MonsterDizzy() {
        Dizzing = true;
        DizzyEffect.SetActive(true);

        SpriteRenderer sr = GetComponentInChildren<eyeCheck>().gameObject.GetComponent<SpriteRenderer>();
        Sprite beforeEye = sr.sprite;
        sr.sprite = DizzyEye;
        yield return new WaitForSeconds(dizzyTime);
        Dizzing = false;
        if (isAlive == false)
            yield break;

        sr.sprite = beforeEye;
        DizzyEffect.SetActive(false);
        canHitted = true;
    }
    //스킬3 ---------------------------------------
    private int Skill3Damage = 5;
    private bool Skill3Damaging = false;
    
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "SKILL3" && Skill3Damaging==false) {
            Skill3Damaging = true;
            StartCoroutine(스킬3데미지());
            Skill3Function(Skill3Damage);
            Attacking = false;//공격못하게함
        }
    }
    IEnumerator 스킬3데미지() {
        yield return new WaitForSeconds(0.1f);
        
        Skill3Damaging = false;
    }

    public void Skill3Function(int damage3) {
        damage3 = Random.Range(damage3 - 2 , damage3 + 3);
        StartCoroutine(DamageUICoroutine(damage3 , true));
        StartCoroutine(hpBarDecrese(damage3));
    }
    // 스킬3 로직 끝-------------------------------------



    //몬스터 체력바 서서히 주는 함수
    public IEnumerator hpBarDecrese(int damage) {
        if (isAlive == false)
            yield break;

        Image fillImage;
        fillImage = hpBarUI.GetComponentInChildren<iamchild>().gameObject.GetComponent<Image>();
        float deltaTime = dizzyTime / damage;
        //헤롱헤롱이 1.5초
        for(int i=0 ;i<damage ;i+=1) {
            monsterCurrHP -= 1;

            fillImage.fillAmount = (float)monsterCurrHP / (float)monsterHPMAX;
            yield return new WaitForSeconds(deltaTime/10f);

            if (monsterCurrHP <= 0) {
                MonsterDie();
                break;
            }
        }


        Debug.Log("몬스터의 현재 체력 = " + monsterCurrHP.ToString());
    }
    //몬스터 죽기 함수
    void MonsterDie() {
        isAlive = false;
        //눈바꿔주기
        SpriteRenderer sr = GetComponentInChildren<eyeCheck>().gameObject.GetComponent<SpriteRenderer>();
        sr.sprite = DizzyEye;
        //몬스터의 리기드바드 비활성화
         


        SoundManager.PlaySfx(info.monsterDieSound , false);
        StartCoroutine(MonsterDieCoroutine());
    }
    IEnumerator MonsterDieCoroutine() {

        GameObject effect = Instantiate(info.dieEffect , tr.position , Quaternion.identity);

        yield return new WaitForSeconds(1.5f);
        
        Destroy(this.gameObject);
        Destroy(hpBarUI);
        
        /*if (DamageUILocal != null)
            Destroy(DamageUILocal);
            */
    }
   

}
