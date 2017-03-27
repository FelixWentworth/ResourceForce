using UnityEngine;
using System.Collections;

public class ButtonFeedback : MonoBehaviour {

    private GameObject _ratingOne;
    private GameObject _ratingTwo;
    private GameObject _ratingThree;
    private GameObject _ratingFour;
    private GameObject _ratingFive;

    private Animation _animation;

    private SatisfactionDisplays _satisfactionDisplays;
    private IncidentManager _incidentManager;

    void Awake()
    {
        _ratingOne = transform.FindChild("RatingOnePanel").gameObject;
        _ratingTwo = transform.FindChild("RatingTwoPanel").gameObject;
        _ratingThree = transform.FindChild("RatingThreePanel").gameObject;
        _ratingFour = transform.FindChild("RatingFourPanel").gameObject;
        _ratingFive = transform.FindChild("RatingFivePanel").gameObject;

        _ratingOne.gameObject.SetActive(false);
        _ratingTwo.gameObject.SetActive(false);
        _ratingThree.gameObject.SetActive(false);
        _ratingFour.gameObject.SetActive(false);
        _ratingFive.gameObject.SetActive(false);

        _animation = GetComponent<Animation>();
    }

    public IEnumerator ShowFeedback(int rating, int severity, Transform buttonPressed)
    {
        if (rating != -1)
        {
            // if rating == -1 then either the rating has not been set or this button should not have been possible to press

            var activeObject = Instantiate(GetActiveObject(rating));

            activeObject.transform.parent = this.transform;
            var rect = activeObject.GetComponent<RectTransform>();
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;  

            activeObject.SetActive(true);
            
            // Set position to the top of the button pressed
            transform.position = buttonPressed.position;

            // Animate the object
            _animation.Play();
            yield return new WaitForSeconds(_animation.clip.length);

            // Fade out the feedback objects
            var ratingObjects = activeObject.GetComponentsInChildren<Transform>();

            if (!_satisfactionDisplays)
            {
                _satisfactionDisplays = GameObject.Find("SatisfactionBar").GetComponent<SatisfactionDisplays>();
            }
            if (!_incidentManager)
            {
                _incidentManager = GameObject.Find("TurnManager").GetComponent<IncidentManager>();
            }

            float ratingImpact = (rating - 3) * severity;

            ratingImpact = ratingImpact > 0 ? ratingImpact * 0.2f : ratingImpact;
            _incidentManager.AddHappiness(ratingImpact);
            foreach (var feedbackTransform in ratingObjects)
            {
                feedbackTransform.parent = GameObject.Find("Canvas").transform;
                StartCoroutine(_satisfactionDisplays.TransitionTo(feedbackTransform, 0.5f, _incidentManager.GetActualHappiness())); // -3 as 3 indicates a neutral choice, so no change
            }
        }
        else
        {
            // This object would normally be set active before it is used, so lets force it to be inactive
            this.gameObject.SetActive(false);
        }
    }

    private GameObject GetActiveObject(int rating)
    {
        switch (rating)
        {
            case 1:
                return _ratingOne;
            case 2:
                return _ratingTwo;
            case 4:
                return _ratingFour;
            case 5:
                return _ratingFive;
            default:
                return _ratingThree;
        }
    }
}
