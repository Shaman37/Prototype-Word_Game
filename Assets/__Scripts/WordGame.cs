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

    [Header("Set Dynamically")]
    public eGameMode mode = eGameMode.preGrame;
    public WordLevel currLevel;

    private void Awake()
    {
        S = this;
    }

    private void Start()
    {
        mode = eGameMode.loading;
        WordList.INIT();
    }

    public void WordListParseComple()
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

        level.subWords.Sort();
        level.subWords = SortWordsByLength(level.subWords).ToList();

        SubWordSearchComplete();
    }

    static private IEnumerable<string> SortWordsByLength(IEnumerable<string> ws)
    {
        ws = ws.OrderBy(s => s.Length);
        return ws;
    }

    public void SubWordSearchComplete()
    {
        mode = eGameMode.levelPrep;
    }

}
