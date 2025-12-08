using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float lifeTime = 2.0f; // 발판이 유지되는 시간 (초)

    void Start()
    {
        // 태어난 지 lifeTime 초 뒤에 파괴
        Destroy(gameObject, lifeTime);
    }
}
