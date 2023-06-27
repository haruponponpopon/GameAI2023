using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] carPrefabs = new GameObject[6];
    [SerializeField] private Text crashText;
    private float[] xlim = new float[] {-150, 150};
    private float[] zlim = new float[] {-100, 100};
    private static int N = 90;
    private GameObject[] cars = new GameObject[N];
    private WheelCollider[][] wheels = new WheelCollider[N][];
    private int[] counts = new int[N];
    private int crashCount;
    private float LIntensity;

    private List<GameObject> carList1 = new List<GameObject>();
    private List<GameObject> carList2 = new List<GameObject>();
    private List<GameObject> carList3 = new List<GameObject>();
    private List<GameObject> carList4 = new List<GameObject>();
    private List<GameObject> carList5 = new List<GameObject>();
    private List<GameObject> carList6 = new List<GameObject>();

    private void Awake()
    {
        Random.InitState(177);
        for (int i = 0; i < N; i++) {
            InstantiateCar(i);
        }
    }

    private void Start()
    {
        crashCount = 0;
        crashText.text = "Crash: " + crashCount.ToString();
        InvokeRepeating("CheckCars", 2.0f, 2.0f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) {
            foreach (GameObject car in carList1) {
                SwitchRenderer(car);
            }
        }
        if (Input.GetKeyDown(KeyCode.F2)) {
            foreach (GameObject car in carList2) {
                SwitchRenderer(car);
            }
        }
        if (Input.GetKeyDown(KeyCode.F3)) {
            foreach (GameObject car in carList3) {
                SwitchRenderer(car);
            }
        }
        if (Input.GetKeyDown(KeyCode.F4)) {
            foreach (GameObject car in carList4) {
                SwitchRenderer(car);
            }
        }
        if (Input.GetKeyDown(KeyCode.F5)) {
            foreach (GameObject car in carList5) {
                SwitchRenderer(car);
            }
        }
        if (Input.GetKeyDown(KeyCode.F6)) {
            foreach (GameObject car in carList6) {
                SwitchRenderer(car);
            }
        }
    }
 
    // ランダムな種類のビークルをランダムな位置に召喚
    private void InstantiateCar(int i)
    {
        // 0以上1未満のランダムな小数
        float x = xlim[0] + (xlim[1] - xlim[0]) * Random.value;
        float y = 1.0f;
        float z = zlim[0] + (zlim[1] - zlim[0]) * Random.value;

        int rnd = Random.Range(0, 6);
        cars[i] = Instantiate(carPrefabs[rnd], new Vector3(x, y, z), Quaternion.identity);
        cars[i].transform.rotation = Quaternion.Euler(0, Random.Range(-180f, 180f), 0);
        wheels[i] = cars[i].GetComponentsInChildren<WheelCollider>();
        TextMeshPro indexText = cars[i].GetComponentInChildren<TextMeshPro>();
        Debug.Assert(indexText is object);
        indexText.text = i.ToString();
        LIntensity = cars[i].GetComponentInChildren<Light>().intensity;

        switch (rnd) {
            case 0:
                carList1.Add(cars[i]);
                break;
            case 1:
                carList2.Add(cars[i]);
                break;
            case 2:
                carList3.Add(cars[i]);
                break;
            case 3:
                carList4.Add(cars[i]);
                break;
            case 4:
                carList5.Add(cars[i]);
                break;
            case 5:
                carList6.Add(cars[i]);
                break;
            default:
                break;
        }
    }

    // ビークルが横転しているかどうかを確認し横転していたら向きを直して復活させる
    private void CheckCars()
    {
        for (int i = 0; i < N; i++) {
            WheelHit[] hits = new WheelHit[2];
            if (wheels[i][0].GetGroundHit(out hits[0]) && wheels[i][1].GetGroundHit(out hits[1])) {
                if (hits[0].collider.gameObject.tag == "Floor" && hits[1].collider.gameObject.tag == "Floor") {
                    counts[i] = 0;
                    continue;
                }
            }
            // Debug.Log(i.ToString() + " 横転");
            counts[i]++;
            if (counts[i] >= 3) {
                counts[i] = 0;
                crashCount++;
                crashText.text = "Crash: " + crashCount.ToString();
                // Debug.Log(i.ToString() + " 復帰");
                Transform carTransform = cars[i].transform;
                float y = carTransform.eulerAngles.y;
                carTransform.rotation = Quaternion.Euler(0, y, 0);
                carTransform.Translate(0.0f, 1.0f, -10.0f);
                // 壁の外に出る奴が稀にいるので対策
                if (xlim[0] < carTransform.position.x && carTransform.position.x < xlim[1] &&
                    zlim[0] < carTransform.position.z && carTransform.position.z < zlim[1]) {
                    return;
                }
                // Debug.Log("はみ出た");
                carTransform.position = Vector3.up;
            }
        }
    }

    // 指定したビークルの見た目を透明にして見えなくする
    private void SwitchRenderer(GameObject car) {
        Renderer[] renderers = car.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            renderer.enabled = !renderer.enabled;
        }
        Light light = car.GetComponentInChildren<Light>();
        light.intensity = (light.intensity > 0) ? 0 : LIntensity;
    }
}
