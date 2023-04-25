using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    [SerializeField] Text text;
    void Start()
    {
        /* Mandatory - set your AppsFlyer’s Developer key. */
      //  AppsFlyer.setAppsFlyerKey("p6kCY8PcXysqHoQVFRmowj");


#if UNITY_ANDROID
        /* Mandatory - set your Android package name */
       // AppsFlyer.setAppID("com.orbitrush.test.app");
        /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
       // AppsFlyer.init("p6kCY8PcXysqHoQVFRmowj", "AppsFlyerTrackerCallbacks");
#endif


    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
    public string getAppsFlyerId()
    {
        string AppsFlyerUID = AppsFlyer.getAppsFlyerId();
       return AppsFlyerUID;
    }

    // method for showing info from AppsFlyer.onInstallConversionData();
    public void Data()
    {
       //  AppsFlyer.onInstallConversionData();
        AppsFlyer.getConversionData();
    }
}
