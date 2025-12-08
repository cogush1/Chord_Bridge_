using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmVisualizer : MonoBehaviour
{
    [Header("연결할 것")]
    public Transform beatMover; // 움직일 큰 동그라미
    public Image guideImage;    // 기준 동그라미 (선택사항)

    [Header("설정")]
    public float startScale = 0f;   // 시작 크기 (0에서 시작해서 커짐)
    public float endScale = 1.0f;   // 끝 크기

    void Update()
    {
        if (VocalManager.instance == null) return;

        // 1. 현재 상황에 맞는 음악 소스와 BPM 찾기 (VocalManager 로직과 똑같이!)
        AudioSource currentSource;
        float currentBpm;

        if (VocalManager.instance.canUseVocal)
        {
            // 보컬 존
            currentSource = VocalManager.instance.bgmAudioSource;
            currentBpm = VocalManager.instance.vocalBpm;
        }
        else if (VocalManager.instance.isChordZone)
        {
            // [추가됨] 코드 존
            currentSource = VocalManager.instance.chordBgmSource;
            currentBpm = VocalManager.instance.chordBpm;
        }
        else
        {
            // 일반 구역 (Base)
            currentSource = VocalManager.instance.baseBgmSource;
            currentBpm = VocalManager.instance.baseBpm;
        }

        // 2. 음악이 없거나 안 나오면 숨기기
        // (안전장치: Source가 없거나 재생 중이 아니면 비활성화)
        if (currentSource == null || !currentSource.isPlaying)
        {
            if (beatMover.gameObject.activeSelf) beatMover.gameObject.SetActive(false);
            return;
        }
        
        // 음악 나오면 켜기
        if (!beatMover.gameObject.activeSelf) beatMover.gameObject.SetActive(true);

        // 3. 박자 진행률 계산
        float secPerBeat = 60f / currentBpm;
        float songTime = currentSource.time;
        float timer = songTime % secPerBeat;
        float progress = timer / secPerBeat;

        // 4. 크기 조절 (Lerp)
        float currentScale = Mathf.Lerp(startScale, endScale, progress);
        beatMover.localScale = new Vector3(currentScale, currentScale, 1);

        // 5. 시각 효과 (박자 맞을 때 반짝)
        if (guideImage != null)
        {
            if (progress < 0.1f) guideImage.color = new Color(1, 1, 1, 1f); // 반짝!
            else guideImage.color = new Color(1, 1, 1, 0.5f); // 평소
        }
    }
}