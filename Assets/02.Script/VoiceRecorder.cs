using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceRecorder : MonoBehaviour {
    AudioClip microphoneInput;
    private string currentMic;
    bool microphoneInitialized=true;
    private float sensitivity=100f;
    //현재 볼륨
    public static float loudness = 0f;

 

    void Awake() {
        StartRecording();
    
    }

    void Update() {
        if (microphoneInitialized) {
            loudness = GetCurrentMicVolume();
        }
        
	}

    public void OnClickUpButton() {
        loudness += 10;
    }
    public void OnClickDownButton() {
        loudness -= 10;
    }
    

    //녹음 시작 함수
    void StartRecording() {
        if (Microphone.devices.Length > 0) {
            Debug.Log("녹음 시작");
            currentMic = Microphone.devices[0];
            microphoneInput = Microphone.Start(Microphone.devices[0] , true , 999 , 44100);
            microphoneInitialized = true;
        }
        while (!(Microphone.GetPosition(currentMic) > 0)) {}
    }
    //녹음 종료 함수
    void StopRecording() {
        Microphone.End(currentMic);
        microphoneInitialized = false;
    }

    //마이크의 볼륨을 가져오는 함수 - 아주 중요(푸리에 변환)
    float GetCurrentMicVolume() {
        int dec = 500;
        float[ ] waveData = new float[dec];
        int micPosition = Microphone.GetPosition(null) - (dec + 1); // null means the first microphone
        microphoneInput.GetData(waveData , micPosition);

        // Getting a peak on the last 128 samples
        float levelMax = 0;
        for (int i = 0 ; i < dec ; i++) {
            float wavePeak = waveData[i] * waveData[i];
            if (levelMax < wavePeak) {
                levelMax = wavePeak;
            }
        }
        float returnValue =Mathf.Abs(levelMax) * sensitivity;
        //Debug.Log(returnValue);
        /*
         if (returnValue >= 5.0f)
             return 5.0f;
         else
         */
        return returnValue;
      
    }
}
