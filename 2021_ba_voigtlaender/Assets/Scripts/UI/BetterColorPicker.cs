using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


[System.Serializable]
public class ColorEvent : UnityEvent<Color> { }

[RequireComponent(typeof(ToggleGroup))]
public class BetterColorPicker : MonoBehaviour
{
    public List<Color> colors;
    public GameObject colorToggleTemplate;

    List<Toggle> toggles = new List<Toggle>();

    public ColorEvent OnValueChanged;


    ToggleGroup toggleGroup;
    private void Awake()
    {
        PopulateColorList();
    }
    public void PopulateColorList()
    {
        toggleGroup = GetComponent<ToggleGroup>();
        foreach (Color color in colors)
        {
            GameObject colorObj = Instantiate(colorToggleTemplate, transform);
            Toggle colorToggle = colorObj.GetComponent<Toggle>();
            colorToggle.targetGraphic.color = color;
            colorToggle.group = toggleGroup;


            colorToggle.onValueChanged.AddListener((value) =>
            {
                if (!value)
                    return;
                OnValueChanged?.Invoke(colorToggle.targetGraphic.color);
            });

            toggles.Add(colorToggle);
        }

        //Deactivate template
        colorToggleTemplate.gameObject.SetActive(false);
    }

    public void SetColor(int index, Color color)
    {
        if (index < 0 || index >= toggles.Count)
            return;

        toggles[index].targetGraphic.color = colors[index] = color;
        toggles[index].isOn = true;
    }

    public void SetColor( Color color)
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            if(toggles[i].targetGraphic.color == color)
            {
                if(!toggles[i].isOn)
                    toggles[i].isOn = true;
                return;
            }

        }

        SetColor(0, color);
    }
}
