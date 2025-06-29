using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameEvents;

public class SudokuGrid : MonoBehaviour
{
    public int columns = 3;
    public int rows = 3;
    public float square_offset = 5.0f;
    public GameObject grid_square;
    public Vector2 start_position = new Vector2(0.0f, 0.0f);
    public float square_scale = 1.0f;
    public float square_gap = 0.1f;
    public Color line_highlight_color = Color.red;
    private int selected_grid_data = -1;

    private List<GameObject> grid_squares_ = new List<GameObject>();

    // ✅ Panel referansı
    public GameObject winPanel;

    void Start()
    {
        if (grid_square == null)
        {
            Debug.LogError("GridSquare prefab'ı atanmadı!");
            return;
        }

        if (grid_square.GetComponent<GridSquare>() == null)
        {
            Debug.LogError("GridSquare prefab'ı GridSquare scriptine sahip değil!");
            return;
        }

        CreateGrid();

        if (GameSettings.Instance.GetContinuePreviousGame())
            SetGridFromFile();
        else
            SetGridNumber(GameSettings.Instance.GetGameMode());

        // ✅ Paneli başlangıçta kapat
        if (winPanel != null)
            winPanel.SetActive(false);
    }

    private void SetGridFromFile()
    {
        string level = GameSettings.Instance.GetGameMode();
        selected_grid_data = Config.ReadGameBoardLevel();
        var data = Config.ReadGridData();

        setGridSquareData(data);
        SetGritNotes(Config.GetGritNotes());
    }

    private void SetGritNotes(Dictionary<int, List<int>> notes)
    {
        foreach (var note in notes)
        {
            grid_squares_[note.Key].GetComponent<GridSquare>().SetGridNotes(note.Value);
        }
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetSquaresPosition();
    }

