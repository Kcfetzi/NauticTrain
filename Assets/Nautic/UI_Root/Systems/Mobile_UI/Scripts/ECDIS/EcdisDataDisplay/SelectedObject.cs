using MPUIKIT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * The Selectedobject dropdownmenu.
 * It handles the objectname, position, velocity, direction and color of the selected object in ecdis.
 */
public class SelectedObject : DropDownMenu
{
        [Header("Objectname")]
        [SerializeField] private TMP_InputField _objectNameText;
    
        [Header("Position")]
        [SerializeField] private TMP_InputField _posX;
        [SerializeField] private TMP_InputField _posY;
        [SerializeField] private TMP_InputField _posZ;
        [SerializeField] private TMP_InputField _latText;
        [SerializeField] private TMP_InputField _lonText;
        
        [Header("Velocity")]
        [SerializeField] private TMP_InputField _velText;
        
        [Header("Direction")]
        [SerializeField] private TMP_InputField _dirText;
        
        [Header("Size")]
        [SerializeField] private Slider _sizeSlider;
        [SerializeField] private TMP_Text _sizeText;
    
        [Header("Color")] [SerializeField] 
        private MPImage _colorImage;
        
        
        private ObjectContainer _selectedData;
        
        // Set data from selecteddata into ui
        public void DisplayData(ObjectContainer selectedData)
        {
            _ownButton.interactable = true;
            _selectedData = selectedData;
            _sizeSlider.SetValueWithoutNotify(selectedData.EcdisSize);
        }

        public void Update()
        {
            if (_selectedData == null)
                return;
                
            _objectNameText.SetTextWithoutNotify(_selectedData.ObjectName);
            
            _posX.text = _selectedData.Position.x.ToString("F2");
            _posY.text = _selectedData.Position.y.ToString("F2");
            _posZ.text = _selectedData.Position.z.ToString("F2");
            _latText.text = _selectedData.Position.Lat.ToString("F6");
            _lonText.text = _selectedData.Position.Lon.ToString("F6");
            
            _velText.text = _selectedData.ActualVelocity.ToString();
            _dirText.text = _selectedData.ActualCourse.ToString();
            
            _sizeText.text = _selectedData.EcdisSize.ToString("F2");
            _colorImage.color = _selectedData.EcdisColor;
        }
        
        // Save uidata into selecteddata
        public void SaveData()
        {
            _selectedData.ObjectName = _objectNameText.text;
            _selectedData.EcdisSize = _sizeSlider.value;
            _selectedData.EcdisColor = _colorImage.color;
        }
    
}
