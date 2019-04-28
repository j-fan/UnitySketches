using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCReceive : MonoBehaviour
{
    // Start is called before the first frame update
    public OSC osc;
    void Start()
    {
        osc.SetAddressHandler("/midi/mixtrack_pro_3/0/3/control_change", OnGetMessage);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGetMessage(OscMessage m){
        print(m);
    }
}
