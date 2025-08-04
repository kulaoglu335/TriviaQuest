using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;

public class UI_Category : MonoBehaviour
{
    [SerializeField] private List<CategoryInfoClass> categoryInfos = new List<CategoryInfoClass>();
    [SerializeField] private List<Image> colorChangeImages;

    [System.Serializable]
    public class CategoryInfoClass
    {
        public string categoryName;
        public GameObject categoryIcon;
        public DOTweenAnimation iconAnimation;
        public Color32 categoryColor;
    }

    public void SetCategoryTo(string categoryName)
    {
        int index = categoryInfos.FindIndex(c => c.categoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        Color32 newCategoryColor = categoryInfos[index].categoryColor;

        for (int i = 0; i < categoryInfos.Count; i++)
        {
            if (i == index)
            {
                categoryInfos[i].categoryIcon.SetActive(true);
                categoryInfos[i].iconAnimation.DORestart();
            }
            else
            {
                categoryInfos[i].categoryIcon.SetActive(false);
            }
        }

        for (int i = 0; i < colorChangeImages.Count; i++)
        {
            colorChangeImages[i].DOColor(newCategoryColor,0.5f);
        }
    }
}
