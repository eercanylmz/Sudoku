using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteButton : Selectable, IPointerClickHandler
{
    public Sprite on_image;
    public Sprite off_image;
    private bool active_;

  public   void Start()
    {
        active_ = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        active_ = !active_;
        GetComponent<Image>().sprite = active_ ? on_image : off_image;

        Debug.Log("Note Mode: " + active_);
        GameEvents.OnNotesActiveMethod(active_);
    }
}
