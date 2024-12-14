using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace IgnoreSolutions
{

    public class MaskedTransition : MonoBehaviour
    {
        public enum Sides
        {
            Left, Right, Top, Bottom
        }

        Image[] SideRects = new Image[(int)Sides.Bottom + 1];

        [SerializeField] Image MaskImage;

        void OnEnable()
        {
            for(int i = 0; i < SideRects.Length; i++)
            {
                if (SideRects[i] != null) continue;
                GameObject newSideRect = new GameObject("RectSide_" + (Sides)i, typeof(Image));
                newSideRect.transform.parent = MaskImage.transform.parent;

                Image sideRectImg = newSideRect.GetComponent<Image>();
                sideRectImg.color = Color.black;
                sideRectImg.rectTransform.anchorMin = Vector2.zero;
                sideRectImg.rectTransform.anchorMax = Vector2.zero;
                sideRectImg.rectTransform.pivot = Vector2.zero;

                SideRects[i] = sideRectImg;
                SetRectSizePosBySize(SideRects[i].rectTransform, (Sides)i);

            }
        }

        private void SetRectSizePosBySize(RectTransform sideRect, Sides side)
        {
            //Screen.width
            Vector2 MaskPos = MaskImage.rectTransform.anchoredPosition;
            Vector2 MaskSize = MaskImage.rectTransform.sizeDelta;

            switch (side)
            {
                case Sides.Left:
                    sideRect.anchoredPosition = Vector2.zero;
                    sideRect.sizeDelta = new Vector2(MaskPos.x, Screen.height);
                    break;

                case Sides.Right:
                    sideRect.anchoredPosition = new Vector2(MaskPos.x + MaskSize.x, 0);
                    sideRect.sizeDelta = new Vector2(Screen.width - (MaskPos.x + MaskSize.x), Screen.height);
                    break;

                case Sides.Top:
                    sideRect.anchoredPosition = new Vector2(MaskPos.x, MaskPos.y + MaskSize.y);
                    sideRect.sizeDelta = new Vector2(MaskSize.x, Screen.height - (MaskPos.y + MaskSize.y));
                    break;

                case Sides.Bottom:
                    sideRect.anchoredPosition = new Vector2(MaskPos.x, 0);
                    sideRect.sizeDelta = new Vector2(MaskSize.x, MaskPos.y);
                    break;

            }
        }

        void Update()
        {
            for (int i = 0; i < SideRects.Length; i++)
            {
                SetRectSizePosBySize(SideRects[i].rectTransform, (Sides)i); 
            }
        }
    }
}
