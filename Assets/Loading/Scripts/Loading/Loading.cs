using System.Collections;

public static class Loading
{
	public static LoadingSpinner LoadingSpinner;

	public static void Set(int speed, bool clockwise)
	{
		if (LoadingSpinner)
		{
			LoadingSpinner.Set(clockwise, speed);
		}
	}

	public static void Start(string text = "")
	{
		if (LoadingSpinner)
		{
			LoadingSpinner.StartSpinner(text);
		}
	}

    public static void Stop(string text = "")
    {
        if (LoadingSpinner)
        {
            LoadingSpinner.StopSpinner(text);
        }
    }
    public static IEnumerator Stop(string text = "", float stopDelay = 0f)
    {
        if (LoadingSpinner)
        {
            yield return LoadingSpinner.StopSpinner(text, stopDelay);
        }
    }
}
