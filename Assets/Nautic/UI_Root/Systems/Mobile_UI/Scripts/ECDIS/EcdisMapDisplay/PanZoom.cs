using System;
using System.Resources;
using System.Timers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ResourceManager = Groupup.ResourceManager;

public class PanZoom : MonoBehaviour
{
    public RawImage image;
    public ScrollRect ScrollRect;
    private float initialDistance;
    private Vector3 initialScale;
    private Vector2 zoomMapPos;
    private Vector2 zoomScrollRectPos;

    public RectTransform ScrollRectTransform;
    public RectTransform MapRectTransform;
    
    private Vector2 mapZoomPoint;
    private Vector2 viewZoomPoint;

    private UI_RootInterface _uiRootInterface;
    
    [Header("Debugger")] [SerializeField] private bool _useDebug;
    public RectTransform posMapMarker;
    public RectTransform posViewMarker;


    private void Start()
    {
        _uiRootInterface = ResourceManager.GetInterface<UI_RootInterface>();
    }

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            ScrollRect.horizontal = false;
            ScrollRect.vertical = false;
            
            var touchZero = Input.GetTouch(0); 
            var touchOne = Input.GetTouch(1);

            // if one of the touches Ended or Canceled do nothing
            if(touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled  
                                                   || touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled) 
            {
                return;
            }

            // It is enough to check whether one of them began since we
            // already excluded the Ended and Canceled phase in the line before
            if(touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                // track the initial values
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = image.transform.localScale;
                
                // use unity to transform touch position on the map
                Vector2 tapOneOnMap;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(MapRectTransform, touchZero.position, null,
                    out tapOneOnMap);
                // use unity to transform touch2 position on the map
                Vector2 tapTwoOnMap;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(MapRectTransform, touchOne.position, null,
                    out tapTwoOnMap);
                // focus point for zoom should be in the middle
                mapZoomPoint = (tapOneOnMap + tapTwoOnMap) / 2;
                
                // set debug image on the map
                if(_useDebug)
                    posMapMarker.localPosition = mapZoomPoint;

                
                // use unity to transform touch position on the viewport
                Vector2 tapOneOnView;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(ScrollRectTransform,
                    (touchZero.position + touchOne.position) / 2, null, out tapOneOnView);
                // use unity to transform touch2 position on the viewport
                Vector2 tapTwoOnView;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(ScrollRectTransform,
                    (touchZero.position + touchOne.position) / 2, null, out tapTwoOnView);
                // focus point for zoom should be in the middle
                viewZoomPoint = (tapOneOnView + tapTwoOnView) / 2;
                
                // set debug image on the map
                if(_useDebug)
                    posViewMarker.localPosition = viewZoomPoint;
                
                // sync the two points
                SyncMapAndViewport();
            }
            else
            {
                // otherwise get the current distance
                float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                // prefent from flickering
                if(Mathf.Approximately(initialDistance, 0)) 
                    return;

                // get the scale factor of the current distance relative to the inital one
                float factor = currentDistance / initialDistance;

                // apply the scale
                // instead of a continuous addition rather always base the 
                // calculation on the initial and current value only
                Vector3 scale = initialScale * factor;
                image.transform.localScale = scale;
                _uiRootInterface.EcdisMapScale.x = scale.x;
                _uiRootInterface.EcdisMapScale.y = scale.y;
                
                // sync the map and the viewport
                SyncMapAndViewport();
            }
        }
        else
        {
            if (!ScrollRect.horizontal || !ScrollRect.vertical)
            {
                ScrollRect.horizontal = true;
                ScrollRect.vertical = true;
                ScrollRect.velocity = Vector2.zero;
            }
        }
    }
    
    // Centeres the ecdis view on given recttransform
    private void SyncMapAndViewport()
    {
        // transform the point set at the start of pinch into worldspace
        Vector3 worldPointA = ScrollRectTransform.TransformPoint(viewZoomPoint);

        // transform the point set at the start of pinch into worldspace
        Vector3 worldPointB = MapRectTransform.TransformPoint(mapZoomPoint);

        // calc diff
        Vector3 offset = worldPointA - worldPointB;

        // add to map to set pos
        MapRectTransform.position += offset;
    }
    
}