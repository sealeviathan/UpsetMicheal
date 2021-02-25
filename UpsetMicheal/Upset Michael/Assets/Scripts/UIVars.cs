using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIVars : MonoBehaviour
{
    public TextMeshProUGUI distanceText;
    // Start is called before the first frame update
    private void Update()
    {
        distanceText.text = MapBuilder.instance.distance.ToString();
    }
    
}
