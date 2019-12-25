using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleRewardsWrapper : MonoBehaviour {
    private AndroidJavaObject javaClass;

    private void Start() {
        javaClass = new AndroidJavaObject("com.unknown.testlib.TestClass");
        
    }

    private void Update() {
        javaClass.Call("LogAndroidMessage");
    }
}
