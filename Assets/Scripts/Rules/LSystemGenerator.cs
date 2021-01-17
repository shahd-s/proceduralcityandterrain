using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SVS;
using UnityEngine;

public class LSystemGenerator : MonoBehaviour
{
    public Rule[] rules;
    public string rootSentance;
    [Range(0, 10)]
    public int iterationinit = 1;
    // Start is called before the first frame update
    void Start()
    {
      //  Debug.Log(GenerateSentance());
    }
    public string GenerateSentance(string word = null)
    {
        if(word == null)
        {
            word = rootSentance
;        }
        return GrowRecursive(word);
    }

    private string GrowRecursive(string word, int index=0)
    {
        if (index >= iterationinit)
        {
            return word;
        }
        StringBuilder stringBuilder = new StringBuilder();

        foreach (char c in word)
        {
            stringBuilder.Append(c);
            ProcessRulesRecursively(stringBuilder, c, index);
        }
        return stringBuilder.ToString();
    }

    private void ProcessRulesRecursively(StringBuilder stringBuilder, char c, int index)
    {
        foreach (Rule r in rules)
        {
            if (r.letter == c.ToString())
            {
                stringBuilder.Append(GrowRecursive(r.GetResult(), index + 1));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
