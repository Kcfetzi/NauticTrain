namespace Groupup
{
    public static class Templater
    {
        public static string GetControllerCode(string controllerName, string interfaceName)
        {
            return $@"
using Groupup;
using UnityEngine;

public class {controllerName} : MonoBehaviour
{{
    private {interfaceName} _{interfaceName.ToLower()};

    void Awake()
    {{
        // Get own interface and subscribe
        _{interfaceName.ToLower()} = ResourceManager.GetInterface<{interfaceName}>();
        if (_{interfaceName.ToLower()})
        {{

        }}
        else
        {{
            Debug.Log(""Could not subscribe to interface in _{interfaceName.ToLower()}"");
        }}
    }}

    private void Start()
    {{
        // Tell all listeners that the service loaded.
        _{interfaceName.ToLower()}.SceneLoaded();
    }}

    void OnDestroy()
    {{
        // Unsubscribe to interface
        if (_{interfaceName.ToLower()})
        {{
            _{interfaceName.ToLower()}.IsActive = false;
        }}
    }}
}}
        ";
        }

        public static string GetDevControllerCode(string controllerName, string interfaceName)
        {
            return $@"
using UnityEngine;
using Groupup;

public class {controllerName} : MonoBehaviour
{{
    private {interfaceName} _{interfaceName.ToLower()};

    void Awake()
    {{
        // Get own interface
        _{interfaceName.ToLower()} = ResourceManager.GetInterface<{interfaceName}>();
        if (!_{interfaceName.ToLower()})
        {{
            Debug.Log(""Could not subscribe to interface in {controllerName}"");
        }}
        else
        {{
            _{interfaceName.ToLower()}.OnSceneLoaded += ReadyForAction;
        }}
    }}

    private void OnDestroy()
    {{
        _{interfaceName.ToLower()}.OnSceneLoaded -= ReadyForAction;
    }}

    private void ReadyForAction()
    {{
        Debug.Log(""{interfaceName} is loaded"");
    }}
}}
        ";
        }

        public static string GetInterfaceCode(string interfaceName)
        {
            return $@"
using UnityEngine;
using UnityEngine.Events;
using Groupup;

   /** 
   * Structurerfile
   * This is a generated file from Structurer.
   * If you edit this file, stick to the format from Funcs, UnityActions and methods, to keep on getting all benefits from Strukturer.
   */ 
public class {interfaceName} : InterfaceSOBase
{{
    
}}
";
        }

        public static string GetInfoCode(string infoName)
        {
            return $@"using UnityEngine;
using Groupup;

   /** 
   * Structurerfile
   * This is a generated file from Structurer.
   * You can add variables to it, to store or set information to the scene.
   */ 
public class {infoName} : SceneInfo
{{
    
}}
";
        }
        
        public static string GetSingletonCode(string singletonName)
        {
            return $@"
using UnityEngine;

public class {singletonName} : MonoBehaviour
{{
    private static {singletonName} _instance;

    public static {singletonName} Instance {{ get {{ return _instance; }} }}

    private void Awake()
    {{
        if (_instance != null && _instance != this)
        {{
            Destroy(this.gameObject);
        }}
        else
        {{
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }}
    }}
}}
            ";
        }

    }
}
 