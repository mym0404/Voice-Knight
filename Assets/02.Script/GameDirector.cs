using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour {
    //게임의 진행을 나타내는 변수 = 이것이 false이면 사물도 모두 안움직인다.
    public static bool isProgressing = true;
    public static bool isGaming=true; //게임중인지 나타내는 변수
    public Transform MoonTr;

    #region Singleton
    private static GameDirector instance;
    public static GameDirector Instance {
        get {
            return instance;
        }
    }
    private void SingletonSet() {
        if(instance != null) {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    private void Awake() {
        SingletonSet();
    }

    private void FixedUpdate() {
        if (isProgressing == true && isGaming==true) {
            MoveMoon();
        }
    }

    public static void ChangeProgressing(bool Progressing){
        isProgressing = Progressing;
        if (isProgressing == true) {
            AllParticleEffectPlay();
            
        } else {
            AllParticleEffectStop();
           
        }
        if (GameObject.FindGameObjectWithTag("PLAYER") != null) {
                GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Animator>().SetBool("isProgressing" , Progressing);
        }
    }
    
     //배경을 멈춤
     public static void AllParticleEffectStop() {
        foreach (ParticleSystem ps in GameObject.FindGameObjectWithTag("BACKGROUND").GetComponentsInChildren<ParticleSystem>()) {
            
            ps.Pause(true);
            
        }
    }
    //배경을 재생함
      public static void AllParticleEffectPlay() {
        foreach (ParticleSystem ps in GameObject.FindGameObjectWithTag("BACKGROUND").GetComponentsInChildren<ParticleSystem>()) {
            
            ps.Play(true);
        }
    }


    void MoveMoon() {
        MoonTr.Translate(Vector3.left * 0.01f);

        if (MoonTr.position.x <= -8)
            
            GameEnd();
    }
    void GameEnd() {
        isGaming = false;
    }
}
