using UnityEngine;

public class BlinkingPlatform : MonoBehaviour
{
    [Header("설정")]
    public bool startHidden = false; // 처음 시작할 때 꺼져있을지 여부 (박자 엇갈리게 할 때 사용)
    public float offsetTime = 0f;    // 타이밍 미세 조절용

    private BoxCollider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 매니저가 없거나 ChordZone이 아니면 그냥 켜두기 (안전장치)
        if (VocalManager.instance == null || !VocalManager.instance.isChordZone)
        {
            SetPlatform(true);
            return;
        }

        // 1. 현재 Chord Zone의 BPM 가져오기
        float bpm = VocalManager.instance.chordBpm;
        if (bpm <= 0) return;

        // 2. 1박자의 시간 계산
        float secPerBeat = 60f / bpm;

        // 3. 타이머 계산 (노래 시간 + 오프셋)
        float time = VocalManager.instance.chordBgmSource.time + offsetTime;

        // 4. 박자 계산 (2박자 주기로 반복: 1박자 ON, 1박자 OFF)
        // time % (2 * secPerBeat) -> 0 ~ 2박자 사이의 시간
        float cycle = time % (secPerBeat * 2);

        bool isOn;

        if (!startHidden)
        {
            // [패턴 A] 앞쪽 1박자 동안 ON
            isOn = cycle < secPerBeat;
        }
        else
        {
            // [패턴 B] 뒤쪽 1박자 동안 ON (엇박자)
            isOn = cycle >= secPerBeat;
        }

        SetPlatform(isOn);
    }

    void SetPlatform(bool active)
    {
        if (col) col.enabled = active;
        
        // 투명도 조절로 깜빡임 표현 (완전히 끄면 안 보이니까 반투명하게)
        if (sr) 
        {
            Color c = sr.color;
            c.a = active ? 1f : 0.3f; // 켜지면 선명하게, 꺼지면 흐릿하게
            sr.color = c;
        }
    }
}