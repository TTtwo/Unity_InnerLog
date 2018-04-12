using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowMsg_demo : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Debuger.Log("show msg");
        Debuger.LogError("erroneous");
        Debug.LogWarning("waring");
    }

}