    private void SpawnGridSquares()
    {
        int square_index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject newSquare = Instantiate(grid_square, this.transform);
                newSquare.transform.localScale = new Vector3(square_scale, square_scale, square_scale);
                grid_squares_.Add(newSquare);
                grid_squares_[grid_squares_.Count - 1].GetComponent<GridSquare>().SetSquareIndex(square_index);
                square_index++;
            }
        }
    }

    private void SetSquaresPosition()
    {
        if (grid_squares_.Count == 0)
        {
            Debug.LogError("GridSquares listesi boş!");
            return;
        }

        RectTransform square_rect = grid_squares_[0].GetComponent<RectTransform>();
        if (square_rect == null)
        {
            Debug.LogError("GridSquare prefab'ında RectTransform eksik!");
            return;
        }

        Vector2 square_gap_number = new Vector2(0.0f, 0.0f);
        bool row_moved = false;

        Vector2 offset = new Vector2(
            square_rect.rect.width * square_rect.transform.localScale.x + square_offset,
            square_rect.rect.height * square_rect.transform.localScale.y + square_offset
        );

        int column_number = 0;
        int row_number = 0;

        foreach (GameObject square in grid_squares_)
        {
            if (column_number >= columns)
            {
                row_number++;
                column_number = 0;
                square_gap_number.x = 0;
                row_moved = false;
            }

            float pos_x_offset = offset.x * column_number + (square_gap_number.x * square_gap);
            float pos_y_offset = offset.y * row_number + (square_gap_number.y * square_gap);

            if (column_number > 0 && column_number % 3 == 0)
            {
                square_gap_number.x++;
                pos_x_offset += square_gap;
            }
            if (row_number > 0 && row_number % 3 == 0 && row_moved == false)
            {
                row_moved = true;
                square_gap_number.y++;
                pos_y_offset += square_gap;
            }

            square.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                start_position.x + pos_x_offset,
                start_position.y - pos_y_offset
            );

            column_number++;
        }
    }

    private void SetGridNumber(string level)
    {
        if (SudokuData.Instance == null)
        {
            Debug.LogError("SudokuData.Instance NULL!");
            return;
        }

        if (!SudokuData.Instance.sudoku_game.ContainsKey(level))
        {
            Debug.LogError($"Seçilen {level} seviyesi SudokuData'da bulunamadı!");
            return;
        }

        selected_grid_data = Random.Range(0, SudokuData.Instance.sudoku_game[level].Count);
        var data = SudokuData.Instance.sudoku_game[level][selected_grid_data];

        setGridSquareData(data);

        for (int index = 0; index < grid_squares_.Count; index++)
        {
            GridSquare square = grid_squares_[index].GetComponent<GridSquare>();
            square.SetCorrectNumber(data.solved_data[index]);
            square.SetNumber(data.unsloved_data[index]);
        }
    }

    private void setGridSquareData(SudokuData.SudokuBoardData data)
    {
        for (int index = 0; index < grid_squares_.Count; index++)
        {
            grid_squares_[index].GetComponent<GridSquare>().SetHasDefauldValue(data.unsloved_data[index] != 0 && data.unsloved_data[index] == data.solved_data[index]);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnSquareSelected += OnSquareSelected;
        GameEvents.OnUpdateSquareNumber += CheckBoardCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnSquareSelected -= OnSquareSelected;
        GameEvents.OnUpdateSquareNumber -= CheckBoardCompleted;

        var solved_data = SudokuData.Instance.sudoku_game[GameSettings.Instance.GetGameMode()][selected_grid_data].solved_data;
        int[] unsolved_data = new int[81];
        Dictionary<string, List<string>> grid_notes = new Dictionary<string, List<string>>();

        for (int i = 0; i < grid_squares_.Count; i++)
        {
            var comp = grid_squares_[i].GetComponent<GridSquare>();
            unsolved_data[i] = comp.GetSquareNumber();
            string key = "square_note:" + i.ToString();
            grid_notes.Add(key, comp.GetSquareNotes());
        }

        SudokuData.SudokuBoardData current_game_data = new SudokuData.SudokuBoardData(unsolved_data, solved_data);

        if (GameSettings.Instance.GetExitAfterWon() == false)
        {
            Config.SaveBoardData(current_game_data, GameSettings.Instance.GetGameMode(), selected_grid_data, Lives.instance.GetErrorNumber(), grid_notes);
        }
        else
        {
            Config.DeleteDataFile();
        }
    }

    private void SetSquaresColor(int[] data, Color col)
    {
        foreach (var index in data)
        {
            var comp = grid_squares_[index].GetComponent<GridSquare>();
            if (comp.HasWrongValue() == false && comp.İsSelected() == false)
            {
                comp.SetSquareColour(col);
            }
        }
    }

    public void OnSquareSelected(int square_index)
    {
        var horizontal_line = LineIndicator.instance.GetHorizontalLine(square_index);
        var vertical_line = LineIndicator.instance.GetVerticalLine(square_index);
        var square = LineIndicator.instance.GetSquare(square_index);

        SetSquaresColor(LineIndicator.instance.GetAllSquareIndexes(), Color.white);
        SetSquaresColor(horizontal_line, line_highlight_color);
        SetSquaresColor(vertical_line, line_highlight_color);
        SetSquaresColor(square, line_highlight_color);
    }

    private void CheckBoardCompleted(int number)
    {
        foreach (var square in grid_squares_)
        {
            var comp = square.GetComponent<GridSquare>();
            if (comp.IsCorrectNumberSet() == false)
            {
                return;
            }
        }

        GameEvents.OnBoardCompletedMethod();

        // ✅ Sudoku tamamlandıysa paneli göster
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }
    }

    public void SolveSudoku()
    {
        foreach (var square in grid_squares_)
        {
            var comp = square.GetComponent<GridSquare>();
            comp.SetCorrectNumber();
        }
        CheckBoardCompleted(0);
    }

    // ✅ Butonla oyunu yeniden başlatmak istersen:
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}