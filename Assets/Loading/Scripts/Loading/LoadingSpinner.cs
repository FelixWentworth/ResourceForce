using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public class LoadingSpinner : MonoBehaviour
{
	private GameObject _container;
	private Image _spinner;
	private Text _text;
	[SerializeField]
	private float _spinSpeed = 1;
	[SerializeField]
	private bool _spinClockwise;
	private bool _animate;

	void Awake()
	{
		Loading.LoadingSpinner = this;
		_container = transform.GetChild(0).gameObject;
		_spinner = _container.GetComponentsInChildren<Image>(true).First(i => i.gameObject != _container.gameObject);
		_text = GetComponentInChildren<Text>(true);
	}

	private void Update()
	{
		if (_container.gameObject.activeSelf)
		{
			if (_animate)
			{
				_spinner.transform.Rotate(0, 0, (_spinClockwise ? -1 : 1) * _spinSpeed * Time.smoothDeltaTime);
			}
			else
			{
				_container.gameObject.SetActive(false);
			}
		}
	}

	public void Set(bool clockwise, float speed)
	{
		_spinClockwise = clockwise;
		_spinSpeed = speed;
	}

	public void StartSpinner(string text)
	{
		_container.gameObject.SetActive(true);
		_text.text = text;

        _spinner.gameObject.SetActive(true);

        _animate = true;
	}

	public void StopSpinner(string text)
	{
		_text.text = text;
        _spinner.gameObject.SetActive(false);
        _animate = false;
	}

    public IEnumerator StopSpinner(string text, float stopDelay)
    {
        _text.text = text;
        _spinner.gameObject.SetActive(false);

        yield return new WaitForSeconds(stopDelay);

        _animate = false;
    }
    

}
