using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puerts;
using Puerts.TSLoader;

namespace PuertsTest
{
    public class HelloWorld : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            JsEnv env = new JsEnv(new TSLoader());
            string str = env.ExecuteModule("main.mts").Get<string>("default");
            Debug.Log(str + " (in C#)");
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}