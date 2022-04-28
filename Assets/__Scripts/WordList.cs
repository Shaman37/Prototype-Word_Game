using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordList : MonoBehaviour
{
    private static WordList S;

    [Header("Set in Inspector")]
    public TextAsset textWordList;
    public int       numToParseBeforeYield = 10000;
    public int       wordLengthMin = 3;
    public int       wordLengthMax = 7;

    [Header("Set Dynamically")]
    public int currLine = 0;
    public int totalLines;
    public int longWordCount;
    public int wordCount;

    private string[]     lines;
    private List<string> longWords;
    private List<string> words;

    private Func<int, int, bool> isLongWord = (wL, wLMax) => wL >= wLMax;
    private Func<int, int, int, bool> isValidWord = (wL, wLMin, wLMax) => wLMin <= wL && wL <= wLMax;
    private void Awake()
    {
        S = this;
    }

    public void Init()
    {
        lines = textWordList.text.Split('\n');
        totalLines = lines.Length;

        StartCoroutine(ParseLines());
    }

    private IEnumerator ParseLines()
    {
        string word;
        longWords = new List<string>();
        words = new List<string>();

        for (currLine = 0; currLine < totalLines; currLine++)
        {
            word = lines[currLine];

            if (isLongWord(word.Length, wordLengthMax))
            {
                longWords.Add(word);
            }

            if (isValidWord(word.Length, wordLengthMin, wordLengthMax))
            {
                words.Add(word);
            }

            if (currLine % numToParseBeforeYield == 0)
            {
                longWordCount = longWords.Count;
                wordCount = words.Count;

                yield return null;
            }
        }

        longWordCount = longWords.Count;
        wordCount = words.Count;

        gameObject.SendMessage("WordListParseComplete");
    }

    static public void INIT()
    {
        S.Init();
    }
    static public List<string> GET_WORDS()
    {
        return S.words;
    }

    static public string GET_WORD(int ndx)
    {
        return S.words[ndx];
    }

    static public List<string> GET_LONG_WORDS()
    {
        return S.words;
    }

    static public string GET_LONG_WORD(int ndx)
    {
        return S.longWords[ndx];
    }

    static public int WORD_COUNT()
    {
        return S.words.Count;
    }

    static public int LONG_WORD_COUNT()
    {
        return S.longWords.Count;
    }

    static public int NUM_TO_PARSE_BEFORE_YIELD()
    {
        return S.numToParseBeforeYield;
    }

    static public int WORD_LENGHT_MIN()
    {
        return S.wordLengthMin;
    }

    static public int WORD_LENGHT_MAX()
    {
        return S.wordLengthMax;
    }
}
