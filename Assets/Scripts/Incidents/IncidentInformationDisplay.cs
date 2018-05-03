using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class IncidentInformationDisplay : MonoBehaviour
{
	public GameObject IncidentElement;
    public GameObject IncidentHistoryElement;

	public GameObject HistoryPanel;

    private IncidentManager _incidentManager;

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

        _totalElements = 0;

        _offset = 0f;

		// populate our history dialog
	    HistoryPanel.SetActive(false);	

		for (var i = elements.Count - 1; i >= 0; i--)
	    {
		    CreateHistoryElement(elements[i], _offset);
		    _totalElements++;
	    }

		//foreach (var incidentHistory in elements)
		//{
		//	CreateHistoryElement(incidentHistory, _offset);
		//	_totalElements++;
		//}

		HistoryPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, -_offset);

		// add our main element to incident dialog
		CreateElement(currentElement);
        _totalElements++;
        _shownElement = _totalElements-1;
    }

    public void ClearChildren()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
	    foreach (Transform child in HistoryPanel.transform)
	    {
		    Destroy(child.gameObject);
	    }
		_setupObjects = new List<IncidentHistoryUISetup>();
    }

    public void ElementSelected(RectTransform icon)
    {
		HistoryPanel.SetActive(!HistoryPanel.activeSelf);
	    var iconRotation = HistoryPanel.activeSelf ? Quaternion.Euler(0f, 0f, 270f) : Quaternion.Euler(0f, 0f, 0f);
	    icon.localRotation = iconRotation;
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

	private void CreateHistoryElement(IncidentHistoryElement element, float offset)
	{
		var go = Instantiate(IncidentHistoryElement);

		var setup = go.GetComponent<IncidentHistoryUISetup>();
		var rectTransform = go.GetComponent<RectTransform>();

		go.transform.SetParent(HistoryPanel.transform);

		rectTransform.anchorMin = Vector2.zero;
		rectTransform.anchorMax = Vector2.one;

		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;

		rectTransform.localScale = Vector3.one;

		var sprite = setup.Icon.sprite;

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

		if (_incidentManager == null)
		{
			_incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
		}

		setup.Setup(element.Type , element.Description, sprite, _totalElements, _incidentManager.GetSeverityColor(element.Severity));
		setup.Header.anchoredPosition3D = new Vector3(0f, offset, 0f);

		_setupObjects.Add(setup);
		_offset -= setup.Header.rect.height;
	}

    private void CreateElement(IncidentHistoryElement element)
    {
        var go = Instantiate(IncidentElement);

        var setup = go.GetComponent<IncidentHistoryUISetup>();
        var rectTransform = go.GetComponent<RectTransform>();

        go.transform.SetParent(this.transform);

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        rectTransform.localScale = Vector3.one;

        var sprite = setup.Icon.sprite;
        setup.Icon.gameObject.SetActive(false);
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
    
        if (_incidentManager == null)
        {
            _incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
        }

        setup.Setup(element.Type, element.Description, sprite, _totalElements, _incidentManager.GetSeverityColor(element.Severity));
        setup.Header.anchoredPosition3D = Vector3.zero;

        _setupObjects.Add(setup);
    }
}
