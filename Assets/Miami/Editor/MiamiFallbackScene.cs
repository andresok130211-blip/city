using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.IO;

public static class MiamiFallbackScene
{
    const string ScenePath="Assets/Miami/Scenes/Miami_FreeRoam.unity";
    [MenuItem("Miami City/Create Demo City Scene")]
    public static void CreateScene()
    {
        Directory.CreateDirectory("Assets/Miami/Scenes");
        Scene scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
        RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.58f,.67f,.76f);
        RenderSettings.fog=true;RenderSettings.fogColor=new Color(.56f,.76f,.86f);RenderSettings.fogDensity=.008f;
        Material road=Mat("Road",new Color(.12f,.14f,.16f)),side=Mat("Sidewalk",new Color(.72f,.69f,.61f)),grass=Mat("Park",new Color(.16f,.38f,.22f)),sand=Mat("Beach",new Color(.83f,.72f,.52f)),water=Mat("Ocean",new Color(.03f,.48f,.62f)),glass=Mat("Glass",new Color(.16f,.55f,.68f)),white=Mat("Stucco",new Color(.91f,.84f,.72f)),pink=Mat("Coral",new Color(.82f,.30f,.24f)),yellow=Mat("Pastel",new Color(.91f,.70f,.36f));
        Cube("Ground",new Vector3(0,-.6f,0),new Vector3(240,.8f,240),grass);
        Cube("Beach",new Vector3(0,-.12f,104),new Vector3(240,.2f,30),sand);
        Cube("Ocean",new Vector3(0,-.35f,139),new Vector3(240,.2f,42),water);
        for(int x=-100;x<=100;x+=40)Cube("Avenue",new Vector3(x,.01f,0),new Vector3(9,.08f,205),road);
        for(int z=-100;z<=100;z+=40)Cube("Street",new Vector3(0,.02f,z),new Vector3(205,.08f,8),road);
        for(int x=-100;x<=100;x+=40)for(int z=-100;z<=60;z+=40)
        {
            if(Mathf.Abs(x)<25&&Mathf.Abs(z)<25){Cube("Park",new Vector3(x,0,z),new Vector3(24,.18f,24),grass);Palm(x-7,z-7);Palm(x+7,z+7);continue;}
            float h=Random.Range(13,46);Material m=(x+z)%3==0?pink:(x-z)%3==0?yellow:white;
            Cube("Building",new Vector3(x, h/2,z),new Vector3(Random.Range(13,21),h,Random.Range(13,21)),m);
            Cube("Glass facade",new Vector3(x,h*.55f,z-10.2f),new Vector3(7,h*.55f,.12f),glass);
            Cube("Roof",new Vector3(x,h+.25f,z),new Vector3(13,.5f,13),white);
        }
        for(int i=-100;i<=100;i+=18)Palm(i,88);
        GameObject sun=new GameObject("Sun",typeof(Light));sun.transform.rotation=Quaternion.Euler(45,-30,0);Light l=sun.GetComponent<Light>();l.type=LightType.Directional;l.intensity=1.25f;l.shadows=LightShadows.Soft;
        GameObject player=new GameObject("Player",typeof(CharacterController),typeof(ThirdPersonPlayer));player.transform.position=new Vector3(0,2,-12);
        CharacterController cc=player.GetComponent<CharacterController>();cc.height=1.9f;cc.radius=.34f;cc.center=new Vector3(0,.95f,0);cc.stepOffset=.3f;
        GameObject torso=GameObject.CreatePrimitive(PrimitiveType.Capsule);torso.name="Player body";torso.transform.SetParent(player.transform,false);torso.transform.localPosition=Vector3.up;torso.transform.localScale=new Vector3(.48f,.88f,.38f);Object.DestroyImmediate(torso.GetComponent<Collider>());torso.GetComponent<Renderer>().sharedMaterial=glass;
        GameObject head=GameObject.CreatePrimitive(PrimitiveType.Sphere);head.transform.SetParent(player.transform,false);head.transform.localPosition=Vector3.up*1.88f;head.transform.localScale=Vector3.one*.34f;Object.DestroyImmediate(head.GetComponent<Collider>());head.GetComponent<Renderer>().sharedMaterial=yellow;
        GameObject cam=new GameObject("Camera",typeof(Camera),typeof(AudioListener),typeof(OrbitFollowCamera));cam.GetComponent<Camera>().fieldOfView=65;cam.GetComponent<Camera>().farClipPlane=500;
        OrbitFollowCamera orbit=cam.GetComponent<OrbitFollowCamera>();orbit.target=player.transform;player.GetComponent<ThirdPersonPlayer>().cameraTransform=cam.transform;
        GameObject.Find("Mobile Controls");GameObject controls=new GameObject("Mobile Controls");MobileControls ui=controls.AddComponent<MobileControls>();ui.player=player.GetComponent<ThirdPersonPlayer>();ui.orbitCamera=orbit;
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        PlayerSettings.companyName="Independent Creator";PlayerSettings.productName="Miami Free Roam";PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,"com.independent.miami.freeroam");PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,ScriptingImplementation.IL2CPP);
        EditorSceneManager.SaveScene(scene,ScenePath);Debug.Log("Miami demo scene created.");
    }
    static Material Mat(string n,Color c){var m=new Material(Shader.Find("Standard"));m.name=n;m.color=c;return m;}
    static void Cube(string n,Vector3 p,Vector3 s,Material m){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=n;g.transform.position=p;g.transform.localScale=s;g.GetComponent<Renderer>().sharedMaterial=m;}
    static void Palm(float x,float z){var trunk=GameObject.CreatePrimitive(PrimitiveType.Cylinder);trunk.name="Palm trunk";trunk.transform.position=new Vector3(x,3,z);trunk.transform.localScale=new Vector3(.35f,3,.35f);trunk.GetComponent<Renderer>().sharedMaterial=Mat("Palm trunk",new Color(.38f,.24f,.12f));for(int i=0;i<6;i++){var leaf=GameObject.CreatePrimitive(PrimitiveType.Cube);leaf.name="Palm frond";leaf.transform.position=new Vector3(x+Mathf.Cos(i*Mathf.PI/3)*2,5.8f,z+Mathf.Sin(i*Mathf.PI/3)*2);leaf.transform.rotation=Quaternion.Euler(0,i*60,25);leaf.transform.localScale=new Vector3(4,.12f,.45f);leaf.GetComponent<Renderer>().sharedMaterial=Mat("Palm leaf",new Color(.12f,.38f,.18f));}}
}