using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance = null;
    public static EffectManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<EffectManager>();
            }

            return instance;
        }
    }

    public GameObject mergeEffect;
    public Transform mergeEffectParent;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ShowCircleMergeEffect(Vector2 mergePos, EnumSets.CircleLevel circleLevel)
    {
        StartCoroutine(CorShowMergeEffect(mergePos, circleLevel));
    }

    private IEnumerator CorShowMergeEffect(Vector2 mergePos, EnumSets.CircleLevel circleLevel)
    {
        yield return null;

        var effect = Instantiate(mergeEffect, mergeEffectParent);

        effect.transform.localPosition = new Vector3(mergePos.x, mergePos.y, -10);

        // CustomDebug.Log($"mergePos : {mergePos}");

        Destroy(effect, 0.35f);
    }

    public void ShowCircleMergeEffect(RectTransform circle, EnumSets.CircleLevel circleLevel)
    {
        StartCoroutine(CorShowMergeEffect(circle, circleLevel));
    }

    private IEnumerator CorShowMergeEffect(RectTransform circle, EnumSets.CircleLevel circleLevel)
    {
        yield return new WaitForSeconds(1f);

        //var mergePos = circle.GetComponent<RectTransform>().anchoredPosition;

        var effect = Instantiate(mergeEffect, mergeEffectParent);
        effect.transform.parent = circle.transform;
        effect.transform.localPosition = Vector3.zero;

        //effect.GetComponent<RectTransform>().anchoredPosition = mergePos;

        //CustomDebug.Log($"mergePos : {mergePos}");

        Destroy(effect, 0.35f);
    }
}
