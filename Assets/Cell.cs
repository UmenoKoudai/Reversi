using UnityEngine;
using UnityEngine.UI;
using static IColors;

public struct Cell
{
    [SerializeField] Text _blackCostText;
    [SerializeField] Text _whiteCostText;
    [SerializeField] GameObject _point;
    Colors _state;
    int _blackCost;
    int _whiteCost;

    public Colors CellColor { get => _state; set => _state = value; }
    public int BlackCost { get => _blackCost; set => _blackCost = value; }
    public int WhiteCost { get => _whiteCost; set => _whiteCost = value; }

    private void Update()
    {
        if (_blackCostText && _whiteCostText)
        {
            _blackCostText.text = _blackCost.ToString();
            _whiteCostText.text = _whiteCost.ToString();
        }
        if (_blackCost > 0)
        {
            _point.SetActive(true);
        }
        else
        {
            _point.SetActive(false);
        }
    }
}
