using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CircleInfo", menuName = "ScriptableObjects/Create CircleInfo")]
public class CircleInfo : ScriptableObject
{
    //[SerializeField]
    //private Dictionary<EnumSets.CircleLevel, Dictionary<EnumSets.CircleImageTypeBySituation, Sprite>> circleImageTable = new Dictionary<EnumSets.CircleLevel, Dictionary<EnumSets.CircleImageTypeBySituation, Sprite>>();

    public CircleImagesByLevel[] arrayCircleImagesByLevel;

    public Sprite GetCircleImage(EnumSets.CircleLevel circleLevel, EnumSets.CircleImageTypeBySituation imageType)
    {
        var arrayIndex = (int)circleLevel;

        var circleImagesByLevel = this.arrayCircleImagesByLevel[arrayIndex];

        var sprite = circleImagesByLevel.GetCircleImage(imageType);
        
        return sprite;
    }

    public Sprite[] GetWholeCircleImagesArray(EnumSets.CircleLevel circleLevel)
    {
        var arrayIndex = (int)circleLevel;

        var circleImagesByLevel = this.arrayCircleImagesByLevel[arrayIndex];

        return circleImagesByLevel.GetWholeCircleImages();
    }
}
