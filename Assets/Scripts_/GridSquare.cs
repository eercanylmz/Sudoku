using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using static GameEvents;

public class GridSquare : Selectable, IPointerClickHandler, ISubmitHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] private Text number_text;  // Text bileşeni (GameObject yerine direkt Text tutuluyor)
    public List<GameObject> number_notes;
    private bool note_active;
    private int number_ = 0;
    private int correct_number_ = 0;

    private bool selected_ = false;
    private int square_index_ = -1;
    private bool has_default_value_ = false;
    private bool has_wrongt_value_ = false;


    public int GetSquareNumber() { return number_; }
    public bool IsCorrectNumberSet() { return number_ == correct_number_; }
    public NotesActive onNotesActive { get; private set; }

    public bool HasWrongValue() { return has_wrongt_value_; }
    public void SetHasDefauldValue(bool has_default) { has_default_value_ = has_default; }

    public bool GetHasDefauldValue() { return has_default_value_; }


    public bool İsSelected() { return selected_; }
    public void SetSquareIndex(int index)
    {
        square_index_ = index;
    }
    public void SetCorrectNumber(int number)
    {
        correct_number_ = number;
        has_wrongt_value_ = false;

        if (number_ != 0 && number_ != correct_number_)
        {
            has_wrongt_value_ = true;
            SetSquareColour(Color.red);
        }
    }
    public void SetCorrectNumber()
    {
        number_ = correct_number_;
        SetNoteNumberValue(0);
        Display_text();
    }

    protected override void Start()
    {
        selected_ = false;
        note_active = false;
        if (GameSettings.Instance.GetContinuePreviousGame() == false)
            SetNoteNumberValue(0);
        else
            SetClearEmptyNotes();
    }
    public List<string> GetSquareNotes()
    {
        List<string> notes = new List<string>();

        foreach (var number in number_notes)
        {
            notes.Add(number.GetComponent<Text>().text);
        }
        return notes;
    }

    private void SetClearEmptyNotes()
    {
        foreach (var number in number_notes)
        {
            if (number.GetComponent<Text>().text == "0")
                number.GetComponent<Text>().text = " ";
        }
    }

    private void SetNoteSingelNumberValue(int value, bool force_update = false)
    {
        if (!note_active && !force_update)
            return;

        Debug.Log($"Setting Note Value: {value}"); // Konsolda kontrol et

        if (value <= 0)
            number_notes[value - 1].GetComponent<Text>().text = " ";
        else
        {
            if (number_notes[value - 1].GetComponent<Text>().text == " " || force_update)
                number_notes[value - 1].GetComponent<Text>().text = value.ToString();
            else
                number_notes[value - 1].GetComponent<Text>().text = " ";
        }
    }

    public void SetGridNotes(List<int> notes)
    {
        foreach (var note in notes)
        {
            SetNoteSingelNumberValue(note, true);
        }
    }

    public void OnNotesActive(bool active)
    {
        note_active = active;
    }
    private void SetNoteNumberValue(int value)
    {
        foreach (var number in number_notes)
        {
            if (value <= 0)
                number.GetComponent<Text>().text = "";
            else
                number.GetComponent<Text>().text = value.ToString();
        }
    }
    void Awake()
    {
        // Eğer number_text atanmadıysa, alt objeler arasında bulmaya çalış
        if (number_text == null)
        {
            number_text = GetComponentInChildren<Text>(); // GridSquare içindeki Text bileşenini bul
        }

        if (number_text == null)
        {
            Debug.LogError($"GridSquare ({gameObject.name}): number_text atanmadı! Lütfen prefab içinde Text bileşeninin olup olmadığını kontrol edin.");
        }
    }

    public void Display_text()
    {
        if (number_text != null)
        {
            number_text.text = (number_ > 0) ? number_.ToString() : "";
        }
        else
        {
            Debug.LogError($"GridSquare ({gameObject.name}): number_text bulunamadı!");
        }
    }

    public void SetNumber(int number)
    {
        number_ = number;
        Display_text();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        selected_ = true;
        GameEvents.SquareSelectedMethod(square_index_);
    }
    public void OnSubmit(BaseEventData eventData)
    {

    }
    public void OnEnable()
    {
        GameEvents.OnUpdateSquareNumber += OnSetNumber;
        GameEvents.OnSquareSelected += OnSquareSelected;
        GameEvents.OnNotesActive += OnNotesActive;
        GameEvents.OnClearNumber += OnClearNumber;

    }
    private void OnDisable()
    {
        GameEvents.OnUpdateSquareNumber -= OnSetNumber;
        GameEvents.OnSquareSelected -= OnSquareSelected;
        GameEvents.OnNotesActive -= OnNotesActive;

        GameEvents.OnClearNumber -= OnClearNumber;
    }

    public void OnClearNumber()
    {
        if (selected_ && !has_default_value_)
        {
            number_ = 0;
            has_wrongt_value_ = false;
            SetSquareColour(Color.white);
            SetNoteNumberValue(0);
            Display_text();
        }
    }

    public void OnSetNumber(int number)
    {
        if (selected_ && has_default_value_ == false)
        {
            if (note_active == true && has_wrongt_value_ == false)
            {
                SetNoteSingelNumberValue(number);
            }
            else if (note_active == false)
            {
                SetNoteNumberValue(0);

                SetNumber(number);
                if (number_ != correct_number_)
                {
                    has_wrongt_value_ = true;
                    var colors = this.colors;
                    colors.normalColor = Color.red;
                    this.colors = colors;

                    GameEvents.OnWrongNumberMethod();
                }
                else
                {
                    has_wrongt_value_ = false;
                    has_default_value_ = true;
                    var colors = this.colors;
                    colors.normalColor = Color.green;
                    this.colors = colors;
                }
            }
        }
    }
    public void OnSquareSelected(int square_index)
    {
        if (square_index_ != square_index)
        {
            selected_ = false;
        }
    }
    public void SetSquareColour(Color col)
    {
        var colors = this.colors;
        colors.normalColor = col;
        this.colors = colors;
    }
}