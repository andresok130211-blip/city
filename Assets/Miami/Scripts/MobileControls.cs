using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MobileControls : MonoBehaviour
{
    public ThirdPersonPlayer player; public OrbitFollowCamera orbitCamera;
    RectTransform stickBase, stickKnob; Image lookPad; int moveFinger=-1, lookFinger=-1; Vector2 lastLook;
    void Start() { BuildUI(); }
    void BuildUI()
    {
        Canvas canvas=new GameObject("Mobile HUD",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster)).GetComponent<Canvas>();
        canvas.renderMode=RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1920,1080);
        if(!FindObjectOfType<EventSystem>())new GameObject("EventSystem",typeof(EventSystem),typeof(StandaloneInputModule));
        stickBase=MakeCircle(canvas.transform,"Joystick",new Vector2(190,190),new Vector2(155,155),Vector2.zero);
        stickKnob=MakeCircle(stickBase,"Knob",new Vector2(76,76),Vector2.zero,new Vector2(.5f,.5f));
        lookPad=MakeImage(canvas.transform,"Look Area",new Vector2(.55f,0),Vector2.one,Vector2.zero,Vector2.zero,new Color(1,1,1,.015f));
        EventTrigger trigger=lookPad.gameObject.AddComponent<EventTrigger>();
        AddTrigger(trigger,EventTriggerType.PointerDown,d=>{lookFinger=((PointerEventData)d).pointerId;lastLook=((PointerEventData)d).position;});
        AddTrigger(trigger,EventTriggerType.Drag,d=>{var p=(PointerEventData)d;if(p.pointerId!=lookFinger)return;Vector2 delta=p.position-lastLook;lastLook=p.position;if(orbitCamera)orbitCamera.AddLook(delta);});
        AddTrigger(trigger,EventTriggerType.PointerUp,d=>{if(((PointerEventData)d).pointerId==lookFinger)lookFinger=-1;});
        Button jump=MakeButton(canvas.transform,"JUMP",new Vector2(118,118),new Vector2(1,0),new Vector2(-155,155));jump.onClick.AddListener(()=>{if(player)player.QueueJump();});
        Button run=MakeButton(canvas.transform,"RUN",new Vector2(94,94),new Vector2(1,0),new Vector2(-290,110));run.onClick.AddListener(()=>{if(player)player.runSpeed=player.runSpeed<7?8f:6.5f;});
    }
    RectTransform MakeCircle(Transform parent,string name,Vector2 size,Vector2 pos,Vector2 anchor)
    {
        Image img=new GameObject(name,typeof(Image)).GetComponent<Image>();img.transform.SetParent(parent,false);img.sprite=Resources.GetBuiltinResource<Sprite>("UI/Skin/UISprite.psd");img.type=Image.Type.Sliced;img.color=name=="Joystick"?new Color(.08f,.12f,.16f,.48f):new Color(.85f,.94f,1,.72f);
        RectTransform rt=img.rectTransform;rt.anchorMin=rt.anchorMax=anchor;rt.pivot=new Vector2(.5f,.5f);rt.sizeDelta=size;rt.anchoredPosition=pos;
        if(name=="Joystick"){EventTrigger t=img.gameObject.AddComponent<EventTrigger>();AddTrigger(t,EventTriggerType.PointerDown,d=>{moveFinger=((PointerEventData)d).pointerId;MoveStick((PointerEventData)d);});AddTrigger(t,EventTriggerType.Drag,d=>{if(((PointerEventData)d).pointerId==moveFinger)MoveStick((PointerEventData)d);});AddTrigger(t,EventTriggerType.PointerUp,d=>{if(((PointerEventData)d).pointerId==moveFinger){moveFinger=-1;stickKnob.anchoredPosition=Vector2.zero;if(player)player.SetMove(Vector2.zero);}});}
        return rt;
    }
    void MoveStick(PointerEventData d){Vector2 p;RectTransformUtility.ScreenPointToLocalPointInRectangle(stickBase,d.position,d.pressEventCamera,out p);Vector2 v=Vector2.ClampMagnitude(p/(stickBase.sizeDelta.x*.5f),1);stickKnob.anchoredPosition=v*(stickBase.sizeDelta.x*.30f);if(player)player.SetMove(v);}
    Image MakeImage(Transform parent,string name,Vector2 min,Vector2 max,Vector2 omin,Vector2 omax,Color color){Image i=new GameObject(name,typeof(Image)).GetComponent<Image>();i.transform.SetParent(parent,false);i.color=color;RectTransform r=i.rectTransform;r.anchorMin=min;r.anchorMax=max;r.offsetMin=omin;r.offsetMax=omax;return i;}
    Button MakeButton(Transform parent,string label,Vector2 size,Vector2 anchor,Vector2 pos){Image img=new GameObject(label+" Button",typeof(Image),typeof(Button)).GetComponent<Image>();img.transform.SetParent(parent,false);img.color=new Color(.08f,.18f,.25f,.72f);RectTransform r=img.rectTransform;r.anchorMin=r.anchorMax=anchor;r.pivot=new Vector2(.5f,.5f);r.sizeDelta=size;r.anchoredPosition=pos;Text t=new GameObject("Label",typeof(Text)).GetComponent<Text>();t.transform.SetParent(img.transform,false);t.text=label;t.alignment=TextAnchor.MiddleCenter;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=24;t.color=Color.white;RectTransform tr=t.rectTransform;tr.anchorMin=Vector2.zero;tr.anchorMax=Vector2.one;tr.offsetMin=tr.offsetMax=Vector2.zero;return img.GetComponent<Button>();}
    void AddTrigger(EventTrigger t,EventTriggerType type,System.Action<BaseEventData> action){var e=new EventTrigger.Entry{eventID=type};e.callback.AddListener(d=>action(d));t.triggers.Add(e);}
}