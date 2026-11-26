using UnityEditor;
using UnityEngine;
public class MiamiAutoSceneSetup : AssetPostprocessor
{
 static bool queued;
 static void OnPostprocessAllAssets(string[] imported,string[] deleted,string[] moved,string[] movedFrom)
 {
  if(queued)return;
  foreach(string path in imported)if(path=="Assets/Miami/Models/city-real-place-25-77400-80-19400.obj"){queued=true;EditorApplication.delayCall+=()=>{queued=false;if(AssetDatabase.LoadAssetAtPath<GameObject>(path)&&!System.IO.File.Exists("Assets/Miami/Scenes/Miami_FreeRoam.unity"))MiamiPlayableGameSetup.CreateScene();};break;}
 }
}