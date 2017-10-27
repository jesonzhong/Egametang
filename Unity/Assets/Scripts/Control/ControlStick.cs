using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Model;

public class ControlStick : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool isDragging = false;

    public GameObject JoyStickBackground;
    public GameObject JoyStickBtn;

    protected float mRadius = 0f;

    private Vector2 origPos;

    private RectTransform thumb;

    //JoyStickBtn的当前位置
    public Vector2 positon;
    public Vector2 direction = new Vector2();
    private Vector3 m_originPosition;
    public Vector3 movePosNorm { get; private set; }
    private float currentAngle = 0;

    public void OnBeginDrag(PointerEventData data)
    {
        isDragging = true;

        Debug.Log("OnBeginDrag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;

        

        Debug.Log("OnEndDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //var contentPostion = this.content.anchoredPosition;
        //if (contentPostion.magnitude > mRadius)
        //{
        //    contentPostion = contentPostion.normalized * mRadius;
        //    SetContentAnchoredPosition(contentPostion);
        //}
        RectTransform draggingPlane = transform as RectTransform;

        Vector3 MousePos;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out MousePos))
        {
            thumb.position = MousePos;

            //Debug.Log("SetTarget Pos");
        }

        //float length = JoyStickBtn.transform.localPosition.magnitude;

        //Debug.Log("length: " + length);
        //if (length > 200)
        //{
            //JoyStickBtn.transform.localPosition = Vector3.ClampMagnitude(JoyStickBtn.transform.localPosition, 200);
        //}

        positon = JoyStickBtn.transform.localPosition;
        positon = positon / mRadius * Mathf.InverseLerp(200, 2, 1);

        movePosNorm = (JoyStickBtn.transform.localPosition - m_originPosition).normalized;
        //movePosNorm = new Vector3(movePosNorm.x, 0, movePosNorm.y);

        direction.x = movePosNorm.x;
        direction.y = movePosNorm.y;
        
        direction.Normalize();
    }

    public void LogicUpdate()
    {
        if (isDragging)
        {
            SessionComponent.Instance.Session.Send(new Frame_ClickMap() { X = (int)(direction.x * 1000), Z = (int)(direction.y * 1000) });
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.enterEventCamera, out localPoint);


        JoyStickBackground.transform.localPosition = localPoint;
        JoyStickBtn.transform.localPosition = Vector2.zero;

        Debug.Log("OnPointerDown x: " + localPoint.x + " y: " + localPoint.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");

        JoyStickBackground.transform.localPosition = origPos;
        JoyStickBtn.transform.localPosition = Vector2.zero;
    }

    // Use this for initialization
    void Start () {

        mRadius = (transform as RectTransform).sizeDelta.x * 0.5f;
        thumb = JoyStickBtn.transform.GetComponent<RectTransform>();
        origPos = JoyStickBackground.transform.localPosition;
        m_originPosition = JoyStickBtn.transform.localPosition;
        Debug.Log("mRadius: " + mRadius);
		
	}
	
    public void ShowJoyStick()
    {
        JoyStickBackground.SetActive(true);
    }

    public void HideJoyStick()
    {
        JoyStickBackground.SetActive(false);
    }
}
