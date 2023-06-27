using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightGenerator : MonoBehaviour
{
    [SerializeField] private GameObject lightPrefab;
    private float[] xlim = new float[] {-150, 150};
    private float[] zlim = new float[] {-100, 100};
    private int N = 100;

    void Awake()
    {
        Random.InitState(49);
        for (int i = 0; i < N; i++) {
            // 0以上1未満のランダムな小数
            float x = xlim[0] + (xlim[1] - xlim[0]) * Random.value;
            float y = 1.0f;
            float z = zlim[0] + (zlim[1] - zlim[0]) * Random.value;
            Instantiate(lightPrefab, new Vector3(x, y, z), Quaternion.identity);
        }
    }
}
