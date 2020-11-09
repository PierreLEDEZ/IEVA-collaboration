using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WasaaMP;

public class ChestManager : MonoBehaviourPun
{

    public GameObject chest;
    private Vector3 initialPos;
    MonoBehaviourPun player;
    public Vector3[] localPositions = new Vector3[2];

    public TMPro.TextMeshProUGUI interactionText;
    public TMPro.TextMeshProUGUI handle0text;
    public TMPro.TextMeshProUGUI handle1text;

    public GameObject[] handles;
    public bool[] isHandlesTaken = new bool[2];

    public float[] distancesHandles = new float[2];

    public float distanceBetweenHandles;

    // Start is called before the first frame update
    void Start()
    {
        interactionText = GameObject.FindWithTag("ChestText").GetComponent<TMPro.TextMeshProUGUI>();
        handle0text = GameObject.FindWithTag("Handle_0").GetComponent<TMPro.TextMeshProUGUI>();
        handle1text = GameObject.FindWithTag("Handle_1").GetComponent<TMPro.TextMeshProUGUI>();
        chest = GameObject.FindWithTag("Chest").transform.parent.gameObject;
        handles = GameObject.FindGameObjectsWithTag("Handle");
        isHandlesTaken[0] = false;
        isHandlesTaken[1] = false;

        if (chest != null && handles.Length != 0) {
            int i = 0;
            Debug.LogFormat("Initial position chestPrefab: {0}", chest.transform.position);
            initialPos = chest.transform.position;
            foreach (GameObject handle in handles) {
                Debug.LogFormat("Initial position handle {0}: {1}", i, handle.transform.position);
                float dist = Vector3.Distance(chest.transform.position, handle.transform.position);
                Debug.LogFormat("Initial Distance with handle {0}: {1}", i, dist);
                distancesHandles[i] = dist;
                localPositions[i] = handle.transform.localPosition;
                i++;
            }
            distanceBetweenHandles = Vector3.Distance(handles[0].transform.position, handles[1].transform.position);
            Debug.LogFormat("Distance between handles: {0}", distanceBetweenHandles);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (chest != null && handles.Length != 0)
        {
            int i = 0;
            Debug.LogFormat("Position of chest prefab: {0} and {1}", chest.transform.position, chest.transform.localPosition);
            foreach (GameObject handle in handles)
            {
                var comp = handle.GetComponent<Interactive>();
                if (comp.caught)
                {
                    photonView.RPC("HandleTaken", RpcTarget.All, i, true);
                }
                else { photonView.RPC("HandleTaken", RpcTarget.All, i, false); }
                i++;
            }
            i = 0;
            if ((isHandlesTaken[0] && !isHandlesTaken[1]) || (!isHandlesTaken[0] && isHandlesTaken[1]))
            {
                photonView.RPC("changeInteractionText", RpcTarget.All, "One more needed to lift the chest");
            }
            else
            {
                photonView.RPC("changeInteractionText", RpcTarget.All, "");
            }
            if (isHandlesTaken[0] && isHandlesTaken[1])
            {
                foreach (GameObject handle in handles)
                {
                    Debug.LogFormat("Position of handle {0} prefab: {1} and {2}", i, handle.transform.position, handle.transform.localPosition);
                    float dist = Vector3.Distance(chest.transform.position, handle.transform.position);
                    if (distancesHandles[i] != dist)
                    {
                        Debug.LogFormat("Distance with {0}: {1}", i, dist);
                        chest.transform.position = Vector3.Lerp(handles[0].transform.position, handles[1].transform.position, 0.5f);
                        //chest.transform.position = handles[0].transform.position + (handles[1].transform.position - handles[0].transform.position).normalized * distanceBetweenHandles;
                    }
                    i++;
                }
            }
        }
    }

    [PunRPC]
    void HandleTaken(int pos, bool taken) {
        isHandlesTaken[pos] = taken;
        if (pos == 0)
        {
            handle0text.text = "status: " + taken.ToString();
        }
        if (pos == 1)
        {
            handle1text.text = "status: " + taken.ToString();
        }
    }

    [PunRPC]
    void changeInteractionText(string text) {
        interactionText.text = text;
    }
}
