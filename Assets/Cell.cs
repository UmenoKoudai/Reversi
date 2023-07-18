using UnityEngine;
using UnityEngine.UI;
using static IColors;

public class Cell : MonoBehaviour
{
    [SerializeField] Text _blackCostText;
    [SerializeField] Text _whiteCostText;
    Colors _state = Colors.None;
    int _blackCost = 0;
    int _whiteCost = 0;

    public Colors CellColor { get => _state; set => _state = value; }
    public int BlackCost { get => _blackCost; set => _blackCost = value; }
    public int WhiteCost { get => _whiteCost; set => _whiteCost = value; }

    private void Update()
    {
        if(_blackCostText && _whiteCostText)
        {
            _blackCostText.text = _blackCost.ToString();
            _whiteCostText.text = _whiteCost.ToString();
        }
    }
}
