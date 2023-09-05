using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Recode : MonoBehaviour
{
    InputField _recodeText;
    void Start()
    {
        _recodeText = GetComponent<InputField>();
    }

    public void AddText(string text)
    {
        for(int i = 0; i < text.Length / 2; i++)
        {
            if(_recodeText.text.Length % 20 == 0 && _recodeText.text.Length != 0)
            {
                _recodeText.text += "\n";
            }
            for(int n = 0; n < 2; n++)
            {
                _recodeText.text += text[i + n];
            }
        }
    }
}
