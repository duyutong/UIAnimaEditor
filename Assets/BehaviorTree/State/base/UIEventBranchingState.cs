using D.Unity3dTools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEventBranchingState : BehaviorTreeBaseState
{
    public EventTrigger trigger;

    public bool checkEndDrag;
    public bool checkBeginDrag;
    public bool checkDrag;

    public bool checkHover;
    public bool checkPointerEnter;
    public bool checkPointerExit;

    public bool checkDown;
    public bool checkClick;
    public bool checkUp;

    private PointerEventData eventData;
    private bool isInitFinish;
    private Dictionary<string, bool> checkDic { get; set; }

    private bool CheckTriggrtEvent(string checkName)
    {
        if (checkDic == null)
        {
            checkDic = new Dictionary<string, bool>();
            for (int i = 0; i < output.Count; i++)
            {
                BTOutputInfo info = output[i];
                checkDic.AddOrReplace(info.fromPortName, true);
            }
        }

        if (checkDic.ContainsKey(checkName)) return checkDic[checkName];
        else checkDic.Add(checkName, false);

        return false;
    }
    private void SetEventDataInOutput(PointerEventData data)
    {
        string portName = "pointerEventData";
        eventData = data;
        if (output.Count > 0 && output[0].fromPortName == portName)
        {
            output[0].value = data;
        }
        else
        {
            BTOutputInfo outputInfo = new BTOutputInfo();
            outputInfo.fromPortName = portName;
            outputInfo.toPortName = portName;
            outputInfo.value = data;
            output.Insert(0, outputInfo);
        }
    }
    public virtual void OnSetPointTriggerEvent(BTTargetObject bTTargetObject)
    {
        if (bTTargetObject == null) return;
        if (bTTargetObject.target == null) return;

        GameObject go = bTTargetObject.target as GameObject; 
        if (go == null) return;

        trigger = go.transform.GetOrAddComponent<EventTrigger>();

        if (runtime != null && !isInitFinish)
        {
            isInitFinish = true;

            checkBeginDrag = CheckTriggrtEvent("beginDrag");
            checkEndDrag = CheckTriggrtEvent("endDrag");
            checkDrag = CheckTriggrtEvent("drag");

            checkClick = CheckTriggrtEvent("click");
            checkUp = CheckTriggrtEvent("pointerUp");
            checkDown = CheckTriggrtEvent("pointerDown");

            checkHover = CheckTriggrtEvent("hover");
            checkPointerEnter = CheckTriggrtEvent("pointerEnter");
            checkPointerExit = CheckTriggrtEvent("pointerExit");


            if (checkDown) trigger.AddTriggerEventListener(EventTriggerType.PointerDown, SetEventDataInOutput);
            if (checkUp) trigger.AddTriggerEventListener(EventTriggerType.PointerUp, SetEventDataInOutput);
            if (checkClick) trigger.AddTriggerEventListener(EventTriggerType.PointerClick, SetEventDataInOutput);
            
            if (checkBeginDrag) trigger.AddTriggerEventListener(EventTriggerType.BeginDrag, SetEventDataInOutput);
            if (checkDrag) trigger.AddTriggerEventListener(EventTriggerType.Drag, SetEventDataInOutput);
            if (checkEndDrag) trigger.AddTriggerEventListener(EventTriggerType.EndDrag, SetEventDataInOutput);

            if (checkPointerEnter) trigger.AddTriggerEventListener(EventTriggerType.PointerEnter, SetEventDataInOutput);
            if (checkPointerExit) trigger.AddTriggerEventListener(EventTriggerType.PointerExit, SetEventDataInOutput);
        }
    }
    public bool IsMouseOverUIElement(RectTransform rect,Camera uiCamera) 
    {
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, uiCamera, out localMousePosition);
        return rect.rect.Contains(localMousePosition);
    }
    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "pointerEventData") info.value = eventData;
            output[i] = info;
        }

        base.OnExit();
    }
}
