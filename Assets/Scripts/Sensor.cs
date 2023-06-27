using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SensorInfo {
    public List<WheelCollider> wheels;
    public GameObject sensor;
}
 
public class Sensor : MonoBehaviour
{
    public List<SensorInfo> sensorInfos; 
    public float maxMotorTorque;
    public float sensitivity = 1f; // 光に対するセンサーの感度
    public bool invert; // インバーターの有無
    public float maxSpeed = 5f; // Replace with your max speed
    public bool gaussian; // for Vehicle 4a
    public float mean; // for Vehicle 4a
    public float stdev; // for Vehicle 4a
    public bool discontinuous; // for Vehicle 4b
    public float threshold; // for Vehicle 4b
    private List<GameObject> lights;
    private Rigidbody rb;

    private void Start()
    {
        if (lights == null)
            lights = new List<GameObject>(GameObject.FindGameObjectsWithTag("Light"));
        int a = lights.RemoveAll(light => light.transform.IsChildOf(this.transform));
        Debug.Assert(a == 1);
        rb = this.gameObject.GetComponent<Rigidbody>(); 
    }

    private void FixedUpdate()
    {
        foreach (SensorInfo sensorInfo in sensorInfos) {
            Vector3 position = sensorInfo.sensor.transform.position;
            float x = calculateLightIntensity(position);
            // Debug.Log(new {sensorInfo.sensor, x});
            float motor = 0; // motor torque
            if (gaussian) {
                motor = maxMotorTorque * Mathf.Exp(-(x - mean) * (x - mean) / (2 * stdev * stdev));
            } else if (discontinuous) {
                motor = (x < threshold) ? 0 : maxMotorTorque;
            } else {
                motor = maxMotorTorque * x; // linear
                motor = Mathf.Min(motor, maxMotorTorque);
            }
            if (invert) {
                motor = maxMotorTorque - motor;
            }
            // Debug.Log(new {sensorInfo.sensor, motor});
            foreach (WheelCollider wheel in sensorInfo.wheels) {
                if (motor > 0) {
                    wheel.motorTorque = motor;
                    wheel.brakeTorque = 0;
                } else {
                    wheel.motorTorque = 0;
                    // wheel.brakeTorque = maxMotorTorque * 0.5f;
                }
            }
        }
        if (rb.velocity.magnitude > maxSpeed) {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 pos = this.transform.position;
        if (other.gameObject.name == "Left" || other.gameObject.name == "Right") {
            pos.x *= -0.99f;
        }
        if (other.gameObject.name == "Up" || other.gameObject.name == "Down") {
            pos.z *= -0.99f;
        }
        this.transform.position = pos;
    }

    private float calculateLightIntensity(Vector3 position)
    {
        float intensity = 0;
        foreach (GameObject light in lights)
        {
            Vector3 diff = light.transform.position - position;
            float sqrDistance = diff.sqrMagnitude;
            intensity += 1 / sqrDistance; // 距離の二乗に反比例
        }
        return intensity * sensitivity;
    }
}
