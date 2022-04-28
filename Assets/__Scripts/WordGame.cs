using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum eGameMode
{
    preGrame,
    loading,
    makeLevel,
    levelPrep,
    inLevel
}

public class WordGame : MonoBehaviour
{
    static public WordGame S;

    [Header("Set in Inspector")]
    public GameObject prefabLetter;
    public Rect       wordArea = new Rect(-24, 19, 48, 28);
    public float      letterSize = 1.5f;
    public bool       showAllWyrds = true;
    public float      bigLetterSize = 4f;
    public Color      bigColorDim = new Color(0.8f, 0.8f, 0.8f);
    public Color      bigColorSelected = new Color(1f, 0.9f, 0.7f);
    public Vector3    bigLetterCenter = new Vector3(0, -16, 0);
    public Color[]    wyrdPalette;

    [Header("Set Dynamically")]
    public eGameMode    mode = eGameMode.preGrame;
    public WordLevel    currLevel;
    public List<Wyrd>   wyrds;
    public List<Letter> bigLetters;
    public List<Letter> bigLettersActive;
    public string       testWord;
    private string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private Transform letterAnchor;
    private Transform bigLetterAnchor;

    private void Awake()
    {
        S = this;
        letterAnchor = new GameObject("LetterAnchor").transform;
        bigLetterAnchor = new GameObject("BigLetterAnchor").transform;
    }

    private void Start()
    {
        mode = eGameMode.loading;
        WordList.INIT();
    }

    private void Update()
    {
        Letter letter;
        char c;

        switch (mode)
        {
            case eGameMode.inLevel:
                foreach (char cInput in Input.inputString)
                {
                    c = System.Char.ToUpperInvariant(cInput);

                    if (upperCase.Contains(c))
                    {
                        letter = FindNextLetterByChar(c);

                        if (letter != null)
                        {
                            testWord += c.ToString();
                            bigLettersActive.Add(letter);
                            bigLetters.Remove(letter);
                            letter.color = bigColorSelected;
                            ArrangeBigLetters();
                        }
                    }

                    // Input is a 'backspace'
                    if (c == '\b')
                    {
                        if (bigLettersActive.Count == 0) return;
                        if (testWord.Length > 1)
                        {
                            testWord = testWord.Substring(0, testWord.Length - 1);
                        }
                        else
                        {
                            testWord = "";
                        }

                        letter = bigLettersActive[bigLettersActive.Count - 1];

                        bigLettersActive.Remove(letter);
                        bigLetters.Add(letter);
                        letter.color = bigColorDim;
                        ArrangeBigLetters();
                    }

                    // Input is 'enter/return'
                    if (c == '\n' || c == '\r')
                    {
                        CheckWord();
                    }

                    // Input is a 'space'
                    if (c == ' ')
                    {
                        bigLetters = ShuffleLetters(bigLetters);
                        ArrangeBigLetters();
                    }
                }
                break;
        }
    }

    private Letter FindNextLetterByChar(char c)
    {
        foreach (Letter letter in bigLetters)
        {
            if (letter.c == c) return letter;
        }

        return null;
    }
    private void CheckWord()
    {
        string subWord;
        bool foundTestWord = false;

        List<int> containedWords = new List<int>();

        for (int i = 0; i < currLevel.subWords.Count; i++)
        {
            if (wyrds[i].found && string.Equals(testWord,wyrds[i])) return;

            subWord = currLevel.subWords[i];

            if (string.Equals(testWord, subWord))
            {
                HighlightWyrd(i);
                ScoreManager.SCORE(wyrds[i], 1);
                foundTestWord = true;
            }
            else if (testWord.Contains(subWord))    
            {
                containedWords.Add(i);
                
            }
        }

        if (foundTestWord)
        {
            int numContained = containedWords.Count;
            int ndx;

            for (int i = 0; i < containedWords.Count; i++)
            {
                ndx = numContained - i - 1;
                HighlightWyrd(containedWords[ndx]);
                ScoreManager.SCORE(wyrds[containedWords[ndx]], i + 2);

            }
        }

        ClearBigLettersActive();
    }

    private void HighlightWyrd(int ndx)
    {
        wyrds[ndx].found = true;
        wyrds[ndx].color = (wyrds[ndx].color + Color.white) / 2f;
        wyrds[ndx].visible = true;
    }

    private void ClearBigLettersActive()
    {
        testWord = "";

        foreach (Letter letter in bigLettersActive)
        {
            bigLetters.Add(letter);
            letter.color = bigColorDim;
        }

        bigLettersActive.Clear();
        ArrangeBigLetters();
    }


    public void WordListParseComplete()
    {
        mode = eGameMode.makeLevel;
        currLevel = MakeWordLevel();
    }

