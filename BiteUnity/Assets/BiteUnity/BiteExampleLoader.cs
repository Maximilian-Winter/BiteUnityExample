using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Bite.Compiler;
using Bite.Modules.Callables;
using Bite.Runtime;
using Bite.Runtime.CodeGen;
using Bite.Runtime.Memory;
using BiteUnity;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class BiteExampleLoader : MonoBehaviour
{
    private BiteProgram m_ExampleBiteProgram;
    private BiteVm m_ExampleBiteVm = new BiteVm();

    public string AssetBundleName;

    public GenericDictionary < string, GameObject > ExternalObjects = new GenericDictionary < string, GameObject >();
    private static Dictionary<string, AssetBundle> s_AssetBundles = new Dictionary < string, AssetBundle >();
    void Start()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
#endif
        StartExample();
    }

    public void StartExample()
    {
        UnitySystemConsoleRedirector.StopRedirect();
        m_ExampleBiteVm.InitVm();
        m_ExampleBiteVm.SynchronizationContext = SynchronizationContext.Current;

        IDictionary < string, object > ext = new Dictionary < string, object >();

        foreach ( KeyValuePair<string,GameObject> externalObject in ExternalObjects )
        {
            ext.Add( externalObject.Key, externalObject.Value );
        }
        
        m_ExampleBiteVm.RegisterExternalGlobalObjects(ext);

        if ( !s_AssetBundles.ContainsKey( AssetBundleName ) )
        {
            s_AssetBundles.Add( AssetBundleName, AssetBundle.LoadFromFile(Path.Combine("AssetBundles\\", AssetBundleName)));
        }
        
        TextAsset[] files = s_AssetBundles[AssetBundleName].LoadAllAssets<TextAsset>();

        string[] fileContent = new string[files.Length];
        int i = 0;
        foreach ( TextAsset textAsset in files )
        {
            fileContent[i] = textAsset.text;
            i++;
        }
        BiteCompiler compiler = new BiteCompiler();

        m_ExampleBiteProgram = compiler.Compile(fileContent);

        m_ExampleBiteProgram.TypeRegistry.RegisterType<Vector3>();
        m_ExampleBiteProgram.TypeRegistry.RegisterType<Input>();
        m_ExampleBiteProgram.TypeRegistry.RegisterType<KeyCode>();
        m_ExampleBiteProgram.TypeRegistry.RegisterType<GameObject>();
        m_ExampleBiteProgram.TypeRegistry.RegisterType<Transform>();
        m_ExampleBiteProgram.TypeRegistry.RegisterType<MeshRenderer>();
        m_ExampleBiteProgram.TypeRegistry.RegisterType(typeof(Random), "Random");
        m_ExampleBiteVm.RegisterSystemModuleCallables( m_ExampleBiteProgram.TypeRegistry );
        m_ExampleBiteVm.RegisterCallable( "UnityDeltaTime", new UnityDeltaTimeVmCallable() );
        
        m_ExampleBiteVm.Interpret(m_ExampleBiteProgram);
        m_ExampleBiteVm.CallBiteFunctionSynchron(new BiteFunctionCall( "Start", Array.Empty < DynamicBiteVariable >()), true);
    }

    public void StopMoveCameraVm()
    {
        m_ExampleBiteVm.Stop();
    }
    

    private void Update()
    {
        m_ExampleBiteVm.CallBiteFunctionSynchron(new BiteFunctionCall( "Update", Array.Empty < DynamicBiteVariable >()), true);
    }
#if UNITY_EDITOR
    private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
    {
        if(obj == PlayModeStateChange.ExitingPlayMode)
        {
            m_ExampleBiteVm.Stop();
        }
    }
#endif
    
}
