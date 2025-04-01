using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UI_Popup : UI_Base
{

   public override void Init()
   {
      Managers.UI.SetCanvas(gameObject, true);
   }


   public virtual void ClosePopupUI()
   {
      Managers.UI.ClosePopupUI(this, 0.5f); 

      CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>(); 

      canvasGroup.DOFade(0, 0.2f).SetEase(Ease.InOutSine);
   }
} 