    private WordLevel MakeWordLevel(int levelNum = -1)
    {
        WordLevel level = new WordLevel();

        if (levelNum == -1)
        {
            level.longWordIndex = Random.Range(0, WordList.LONG_WORD_COUNT());
        }
        else
        {
            
        }

        level.levelNum = levelNum;
        level.word = WordList.GET_LONG_WORD(level.longWordIndex);
        level.charDict = WordLevel.MakeCharDict(level.word);

        StartCoroutine(FindSubWordsCoroutine(level));

        return level;
    }

    private IEnumerator FindSubWordsCoroutine(WordLevel level)
    {
        level.subWords = new List<string>();
        string str;

        List<string> words = WordList.GET_WORDS();
        for (int i = 0; i < WordList.WORD_COUNT() ; i++)
        {
            str = words[i];

            if (WordLevel.CheckWordInLevel(str, level))
            {
                level.subWords.Add(str);
            }

            if (i % WordList.NUM_TO_PARSE_BEFORE_YIELD() == 0)
            {
                yield return null;
            }
        }

        level.SelectWords();
        SubWordSearchComplete();
    }
    
    public static IEnumerable<string> SortWordsByLength(IEnumerable<string> ws) {
        ws = ws.OrderBy(s => s.Length);
        return ws;
    }
    
    public void SubWordSearchComplete()
    {
        mode = eGameMode.levelPrep;
        Layout();
    }

    private void Layout()
    {
        CreateSubWordLayout();
        CreateBigLettersLayout();

        mode = eGameMode.inLevel;
    }

    private void CreateBigLettersLayout()
    {
        GameObject go;
        Letter letter;
        Vector3 pos;
        char c;

        bigLetters = new List<Letter>();
        bigLettersActive = new List<Letter>();

        for (int i = 0; i < currLevel.word.Length; i++)
        {
            c = currLevel.word[i];

            go = Instantiate<GameObject>(prefabLetter);
            go.transform.SetParent(bigLetterAnchor);

            letter = go.GetComponent<Letter>();
            letter.c = c;

            go.transform.localScale = Vector3.one * bigLetterSize;

            pos = new Vector3(0, -100, 0);
            letter.posImmediate = pos;
            letter.pos = pos;
            letter.timeStart = Time.time + currLevel.subWords.Count * 0.05f;
            letter.easingCurve = Easing.Sin + "-0.18";
            letter.color = bigColorDim;
            letter.visible = true;
            letter.big = true;
            bigLetters.Add(letter);
        }

        bigLetters = ShuffleLetters(bigLetters);

        ArrangeBigLetters();
    }

    private void CreateSubWordLayout()
    {
        GameObject go;
        Letter letter;
        Vector3 pos;
        char c;

        wyrds = new List<Wyrd>();
        string word;
        float left = 0;
        float columnWidth = 3;
        Wyrd wyrd;

        int numRows = Mathf.RoundToInt(wordArea.height / letterSize);

        for (int i = 0; i < currLevel.subWords.Count; i++)
        {
            wyrd = new Wyrd();
            word = currLevel.subWords[i];

            columnWidth = Mathf.Max(columnWidth, word.Length);

            for (int j = 0; j < word.Length; j++)
            {
                go = Instantiate<GameObject>(prefabLetter);
                go.transform.SetParent(letterAnchor);
                letter = go.GetComponent<Letter>();

                c = word[j];
                letter.c = c;

                pos = new Vector3(wordArea.x + left + j * letterSize, wordArea.y, 0);
                pos.y -= (i % numRows) * letterSize;

                letter.posImmediate = pos + Vector3.up * (20 + i % numRows);
                letter.pos = pos;
                letter.timeStart = Time.time + i * 0.05f;

                go.transform.localScale = Vector3.one * letterSize;
                wyrd.Add(letter);
            }

            if (showAllWyrds)
            {
                wyrd.visible = true;
            }

            wyrd.color = wyrdPalette[word.Length - WordList.WORD_LENGTH_MIN()];
            wyrds.Add(wyrd);

            if (i % numRows == numRows - 1)
            {
                left += (columnWidth + 0.5f) * letterSize;
            }
        }
    }

    private List<Letter> ShuffleLetters(List<Letter> letters)
    {
        int ndx;
        List<Letter> newLetters = new List<Letter>();

        while (letters.Count > 0)
        {
            ndx = Random.Range(0, letters.Count);
            newLetters.Add(letters[ndx]);
            letters.RemoveAt(ndx);
        }

        return newLetters;
    }

    private void ArrangeBigLetters()
    {
        Vector3 pos;

        float halfWidth = ((float) bigLetters.Count) / 2f - 0.5f;
        for (int i = 0; i < bigLetters.Count; i++)
        {
            pos = bigLetterCenter;
            pos.x += (i - halfWidth) * bigLetterSize;
            bigLetters[i].pos = pos;
        }

        halfWidth = ((float) bigLettersActive.Count) / 2f - 0.5f;
        for (int i = 0; i < bigLettersActive.Count; i++)
        {
            pos = bigLetterCenter;
            pos.x += (i - halfWidth) * bigLetterSize;
            pos.y += bigLetterSize * 1.25f;
            bigLettersActive[i].pos = pos;
        }

    }
}
