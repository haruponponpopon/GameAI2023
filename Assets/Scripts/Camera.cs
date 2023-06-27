using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera : MonoBehaviour
{
    [SerializeField] private GameObject sunLight;
    [SerializeField] private Text indexText;
    [SerializeField] private InputField inputField;
    private GameObject[] cars;
    private Transform myTransform;
    private int index;
    private bool b_left, b_right, b_up, b_down;

    private void Start()
    {
        if (cars == null)
            cars = GameObject.FindGameObjectsWithTag("Car");
        myTransform = this.transform;
        index = 0;
        ChangeCamera();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) && !b_left) {
            if (myTransform.parent is object)
            {
                if (index > 0) {
                    index--;
                } else {
                    index = cars.Length - 1;
                }
            }
            ChangeCamera();
        }
        if (Input.GetKey(KeyCode.RightArrow) && !b_right) {
            if (myTransform.parent is object)
            {
                if (index < cars.Length - 1) {
                    index++;
                } else {
                    index = 0;
                }
            }
            ChangeCamera();
        }
        if (Input.GetKey (KeyCode.UpArrow) && !b_up) {
            ResetCamera();
        }
        if (Input.GetKey (KeyCode.DownArrow) && !b_down) {
            sunLight.transform.Rotate(180f, 0, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow)) {
            b_left = true;
        } else {
            b_left = false;
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            b_right = true;
        } else {
            b_right = false;
        }
        if (Input.GetKey(KeyCode.UpArrow)) {
            b_up = true;
        } else {
            b_up = false;
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            b_down = true;
        } else {
            b_down = false;
        }
    }

    private void ChangeCamera()
    {
        indexText.text = "Index: " + index.ToString();
        Transform carTransform = cars[index].transform;
        myTransform.SetParent(carTransform);
        myTransform.position = carTransform.position;
        myTransform.rotation = carTransform.rotation;
        myTransform.localPosition = new Vector3(0, 5f, -10f);
        myTransform.Rotate(15f, 0, 0);
    }

    private void ResetCamera()
    {
        myTransform.SetParent(null);
        myTransform.position = new Vector3(0, 200f, 0);
        myTransform.eulerAngles = new Vector3(90f, 0, 0);
    }

    public void OnEndEdit(string text)
    {
        if (text == "") {
            return;
        }
        int tmp = int.Parse(text);
        if (tmp >= 0 && tmp < cars.Length) {
            index = tmp;
            ChangeCamera();
        } else {
            inputField.text = "";
        }
    }
}
