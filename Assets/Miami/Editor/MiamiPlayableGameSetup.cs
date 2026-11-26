using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.IO;
public static class MiamiPlayableGameSetup
{
 const string ScenePath="Assets/Miami/Scenes/Miami_FreeRoam.unity";
 const string ModelPath="Assets/Miami/Models/city-real-place-25-77400-80-19400.obj";
 public static void CreateScene()
 {
  Directory.CreateDirectory("Assets/Miami/Scenes");AssetDatabase.Refresh();
  GameObject model=AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
  if(!model){Debug.LogError("City OBJ missing or not imported.");return;}
  Scene scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(.55f,.61f,.68f);RenderSettings.fog=true;RenderSettings.fogColor=new Color(.68f,.78f,.84f);RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=.00016f;
  GameObject city=(GameObject)PrefabUtility.InstantiatePrefab(model);city.name="MIAMI — STATIC CITY";AddCityColliders(city);
  GameObject sun=new GameObject("Miami Sun",typeof(Light));sun.transform.rotation=Quaternion.Euler(42,-28,0);Light light=sun.GetComponent<Light>();light.type=LightType.Directional;light.intensity=1.2f;light.shadows=LightShadows.Soft;
  GameObject player=new GameObject("Player",typeof(CharacterController),typeof(ThirdPersonPlayer));player.transform.position=new Vector3(-600,85,-600);CharacterController cc=player.GetComponent<CharacterController>();cc.height=1.9f;cc.radius=.34f;cc.center=new Vector3(0,.95f,0);
  GameObject body=GameObject.CreatePrimitive(PrimitiveType.Capsule);body.transform.SetParent(player.transform,false);body.transform.localPosition=Vector3.up;body.transform.localScale=new Vector3(.48f,.88f,.38f);Object.DestroyImmediate(body.GetComponent<Collider>());
  GameObject head=GameObject.CreatePrimitive(PrimitiveType.Sphere);head.transform.SetParent(player.transform,false);head.transform.localPosition=Vector3.up*1.88f;head.transform.localScale=Vector3.one*.34f;Object.DestroyImmediate(head.GetComponent<Collider>());
  GameObject cameraObject=new GameObject("Third Person Camera",typeof(Camera),typeof(AudioListener),typeof(OrbitFollowCamera));Camera cam=cameraObject.GetComponent<Camera>();cam.fieldOfView=65;cam.farClipPlane=1800;OrbitFollowCamera orbit=cameraObject.GetComponent<OrbitFollowCamera>();orbit.target=player.transform;player.GetComponent<ThirdPersonPlayer>().cameraTransform=cameraObject.transform;
  GameObject ui=new GameObject("Mobile Controls");MobileControls controls=ui.AddComponent<MobileControls>();controls.player=player.GetComponent<ThirdPersonPlayer>();controls.orbitCamera=orbit;
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};PlayerSettings.companyName="Independent Creator";PlayerSettings.productName="Miami Free Roam";PlayerSettings.defaultInterfaceOrientation=UIOrientation.LandscapeLeft;PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android,"com.independent.miami.freeroam");PlayerSettings.Android.minSdkVersion=AndroidSdkVersions.AndroidApiLevel26;PlayerSettings.Android.targetArchitectures=AndroidArchitecture.ARM64;PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android,ScriptingImplementation.IL2CPP);
  EditorSceneManager.SaveScene(scene,ScenePath);
 }
 static void AddCityColliders(GameObject city){foreach(Transform t in city.GetComponentsInChildren<Transform>(true)){MeshFilter mf=t.GetComponent<MeshFilter>();if(!mf||!mf.sharedMesh)continue;string n=t.name.ToLower();if(n=="terrain"||n=="roads"){if(!t.GetComponent<MeshCollider>())t.gameObject.AddComponent<MeshCollider>().sharedMesh=mf.sharedMesh;}else if(n.StartsWith("building-")&&!t.GetComponent<Collider>()){Bounds b=mf.sharedMesh.bounds;BoxCollider box=t.gameObject.AddComponent<BoxCollider>();box.center=b.center;box.size=b.size;}}}
}