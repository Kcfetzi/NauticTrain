using System;
using System.Collections.Generic;
using Groupup;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EcdisMapDisplay : MonoBehaviour, IPointerClickHandler
{
    [Header("Ecdisshapes")] 
    [SerializeField] private Area m_Area;
    [SerializeField] private PolyLineContainer _polyLineContainer;
    
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    
    // Container where all ecdissymbols instantiated
    [SerializeField] private Transform _symbolsContainer;
    [SerializeField] private Transform _shapesContainer;

    [SerializeField] private Symbol _cargo;
    [SerializeField] private Symbol _minesweeper;
    [SerializeField] private Symbol _ton;
    [SerializeField] private Symbol _point;
    
    // The scrollview
    [SerializeField] private ScrollRect _ectisView;

    [SerializeField] private RectTransform _scrollCenterPoint;
    // The viewport
    [SerializeField] private RectTransform _displayMask;
    [SerializeField] private RectTransform _viewPortCenter;
    // The image that shows the map
    [SerializeField] private RectTransform _mapImage;
    // scaleslider on ecdismap
    [SerializeField] private Slider _scaler;
    
    [SerializeField] private Toggle _showNameToggle;
    [SerializeField] private Toggle _centerActiveObjectToggle;
    
    private bool _showName;
    private bool _centerActiveObject;

    // user clicked in rightclickmenu on messure distance
    private bool _messureDistanceMode;
    private Symbol _lastCreatedSymbol;
    
    private UI_RootInterface _uiInterface;
    
    // Dropdown menu
    [SerializeField] private RightclickMenu _rightclickMenu;
    
    // active displayed ecdis object
    private Symbol _selectedObject;
    
    private int _uID = 0;

    public Symbol SelectedObject => _selectedObject;
    
    private void LateUpdate()
    {
        if (!SelectedObject)
            return;
        if (_centerActiveObject)
            CenterContentToTarget(SelectedObject.RectTransform.position);
    }

    // Setup connection to interface data
    public void Init(Texture2D map)
    {
         _uiInterface = ResourceManager.GetInterface<UI_RootInterface>();
        
         _rightclickMenu.Init(OnClick_Create, OnClick_Distance, OnClick_Delete);
         
        _mapImage.GetComponent<RawImage>().texture = map;
        _mapImage.sizeDelta = new Vector2(map.height, map.width);
        
        _uiInterface.EcdisMapSize = _mapImage.rect.size;
        
        _uiInterface.EcdisMapScale = new Vector3(.3f,
            .3f, 1);

        Vector3 terrainSize = ResourceManager.GetInterface<ScenarioInterface>().MapSize;
        _uiInterface.WorldToEcdisRatio = _uiInterface.EcdisMapSize / new Vector2(terrainSize.x, terrainSize.z);
        _uiInterface.EcdisToWorldRatio = new Vector2(terrainSize.x, terrainSize.z) / _uiInterface.EcdisMapSize;
        
        _scaler.minValue =  .3f;
        _scaler.maxValue = 8f;
        _scaler.onValueChanged.AddListener(Zoom);
        
        _mapImage.localScale = _uiInterface.EcdisMapScale;
        
        _showNameToggle.onValueChanged.AddListener((active) => _showName = active);  
        _showNameToggle.onValueChanged.AddListener((active) => ToggleObjectNames());  
        _centerActiveObjectToggle.onValueChanged.AddListener((active) => _centerActiveObject = active);
    }
    
    // Register an element from the scene in Ecdis.
    public void RegisterNauticObject(NauticObject obj)
    {
        Symbol symbol = null;
        switch (obj.Data.ShipType)
        {
            case NauticType.Cargo:
                symbol = Instantiate(_cargo, _symbolsContainer);
                break; 
            case NauticType.MineSweeper:
                symbol = Instantiate(_minesweeper, _symbolsContainer);
                break;
            case NauticType.Ton:
                symbol = Instantiate(_ton, _symbolsContainer);
                break;
            case NauticType.Point:
                symbol = Instantiate(_point, _symbolsContainer);
                break;
            default:
                symbol = Instantiate(_point, _symbolsContainer);
                break;
        }
        
        symbol.Init(obj, _showName);
        _uiInterface.ActiveEcdisSymbols.Add(symbol);
    }

    public void DeleteNauticObject(NauticObject obj)
    {
        _uiInterface.ActiveEcdisSymbols.Remove(obj.Symbol);
        DeletePolyLines(obj);
        
        // sometimes no symbol
        if (obj.Symbol)
            Destroy(obj.Symbol.gameObject);
    }

    public void SetSelectedObject(NauticObject obj)
    {
        if (_selectedObject)
            _selectedObject.SetSelected(false);

        if (obj)
        {
            _selectedObject = obj.Symbol;
            _selectedObject.SetSelected(true);
        }
    }
    
    
    // Spawns a polyline between 2 nauticobjects
    public PolyLine SpawnDynamicPolyline(List<Symbol> symbols)
   {
       // add the line before init, because init will invoke observer that maybe need the list of activeectislines
       PolyLine line = Instantiate(_polyLineContainer.EcdisPolyLinePrefab, _shapesContainer);
       _uiInterface.ActiveEcdisLines.Add(line);
       PolyLineContainer lineData = Instantiate(_polyLineContainer);
       lineData.UID  =  "l" + _uID++;
       line.Init(symbols, lineData);

       foreach (Symbol symbol in symbols)
       {
           symbol.ToggleObjectName(_showName);
       }
       
       return line;
   }
   
   // Spawns a polyline between 2 positions
   public PolyLine SpawnStaticPolyline(List<double2> positions)
   {
       // add the line before init, because init will invoke observer that maybe need the list of activeectislines
       PolyLine line = Instantiate(_polyLineContainer.EcdisPolyLinePrefab, _shapesContainer);
       _uiInterface.ActiveEcdisLines.Add(line);
       
       PolyLineContainer lineData = Instantiate(_polyLineContainer);
       lineData.UID  =  "l" + _uID++;
       line.Init(positions, lineData);
       
       
       return line;
   }
   
   // Returns a polyline with given id
   private PolyLine GetPolyLineForId(string id)
   {
       foreach (PolyLine ecdisMapPolyLine in _uiInterface.ActiveEcdisLines)
       {
           if (ecdisMapPolyLine.Data.UID == id)
               return ecdisMapPolyLine;
       }

       return null;
   }
   
   /**
    * Channelmethod. Destroy the given ecdisline
    */
   public void DeletePolyLine(string lineId)
   {
       ObjectsInterface objectsInfo = ResourceManager.GetInterface<ObjectsInterface>();
       
       PolyLine line = GetPolyLineForId(lineId);
       
       foreach (string pointId in line.Data.PointIds)
       {
           NauticObject obj = objectsInfo.GetNauticObjectForId(pointId);
           obj.Data.PolylineIds.Remove(line.Data.UID);
       }

       _uiInterface.ActiveEcdisLines.Remove(line);
       Destroy(line.gameObject);
   }
   
   /**
    * Remove all polylines connected to given nauticobject
    */
   public void DeletePolyLines(NauticObject obj)
   {
       ObjectsInterface objectsInfo = ResourceManager.GetInterface<ObjectsInterface>();
       
       foreach (string polylineId in obj.Data.PolylineIds)
       {
           PolyLine line = GetPolyLineForId(polylineId);

           foreach (string pointId in line.Data.PointIds)
           {
               // dont delete it from yourself to not modify the collection
               if (obj.Data.UID == pointId)
                   continue;
               
               NauticObject connectedObj = objectsInfo.GetNauticObjectForId(pointId);
               connectedObj.Data.PolylineIds.Remove(line.Data.UID);
           }
           
           _uiInterface.ActiveEcdisLines.Remove(line);
           Destroy(line.gameObject);
       }
       
       obj.Data.PolylineIds.Clear();
   }
   
   
   #region Helper

   // Konvert given coord to localcoord inside of map. If not found vector2.zero will be retured
   Vector2 FindLocalMapPoint(PointerEventData ped)
   {
       Vector2 localCursor;
       if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_mapImage, ped.position,
               ped.pressEventCamera, out localCursor))
           return Vector2.zero;
       
       return localCursor;
   }

   // Centeres the ecdis view on given recttransform
   private void CenterContentToTarget(Vector3 target)
   {
       Canvas.ForceUpdateCanvases();
       
       _mapImage.anchoredPosition =
           (Vector2)_ectisView.transform.InverseTransformPoint(_mapImage.position)
           - (Vector2)_ectisView.transform.InverseTransformPoint(target);
   }
   #endregion
   
   
   #region UIButtons

   // Ecdis map targetbutton
   public void OnClick_TargetPlayer()
   {
       CenterContentToTarget(_selectedObject.RectTransform.position);
   }
    
   // Slider zoom function
   private void Zoom(float zoomValue)
   {
       // set the point to focus after scroll to the point that is centered in the viewport
       _scrollCenterPoint.anchoredPosition =
           _mapImage.transform.InverseTransformPoint(_viewPortCenter.position);

       // after that scale the map
       _uiInterface.EcdisMapScale.x = zoomValue;
       _uiInterface.EcdisMapScale.y = zoomValue;
       _mapImage.localScale = _uiInterface.EcdisMapScale;
        
       // center map to old position
       CenterContentToTarget(_scrollCenterPoint.position);
   }

   // Toggle the displayed name
   private void ToggleObjectNames()
   {
       foreach (Symbol activeEcdisSymbol in _uiInterface.ActiveEcdisSymbols)
       {
           activeEcdisSymbol.ToggleObjectName(_showName);
       }
   }
   
   #endregion


   #region RightClickMenu

   
   /**
* Buttoncallback for rightclickmenu "erstellen"
*/
   public void OnClick_Create(Position pos)
   {
       ResourceManager.GetInterface<ObjectsInterface>()
           .SpawnObjectUnityPos(NauticType.Point, pos.UnityPositionFloat, Vector3.zero);
   }

   /**
    * Buttoncallback for rightclickmenu "distanz".
    * Spawns a line. If symbol is null pos has to be set and a point is spawned at it.
    * Startpoint is m_selectedObject, endpoint given symbol or created point.
    */
   public void OnClick_Distance(Position pos)
   {
       _messureDistanceMode = !_messureDistanceMode;
       // user activated it
       if (_messureDistanceMode)
       {
           _lastCreatedSymbol = ResourceManager.GetInterface<ObjectsInterface>()
               .SpawnObjectUnityPos(NauticType.Point, pos.UnityPositionFloat, Vector3.zero).Symbol;
       }
       else
       {
           // delete all created points and lines
           _lastCreatedSymbol = null;
       }
   }

   /**
    * Buttoncallback for rightclickmenu "Löschen".
    * Delete the clicked symbol object
    */
   public void OnClick_Delete(Symbol symbol)
   {
       ResourceManager.GetInterface<ObjectsInterface>().DeleteNauticObject(symbol.NauticObject);
       ResourceManager.GetInterface<ObjectsInterface>().SelectNauticObject(null);
   }

   #endregion
   
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            HandleLeftClickInput(eventData);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            HandleRightClickInput(eventData);
        }
    }
    
    /**
     * Raycast into the ui and determine actions based on the names from hitten objects.
     */
    private void HandleRightClickInput(PointerEventData eventData)
    {
        if (eventData.pointerPressRaycast.gameObject) {
            // Map was hit, that means new point will be created
            if (eventData.pointerPressRaycast.gameObject.CompareTag("EcdisMap"))
            {
                _rightclickMenu.Show(eventData.position,
                    new Position(_uiInterface.EcdisToWorldPosition(FindLocalMapPoint(eventData))), _messureDistanceMode);
            }

            Transform parent = eventData.pointerPressRaycast.gameObject.transform.parent;
            if (parent.CompareTag("NauticObject"))
            {
                _rightclickMenu.Show(eventData.position,
                    parent.GetComponent<Symbol>(), _messureDistanceMode);
            }
        }
    }
    
    /**
     * Raycast into the ui and determine actions based on the names from hitten objects.
     */
    private void HandleLeftClickInput(PointerEventData eventData)
    {
        // if user wants to messure distance just create point and draw line
        if (_messureDistanceMode)
        {
            // clicked on an nauticobject
            if (eventData.pointerPressRaycast.gameObject.transform.parent.CompareTag("NauticObject"))
            {
                Symbol symbol = eventData.pointerPressRaycast.gameObject.transform.parent.GetComponent<Symbol>();
                SpawnDynamicPolyline(new List<Symbol>(){_lastCreatedSymbol, symbol});
                
                _lastCreatedSymbol = symbol;
            }
            // clicked random
            else
            {
                Position worldPos = new Position(_uiInterface.EcdisToWorldPosition(FindLocalMapPoint(eventData)));
                Symbol nextPoint = ResourceManager.GetInterface<ObjectsInterface>()
                    .SpawnObjectUnityPos(NauticType.Point, worldPos.UnityPositionFloat, Vector3.zero).Symbol;
                SpawnDynamicPolyline(new List<Symbol>(){_lastCreatedSymbol, nextPoint});
                
                
                _lastCreatedSymbol = nextPoint;
            }
            return;
        }
        
        // interact with rightclickmenu
        if (eventData.pointerPressRaycast.gameObject.CompareTag("Rightclickmenu"))
        {
            eventData.pointerPressRaycast.gameObject.SetActive(true);
        }
        // interact with map
        else if (eventData.pointerPressRaycast.gameObject.CompareTag("EcdisMap"))
        {
            _rightclickMenu.gameObject.SetActive(false);
        }
        // look for selecting an object
        else
        {
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            graphicRaycaster.Raycast(eventData, raycastResults);

            GameObject closestSymbol = null;
            foreach (RaycastResult raycastResult in raycastResults)
            {
                if (raycastResult.gameObject.transform.parent.CompareTag("NauticObject"))
                {
                    if (closestSymbol == null || Vector2.Distance(closestSymbol.gameObject.transform.position, eventData.position) > Vector2.Distance(raycastResult.gameObject.transform.position, eventData.position))
                    {
                        closestSymbol = raycastResult.gameObject;
                    }
                }
            }
            if (closestSymbol)
            {
                Symbol symbol = closestSymbol.transform.parent.GetComponent<Symbol>();
                ResourceManager.GetInterface<ObjectsInterface>().SelectNauticObject(symbol.NauticObject);
                _rightclickMenu.gameObject.SetActive(false);
            }
        }
    }
}
