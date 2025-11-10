using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CircleImagesByLevel_Level_", menuName = "ScriptableObjects/Create CircleImagesByLevel")]
public class CircleImagesByLevel : ScriptableObject
{
    [SerializeField]
    private Sprite spriteAppear = null;

    [SerializeField]
    private Sprite spriteDragAndDrop = null;

    [SerializeField]
    private Sprite spriteCollided = null;

    [SerializeField]
    private Sprite spriteDefault = null;

    public Sprite GetCircleImage(EnumSets.CircleImageTypeBySituation circleImageBySituation)
    {
        Sprite circleSprite = null;

        switch (circleImageBySituation)
        {
            case EnumSets.CircleImageTypeBySituation.Appear:
                {
                    circleSprite = spriteAppear;
                }
                break;
            case EnumSets.CircleImageTypeBySituation.WhileDragAndDrop:
                {
                    circleSprite = spriteDragAndDrop;
                }
                break;
            case EnumSets.CircleImageTypeBySituation.Collided:
                {
                    circleSprite = spriteCollided;
                }
                break;
            case EnumSets.CircleImageTypeBySituation.Default:
                {
                    circleSprite = spriteDefault;
                }
                break;
        }

        return circleSprite;

        // InGameSceneController.Instance.UpdateNextCircleImage(circleSprite);
    }

    public Sprite[] GetWholeCircleImages()
    {
        Sprite[] wholeImages = new Sprite[] {spriteAppear, spriteDragAndDrop, spriteCollided, spriteDefault };

        return wholeImages;
    }
}