using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
public class FlicToUnity : MonoBehaviour
{
    private AndroidJavaObject Flic;
    private bool flicCalled = false;
    private bool permCheckCalled = false;
    public GameObject dialog = null;

    public void shoot()
    {
        Debug.Log("Pew pew pew - shooting");
    }

    // Start is called before the first frame update
    void Start()
    {

#if PLATFORM_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.Log("Permission check failed");

                Permission.RequestUserPermission(Permission.FineLocation);
                dialog = new GameObject();
            } else {
            Debug.Log("Permission check passed ");
            startFlic();
            }
#endif
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnGUI()
    {
        if (!permCheckCalled)
        {
#if PLATFORM_ANDROID
                Debug.Log("Initializing permission check");
                Debug.Log("Permission.HasUserAuthorizedPermission: " + Permission.HasUserAuthorizedPermission(Permission.FineLocation));

                if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    Debug.Log("User has not given permissions to use fineLocation yet");
                    // The user denied permission to use the FineLocation.
                    // Display a message explaining why you need it with Yes/No buttons.
                    // If the user says yes then present the request again
                    // Display a dialog here.
                    dialog.AddComponent<PermissionsRationaleDialog>();
                    return;
                } else if (dialog != null)
                {
                    Destroy(dialog);
                    startFlic();
                } else {
                    startFlic();
                }
#endif
            permCheckCalled = true;
        }
    }

    private class AndroidUnityCallback : AndroidJavaProxy
    {
        public FlicToUnity flics;
        public AndroidUnityCallback() : base("dk.osandweb.flic2unity.UnityCallback")
        {
        }
        void onFlicClick()
        {
            flics.shoot();
        }

        void onFlicFailure()
        {
            Debug.Log("Flic clic failed.");
        }
    }

    void startFlic()
    {
        if (!flicCalled)
        {
            Debug.Log("Initiating flic ");
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
            Flic = new AndroidJavaObject("dk.osandweb.flic2unity.Flic");
            AndroidUnityCallback auc = new AndroidUnityCallback();
            auc.flics = this;
            Flic.Call("initAndGetInstance", context, auc);
            flicCalled = true;
        }
    }

}
