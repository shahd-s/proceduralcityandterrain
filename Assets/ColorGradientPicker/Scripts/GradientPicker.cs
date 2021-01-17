using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GradientPicker : MonoBehaviour
{
    /// <summary>
    /// Event that gets called by the GradientPicker.
    /// </summary>
    /// <param name="g">received Gradient</param>
    public delegate void GradientEvent(Gradient g);

    #region private variables
    private static GradientPicker instance;
    /// <summary>
    /// True when the GradientPicker is closed
    /// </summary>
    private static bool done = true;


    //onGradientChanged Event
    private static GradientEvent onGC;
    //onGradientSelected Event
    private static GradientEvent onGS;

    //Gradient before editing
    private static Gradient originalGradient;
    //current Gradient
    private static Gradient modifiedGradient;

    private static bool interact;

    private List<Slider> colorKeyObjects;
    private List<GradientColorKey> colorKeys;
    private int selectedColorKey;
    private List<Slider> alphaKeyObjects;
    private List<GradientAlphaKey> alphaKeys;
    private int selectedAlphaKey;
    #endregion

    #region public variables
    [Header("Object references")]
    public Image colorComponent;
    public Transform alphaComponent;
    public GameObject key;
    public TextMeshProUGUI alphaText;
    #endregion

    #region Awake/update
    private void Awake()
    {
        instance = this;
        alphaText.text = 255.ToString();
    }
    #endregion

    #region creating

    public static bool Create(Gradient original, string message, GradientEvent onGradientChanged, GradientEvent onGradientSelected)
    {
        if (instance is null)
        {
            return false;
        }
        if (done)
        {
            done = false;
            originalGradient = new Gradient();
            originalGradient.SetKeys(original.colorKeys, original.alphaKeys);
            modifiedGradient = new Gradient();
            modifiedGradient.SetKeys(original.colorKeys, original.alphaKeys);
            onGC = onGradientChanged;
            onGS = onGradientSelected;
            instance.Setup();
            return true;
        }
        else
        {
            Done();
            return false;
        }
    }

    #endregion



    //Setup new GradientPicker
    private void Setup()
    {
        interact = false;
        colorKeyObjects = new List<Slider>();
        colorKeys = new List<GradientColorKey>();
        alphaKeyObjects = new List<Slider>();
        alphaKeys = new List<GradientAlphaKey>();
        foreach (GradientColorKey k in originalGradient.colorKeys)
        {
            CreateColorKey(k);
        }
        foreach (GradientAlphaKey k in originalGradient.alphaKeys)
        {
            CreateAlphaKey(k);
        }
        CalculateTexture();
        interact = true;
    }
    //creates a ColorKey UI object
    private void CreateColorKey(GradientColorKey k)
    {
        if (colorKeys.Count < 8)
        {
            Slider s = Instantiate(key, transform.position, new Quaternion(), transform).GetComponent<Slider>();
            ((RectTransform)s.transform).anchoredPosition = new Vector2(0, -29f);
            s.name = "ColorKey";
            s.gameObject.SetActive(true);
            s.value = k.time;
            s.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = k.color;
            colorKeyObjects.Add(s);
            colorKeys.Add(k);
            ChangeSelectedColorKey(colorKeys.Count - 1);
        }
    }
    //checks if new ColorKey should be created
    public void CreateNewColorKey(float time)
    {
        if (Input.GetMouseButtonDown(0))
        {
            interact = false;
            CreateColorKey(new GradientColorKey(modifiedGradient.Evaluate(time), time));
            interact = true;
        }
    }
    //creates a AlphaKey UI object
    private void CreateAlphaKey(GradientAlphaKey k)
    {
        if (alphaKeys.Count < 8)
        {
            Slider s = Instantiate(key, transform.position, new Quaternion(), transform).GetComponent<Slider>();
            ((RectTransform)s.transform).anchoredPosition = new Vector2(0, 25f);
            s.transform.GetChild(0).GetChild(0).rotation = new Quaternion();
            s.name = "AlphaKey";
            s.gameObject.SetActive(true);
            s.value = k.time;
            s.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(k.alpha, k.alpha, k.alpha, 1f);
            alphaKeyObjects.Add(s);
            alphaKeys.Add(k);
            ChangeSelectedAlphaKey(alphaKeys.Count - 1);
        }
    }
    //checks if new AlphaKey should be created
    public void CreateNewAlphaKey(float time)
    {
        if (Input.GetMouseButtonDown(0))
        {
            interact = false;
            CreateAlphaKey(new GradientAlphaKey(modifiedGradient.Evaluate(time).a, time));
            interact = true;
        }
    }

    private void CalculateTexture()
    {
        Color[] g = new Color[325];
        for (int i = 0; i < g.Length; i++)
        {
            g[i] = modifiedGradient.Evaluate(i / (float)g.Length);
        }
        Texture2D tex = new Texture2D(g.Length, 1)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        tex.SetPixels(g);
        tex.Apply();
        GetComponent<RawImage>().texture = tex;
        onGC?.Invoke(modifiedGradient);
    }

    //accessed by alpha Slider
    public void SetAlpha(float value)
    {
        if (interact)
        {
            alphaKeys[selectedAlphaKey] = new GradientAlphaKey(value, alphaKeys[selectedAlphaKey].time);
            modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
            CalculateTexture();
            alphaText.text = Mathf.RoundToInt(value * 255f).ToString();
            alphaKeyObjects[selectedAlphaKey].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(value, value, value, 1f);
        }
    }
    //accessed by alpha InputField
    public void SetAlpha(string value)
    {
        alphaComponent.GetComponent<Slider>().value = Mathf.Clamp(int.Parse(value), 0, 255) / 255f;
        CalculateTexture();
    }

    private void ChangeSelectedColorKey(int value)
    {
        if (colorKeyObjects.Count() > selectedColorKey)
        {
            colorKeyObjects[selectedColorKey].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        }
        if (alphaKeyObjects.Count() > 0)
        {
            alphaKeyObjects[selectedAlphaKey].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        }
        colorKeyObjects[value].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.green;
        if (selectedColorKey != value && !ColorPicker.done)
        {
            ColorPicker.Done();
        }
        selectedColorKey = value;
        colorKeyObjects[value].Select();
    }

    private void ChangeSelectedAlphaKey(int value)
    {
        if (alphaKeyObjects.Count > selectedAlphaKey)
        {
            alphaKeyObjects[selectedAlphaKey].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        }
        if (colorKeyObjects.Count > 0)
        {
            colorKeyObjects[selectedColorKey].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.gray;
        }
        alphaKeyObjects[value].transform.GetChild(0).GetChild(0).GetComponent<Image>().color = Color.green;
        selectedAlphaKey = value;
        alphaKeyObjects[value].Select();
    }
    //checks if Key can be deleted
    public void CheckDeleteKey(Slider s)
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (s.name == "ColorKey" && colorKeys.Count > 2)
            {
                if (!ColorPicker.done)
                {
                    ColorPicker.Done();
                    return;
                }
                int index = colorKeyObjects.IndexOf(s);
                Destroy(colorKeyObjects[index].gameObject);
                colorKeyObjects.RemoveAt(index);
                colorKeys.RemoveAt(index);
                if (index <= selectedColorKey)
                {
                    ChangeSelectedColorKey(selectedColorKey - 1);
                }
                modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
                CalculateTexture();
            }
            if(s.name == "AlphaKey" && alphaKeys.Count > 2)
            {
                int index = alphaKeyObjects.IndexOf(s);
                Destroy(alphaKeyObjects[index].gameObject);
                alphaKeyObjects.RemoveAt(index);
                alphaKeys.RemoveAt(index);
                if (index <= selectedAlphaKey)
                {
                    ChangeSelectedAlphaKey(selectedAlphaKey - 1);
                }
                modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
                CalculateTexture();
            }
        }
    }
    //changes Selected Key
    public void Select()
    {
        Slider s = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
        s.transform.SetAsLastSibling();
        if (s.name == "ColorKey")
        {
            ChangeSelectedColorKey(colorKeyObjects.IndexOf(s));
            alphaComponent.gameObject.SetActive(false);
            colorComponent.gameObject.SetActive(true);
            colorComponent.GetComponent<Image>().color = colorKeys[selectedColorKey].color;
        }
        else
        {
            ChangeSelectedAlphaKey(alphaKeyObjects.IndexOf(s));
            colorComponent.gameObject.SetActive(false);
            alphaComponent.gameObject.SetActive(true);
            alphaComponent.GetComponent<Slider>().value = alphaKeys[selectedAlphaKey].alpha;
            alphaComponent.GetChild(4).GetComponent<InputField>().text = Mathf.RoundToInt(alphaKeys[selectedAlphaKey].alpha * 255f).ToString();
        }
    }
    //accessed by position Slider
    public void SetTime(float time)
    {
        if (interact)
        {
            Slider s = EventSystem.current.currentSelectedGameObject.GetComponent<Slider>();
            if (s.name == "ColorKey")
            {
                int index = colorKeyObjects.IndexOf(s);
                colorKeys[index] = new GradientColorKey(colorKeys[index].color, time);
            }
            else
            {
                int index = alphaKeyObjects.IndexOf(s);
                alphaKeys[index] = new GradientAlphaKey(alphaKeys[index].alpha, time);
            }
            modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
            CalculateTexture();
        }
    }
    //accessed by position InputField
    public void SetTime(string time)
    {
        interact = false;
        float t = Mathf.Clamp(int.Parse(time), 0, 100) * 0.01f;
        if (colorComponent.gameObject.activeSelf)
        {
            colorKeyObjects[selectedColorKey].value = t;
            colorKeys[selectedColorKey] = new GradientColorKey(colorKeys[selectedColorKey].color, t);
        }
        else
        {
            alphaKeyObjects[selectedAlphaKey].value = t;
            alphaKeys[selectedAlphaKey] = new GradientAlphaKey(alphaKeys[selectedAlphaKey].alpha, t);
        }
        modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
        CalculateTexture();
        interact = true;
    }
    //choose color button call
    public void ChooseColor()
    {
        ColorPicker.Create(colorKeys[selectedColorKey].color, "", (c) => UpdateColor(selectedColorKey, c), null);
    }

    private void UpdateColor(int index, Color c)
    {
        interact = false;
        colorKeys[index] = new GradientColorKey(c, colorKeys[index].time);
        colorKeyObjects[index].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().color = c;
        colorComponent.color = c;
        modifiedGradient.SetKeys(colorKeys.ToArray(), alphaKeys.ToArray());
        CalculateTexture();
        interact = true;
    }
    //done button call
    public void CDone()
    {
        Done();
    }
    /// <summary>
    /// Manually close the GradientPicker and apply the selected color
    /// </summary>
    public static void Done()
    {
        if(!ColorPicker.done)
            ColorPicker.Done();
        foreach (Slider s in instance.colorKeyObjects)
        {
            Destroy(s.gameObject);
        }
        foreach (Slider s in instance.alphaKeyObjects)
        {
            Destroy(s.gameObject);
        }
        instance.colorKeyObjects = null;
        instance.colorKeys = null;
        instance.alphaKeyObjects = null;
        instance.alphaKeys = null;
        done = true;
        onGC?.Invoke(modifiedGradient);
        onGS?.Invoke(modifiedGradient);
        instance.transform.parent.gameObject.SetActive(false);
    }
}
