using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[ExecuteAlways]
public class CamTest : MonoBehaviour {
    public List<Transform> camPos;
    public float direction;
    public CinemachineSplineDolly cmCamDolly;

    [SerializeField]
    private float _pos = 0f;

    void Update() {
        _pos = Mathf.Clamp(_pos + direction * Time.deltaTime, 0f, 4f);
        cmCamDolly.CameraPosition = _pos;
        int posInt = Mathf.FloorToInt(_pos);
        float posFloat = _pos - posInt;
        if (posInt < 4) {
            transform.position = Vector3.Lerp(camPos[posInt].position, camPos[posInt + 1].position, posFloat);
        }
        else {
            transform.position = camPos[4].position;
        }
    }
}
