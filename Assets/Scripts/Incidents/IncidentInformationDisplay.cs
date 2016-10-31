using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class IncidentInformationDisplay : MonoBehaviour
{
    public GameObject IncidentHistoryElement;

    private List<IncidentHistoryElement> _incidentElements;
    private List<IncidentHistoryUISetup> _setupObjects;
    private int _totalElements;
    private int _shownElement;
    private float _offset;
    public void Show(List<IncidentHistoryElement> elements, IncidentHistoryElement currentElement)
    {
        ClearChildren();
        
        
        _totalElements = 0;

        _incidentElements = new List<IncidentHistoryElement>();
        _incidentElements = elements;
        //_incidentElements.Add(currentElement);

        _offset = 0f;

        foreach (var incidentHistory in elements)
        {
            CreateElement(incidentHistory, _offset);
            _totalElements++;
        }
        CreateElement(currentElement, _offset);
        _totalElements++;
        _shownElement = _totalElements-1;
    }

    public void ClearChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        _setupObjects = new List<IncidentHistoryUISetup>();
    }

    public void ElementSelected(int pos)
    {
        if (pos == _shownElement)
        {
            // out early as there is no animating to do here
            return;
        }
        // move the Header objects to expose the selected element

        // HACK reset all items then set the one we want explicitly for testing
        _offset = 0f;
        var foundPos = false;
        var rectHeight = this.GetComponent<RectTransform>().rect.height;

        for (var i =0; i < _setupObjects.Count; i++)
        {
           
            if (!foundPos)
            {
                _setupObjects[i].Header.anchoredPosition3D = new Vector3(0f, _offset * i, 0f);
                foundPos = i == pos;
            }
            else
            {
                var offsetFromBottom = rectHeight + (_offset * (_setupObjects.Count - (i) ) );
                _setupObjects[i].Header.anchoredPosition3D = new Vector3(0f, offsetFromBottom * -1f , 0f);
            }
            
            _offset = _setupObjects[i].Header.rect.height * -1f;
        }
        _shownElement = pos;
    }

    private void CreateElement(IncidentHistoryElement element, float offset)
    {
        var go = GameObject.Instantiate(IncidentHistoryElement);

        var setup = go.GetComponent<IncidentHistoryUISetup>();
        var rectTransform = go.GetComponent<RectTransform>();

        go.transform.parent = this.transform;

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        rectTransform.localScale = Vector3.one;

        setup.Setup(element.Type, Localization.Get(element.Description), null, _totalElements);
        setup.Header.anchoredPosition3D = new Vector3(0f, offset, 0f);

        _setupObjects.Add(setup);
        _offset -= setup.Header.rect.height;
    }
}
