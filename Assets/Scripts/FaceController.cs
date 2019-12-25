using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class FaceController : MonoBehaviour {
    public Face[] faces;

    private int _currentIndex = 0;
    private List<AugmentedFace> _augmentedFaces = new List<AugmentedFace>();
    private bool _isQuitting = false;

    private void Start() {
        SwitchFace(6);
    }

    private void Update() {
        UpdateApplicationLifecycle();

        Session.GetTrackables<AugmentedFace>(_augmentedFaces, TrackableQueryFilter.All);

        if(_augmentedFaces.Count == 0) {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            faces[_currentIndex].gameObject.SetActive(false);
        }
        else {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            faces[_currentIndex].gameObject.SetActive(true);
        }
    }

    public void SwitchFace(int j) {
        for (int i = 0; i < faces.Length; i++) {
            faces[i].gameObject.SetActive(i == j ? true : false);
            _currentIndex = j;
        }
    }

    private void UpdateApplicationLifecycle() {
        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }

        if (_isQuitting) {
            return;
        }

        if (Session.Status == SessionStatus.ErrorPermissionNotGranted) {
            ShowAndroidToastMessage("Camera permission is needed to run this application.");
            _isQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
        else if (Session.Status.IsError()) {
            ShowAndroidToastMessage(
                "ARCore encountered a problem connecting.  Please start the app again.");
            _isQuitting = true;
            Invoke("_DoQuit", 0.5f);
        }
    }

    private void DoQuit() {
        Application.Quit();
    }

    private void ShowAndroidToastMessage(string message) {
        AndroidJavaClass unityPlayer =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity =
            unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null) {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>(
                    "makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
    }
}
