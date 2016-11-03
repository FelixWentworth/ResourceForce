using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class IncidentInformationDisplay : MonoBehaviour
{
    public GameObject IncidentHistoryElement;
    private IncidentManager _incidentManager;

    private List<IncidentHistoryElement> _incidentElements;
    private List<IncidentHistoryUISetup> _setupObjects;
    private int _totalElements;
    private int _shownElement;
    private float _offset;

    public Sprite WaitSprite;
    public Sprite OfficerSprite;
    public Sprite CitizenSprite;
    public void Show(List<IncidentHistoryElement> elements, IncidentHistoryElement currentElement, int severity)
    {
        ClearChildren();

        var alpha = GetSeverityTransparency(severity);

        _totalElements = 0;

        _incidentElements = new List<IncidentHistoryElement>();
        _incidentElements = elements;

        _offset = 0f;

        foreach (var incidentHistory in elements)
        {
            CreateElement(incidentHistory, _offset, false);
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
            pos = _setupObjects.Count-1;
        }
        // move the Header objects to expose the selected element

        // HACK reset all items then set the one we want explicitly for testing
        _offset = 0f;
        var foundPos = false;
        var rectHeight = this.GetComponent<RectTransform>().rect.height;

        for (var i =0; i < _setupObjects.Count; i++)
        {
           _setupObjects[i].DropDownImage.localRotation = Quaternion.Euler(0f, 0f, 0f);
            if (!foundPos)
            {
                StartCoroutine(TransitionToPosition(_setupObjects[i].Header, new Vector3(0f, _offset * i, 0f), 0.5f));
                //_setupObjects[i].Header.anchoredPosition3D = new Vector3(0f, _offset * i, 0f);
                foundPos = i == pos;
                if (foundPos)
                {
                    _setupObjects[i].DropDownImage.localRotation = Quaternion.Euler(0f, 0f, 270f);
                }
            }
            else
            {
                var offsetFromBottom = rectHeight + (_offset * (_setupObjects.Count - (i) ) );
                StartCoroutine(TransitionToPosition(_setupObjects[i].Header, new Vector3(0f, offsetFromBottom * -1f, 0f), 0.5f));
                //_setupObjects[i].Header.anchoredPosition3D = new Vector3(0f, offsetFromBottom * -1f , 0f);
            }
            
            _offset = _setupObjects[i].Header.rect.height * -1f;
        }
        _shownElement = pos;
    }

    private IEnumerator TransitionToPosition(RectTransform transform, Vector3 position, float time)
    {
        var deltaTime = 0f;
        var startPos = transform.anchoredPosition3D;
        while (deltaTime <= time)
        {
            transform.anchoredPosition3D = Vector3.Lerp(startPos, position, deltaTime/time);
            deltaTime += Time.deltaTime;
            yield return null;
        }
        transform.anchoredPosition3D = position;
    }
    private void CreateElement(IncidentHistoryElement element, float offset, bool isCurrent = true)
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

        var sprite = setup.Icon.sprite;
        if (!isCurrent)
        {
            setup.Icon.gameObject.SetActive(true);
            switch (element.PlayerDecision)
            {
                case global::IncidentHistoryElement.Decision.Ignore:
                    sprite = WaitSprite;
                    break;
                case global::IncidentHistoryElement.Decision.Citizen:
                    sprite = CitizenSprite;
                    break;
                case global::IncidentHistoryElement.Decision.Officer:
                    sprite = OfficerSprite;
                    break;
            }
        }
        else
        {
            setup.Icon.gameObject.SetActive(false);
            setup.DropDownImage.localRotation = Quaternion.Euler(0f, 0f, 270f);
        }
        if (_incidentManager == null)
        {
            _incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
        }

        setup.Setup(element.Type, element.Description, sprite, _totalElements, _incidentManager.GetSeverityColor(element.Severity));
        setup.Header.anchoredPosition3D = new Vector3(0f, offset, 0f);

        _setupObjects.Add(setup);
        _offset -= setup.Header.rect.height;
    }

    public float GetSeverityTransparency(int severity)
    {
        //set the alpha of the severity overlay
        switch (severity)
        {
            case 3:
                return 1f;
            case 2:
                return 0.5f;
            default:
                return 0f;
        }
    }
}
