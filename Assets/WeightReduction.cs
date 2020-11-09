using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WasaaMP;

public class WeightReduction : MonoBehaviourPun
{
    bool power = false;
    MonoBehaviourPun player;
    public Camera cam;
    public float xRotation = 0f;
    public float interactionDistance;
    public TMPro.TextMeshProUGUI interactionText;

    // Start is called before the first frame update
    void Start()
    {
        interactionDistance = 4;
        player = (MonoBehaviourPun)this.GetComponentInParent(typeof(Navigation));
        cam = (Camera)GameObject.FindObjectOfType(typeof(Camera));
        interactionText = (TMPro.TextMeshProUGUI)GameObject.FindGameObjectWithTag("PowerText").GetComponent<TMPro.TextMeshProUGUI>();
        interactionText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (player.photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                power = true;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                power = false;
            }

            if (power)
            {
                interactionText.text = "Power activated";
                GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
                Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
                RaycastHit hit;

                bool successfulHit = false;

                if (Physics.Raycast(ray, out hit, interactionDistance))
                {
                    if (hit.collider.name == "KyleSimple")
                    {
                        successfulHit = true;
                        foreach (GameObject cube in cubes)
                        {
                            photonView.RPC("changeGravity", RpcTarget.All, false);
                        }
                    }
                }
                else
                {
                    foreach (GameObject cube in cubes)
                    {
                        successfulHit = false;
                        photonView.RPC("changeGravity", RpcTarget.All, true);
                    }
                }
            } else { interactionText.text = ""; }
        }
    }

    [PunRPC]
    void changeGravity(bool gravity)
    {
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach(GameObject cube in cubes)
        {
            cube.GetComponent<Rigidbody>().useGravity = gravity;
            cube.GetComponent<Rigidbody>().isKinematic = !gravity;
        }
    }
}
