using JetBrains.Annotations;
using UnityEngine;

public class BoundsLookup : MonoBehaviour
{
    public static BoundsLookup Instance;

    public BoxCollider BarBounds;

	[UsedImplicitly]
	void Start ()
	{
	    Instance = this;
	}

    public Bounds GetBarBounds()
    {
        return BarBounds.bounds;
    }
}
