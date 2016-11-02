using UnityEngine;
using System.Collections;

public class ButtonFeedback : MonoBehaviour {

    private GameObject _ratingOne;
    private GameObject _ratingTwo;
    private GameObject _ratingThree;
    private GameObject _ratingFour;
    private GameObject _ratingFive;

    private Animation _animation;


    void Awake()
    {
        _ratingOne = transform.FindChild("RatingOnePanel").gameObject;
        _ratingTwo = transform.FindChild("RatingTwoPanel").gameObject;
        _ratingThree = transform.FindChild("RatingThreePanel").gameObject;
        _ratingFour = transform.FindChild("RatingFourPanel").gameObject;
        _ratingFive = transform.FindChild("RatingFivePanel").gameObject;

        _animation = GetComponent<Animation>();
    }

    public IEnumerator ShowFeedback(int rating, Transform buttonPressed)
    {
        var activeObject = GetActiveObject(rating);
        activeObject.SetActive(true);
        // Set position to the top of the button pressed
        transform.position = buttonPressed.position;

        // Animate the object
        _animation.Play();
        yield return new WaitForSeconds(_animation.clip.length);

        // Fade out the feedback objects
        var children = activeObject.GetComponentsInChildren<UnityEngine.UI.Image>();
        var childAnimLength = 0.0f;
        foreach (var child in children)
        {
            childAnimLength = child.GetComponent<Animation>().clip.length;
            child.GetComponent<Animation>().Play();
        }

        yield return new WaitForSeconds(childAnimLength);

        // Disable All objects
        DisableAll();

        // Reset for use later
        foreach (var child in children)
        {
            child.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    private GameObject GetActiveObject(int rating)
    {
        DisableAll();
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

    private void DisableAll()
    {
        _ratingOne.SetActive(false);
        _ratingTwo.SetActive(false);
        _ratingThree.SetActive(false);
        _ratingFour.SetActive(false);
        _ratingFive.SetActive(false);
    }
}
