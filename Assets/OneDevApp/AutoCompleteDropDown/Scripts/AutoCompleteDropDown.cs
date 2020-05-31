using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace OneDevApp
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AutoCompleteDropDown : MonoBehaviour
    {
        public DropDownListItem SelectedItem { get; private set; } //outside world gets to get this, not set it
        
        public string Value { get; private set; }

        public bool SelectFirstItemOnStart = false;

        private bool _selectionIsValid = false;

        [SerializeField]
        private List<string> _availableOptions;
        public List<string> AvailableOptions
        {
            get { return _availableOptions; }
            set
            {
                _availableOptions = value;
                Init();
            }
        }

        private bool _isPanelActive = false;
        private bool _hasDrawnOnce = false;
        private bool isIntialize = false;

        private InputField _mainInput;
        private RectTransform _overlayRT;
        private RectTransform _itemsPanelRT;
        private GameObject itemTemplate;
        private CanvasGroup canvasGroup;

        private List<string> _removableOptions;
        private List<string> _panelItems; //items that will get shown in the dropdown
        private List<string> _prunedPanelItems; //items that used to show in the dropdown

        private Dictionary<string, GameObject> panelObjects;

        [System.Serializable]
        public class SelectionChangedEvent : UnityEngine.Events.UnityEvent<string, bool, int>
        {
        }

        [System.Serializable]
        public class SelectionTextChangedEvent : UnityEngine.Events.UnityEvent<string>
        {
        }

        [System.Serializable]
        public class SelectionValidityChangedEvent : UnityEngine.Events.UnityEvent<bool>
        {
        }

        // fires when input text is changed;
        public SelectionTextChangedEvent OnSelectionTextChanged;
        // fires when when an Item gets selected / deselected (including when items are added/removed once this is possible)
        public SelectionValidityChangedEvent OnSelectionValidityChanged;
        // fires in both cases
        public SelectionChangedEvent OnSelectionChanged;


        public void Awake()
        {
            isIntialize = Initialize();
        }

        private void OnEnable()
        {
            Init();
        }


        public void Init()
        {
            if (!isIntialize)
                Initialize();

            Value = string.Empty;
            _mainInput.text = string.Empty;
            panelObjects = new Dictionary<string, GameObject>();

            _removableOptions = new List<string>();
            _prunedPanelItems = new List<string>();
            _panelItems = new List<string>();

            RebuildPanel();

            if (SelectFirstItemOnStart && AvailableOptions.Count > 0)
            {
                OnItemClicked(AvailableOptions[0], 0);
            }

            ToggleDropdownPanel(false);

        }

        private bool Initialize()
        {

            bool success = true;
            try
            {
                canvasGroup = GetComponent<CanvasGroup>();
                _mainInput = GetComponentInChildren<InputField>();

                _overlayRT = _mainInput.transform.Find("Overlay").GetComponent<RectTransform>();
                _itemsPanelRT = _overlayRT.transform.Find("Scroll View").Find("Viewport").Find("Content").GetComponent<RectTransform>();
                _overlayRT.gameObject.SetActive(false);

                itemTemplate = transform.Find("ItemTemplate").gameObject;
                itemTemplate.SetActive(false);

                SetInteractable(true);
            }
            catch (System.NullReferenceException ex)
            {
                Debug.LogError("Something is setup incorrectly with the dropdownlist component causing a Null Refernece Exception");
                Debug.LogException(ex);
                success = false;
            }

            return success;
        }


        /// <summary>
        /// Rebuilds the contents of the panel in response to items being added.
        /// </summary>
        private void RebuildPanel()
        {
            //panel starts with all options
            _panelItems.Clear();
            _prunedPanelItems.Clear();
            panelObjects.Clear();

            foreach (Transform childTransform in _itemsPanelRT.transform)
            {
                Destroy(childTransform.gameObject);
            }

            foreach (string option in _availableOptions)
            {
                _panelItems.Add(option);
            }

            List<GameObject> itemObjs = new List<GameObject>(panelObjects.Values);

            int indx = 0;
            while (itemObjs.Count < _availableOptions.Count)
            {
                GameObject newItem = Instantiate(itemTemplate) as GameObject;
                newItem.name = "Item " + indx;
                newItem.transform.SetParent(_itemsPanelRT, false);
                itemObjs.Add(newItem);
                indx++;
            }

            for (int i = 0; i < itemObjs.Count; i++)
            {
                itemObjs[i].SetActive(i <= _availableOptions.Count);
                if (i < _availableOptions.Count)
                {
                    itemObjs[i].name = "Item " + i + " " + _panelItems[i];
                    itemObjs[i].transform.Find("Text").GetComponent<Text>().text = _panelItems[i]; //set the text value

                    Button itemBtn = itemObjs[i].GetComponent<Button>();
                    itemBtn.onClick.RemoveAllListeners();
                    string textOfItem = _panelItems[i]; //has to be copied for anonymous function or it gets garbage collected away
                    int position = i;
                    itemBtn.onClick.AddListener(() =>
                    {
                        OnItemClicked(textOfItem, position);
                    });
                    panelObjects[_panelItems[i]] = itemObjs[i];
                }
            }
            foreach (string key in _availableOptions)
            {

                if (_removableOptions.Contains(key))
                {
                    _panelItems.Remove(key);
                    panelObjects[key].SetActive(false);
                    _prunedPanelItems.Add(key);
                }
            }
        }

        public void SetAsSelectedItem(int position, bool toSetClicked = true)
        {
            if (position < 0) return;

            ToggleDropdownPanel(false);

            if (toSetClicked)
                OnItemClicked(AvailableOptions[position], position);
            else if (position >= 0)
            {
                this.Value = AvailableOptions[position];
                _mainInput.text = AvailableOptions[position];
                ToggleDropdownPanel(false);
            }
        }

        public void SetAsSelectedItem(string value, bool toSetClicked = true)
        {
            if (string.IsNullOrEmpty(value)) return;

            if (AvailableOptions.Count <= 0) return;

            int valuePos = AvailableOptions.IndexOf(value);

            SetAsSelectedItem(valuePos, toSetClicked);
        }

        /// <summary>
        /// what happens when an item in the list is selected
        /// </summary>
        /// <param name="item"></param>
        public virtual void OnItemClicked(string item, int position)
        {
            Value = item;

            if (Value.Contains("select"))
                _mainInput.text = string.Empty;
            else
                _mainInput.text = Value;

            bool validity_changed = (_panelItems.Contains(Value) != _selectionIsValid);
            _selectionIsValid = _panelItems.Contains(Value);
            OnSelectionChanged.Invoke(Value, _selectionIsValid, position);
            OnSelectionTextChanged.Invoke(Value);
            if (validity_changed)
            {
                OnSelectionValidityChanged.Invoke(_selectionIsValid);
            }

            ToggleDropdownPanel(false);
        }

        public void OnValueChanged(string currText)
        {
            PruneItems(currText);
            if (!_isPanelActive)
            {
                ToggleDropdownPanel(true);
            }
        }


        /// <summary>
        /// Toggle the drop down list
        /// </summary>
        public void ToggleDropdownPanel()
        {
            ToggleDropdownPanel(!_isPanelActive);

            if (_isPanelActive)
                RebuildPanel();
        }


        /// <summary>
        /// Toggle the drop down list
        /// </summary>
        /// <param name="directClick"> whether an item was directly clicked on</param>
        public void ToggleDropdownPanel(bool directClick)
        {
            _isPanelActive = directClick;

            _overlayRT.gameObject.SetActive(_isPanelActive);
        }

        private void PruneItems(string currText)
        {
            var toPrune = _panelItems.Where(x => !x.ToLower().Contains(currText.ToLower())).ToArray();
            foreach (string key in toPrune)
            {
                panelObjects[key].SetActive(false);
                _panelItems.Remove(key);
                _prunedPanelItems.Add(key);
            }

            var toAddBack = _prunedPanelItems.Where(x => x.ToLower().Contains(currText.ToLower())).ToArray();
            foreach (string key in toAddBack)
            {
                if (!_removableOptions.Contains(key))
                {
                    panelObjects[key].SetActive(true);
                    _panelItems.Add(key);
                    _prunedPanelItems.Remove(key);
                }
                else
                {
                    panelObjects[key].SetActive(false);
                    _prunedPanelItems.Add(key);
                }
            }
        }

        public void SetInteractable(bool isActive)
        {
            if (canvasGroup)
                canvasGroup.interactable = isActive;
        }
    }

}