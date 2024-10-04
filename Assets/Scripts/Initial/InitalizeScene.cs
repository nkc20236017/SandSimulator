using MackySoft.Navigathena.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitalizeScene : MonoBehaviour
{
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void RuntimeInitialize()
    {
        //StartupÉVÅ[Éìì«Ç›çûÇﬁ
        SceneManager.LoadScene("RootScene", LoadSceneMode.Additive);
    }
#endif
}
