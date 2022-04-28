using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WordLevel
{
    public int                   levelNum;
    public int                   longWordIndex;
    public string                word;
    public Dictionary<char, int> charDict;
    public List<string>          subWords;
    
    private System.Random rnd = new System.Random();



    static public Dictionary<char,int> MakeCharDict(string w)
    {
        Dictionary<char, int> dict = new Dictionary<char, int>();

        char c;
        for (int i = 0; i < w.Length; i++)
        {
            c = w[i];

            if (dict.ContainsKey(c))
            {
                dict[c]++;
            }
            else
            {
                dict.Add(c, 1);
            }
        }

        return dict;
    }

    static public bool CheckWordInLevel(string str, WordLevel level)
    {
        Dictionary<char, int> counts = new Dictionary<char, int>();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];

            if (level.charDict.ContainsKey(c))
            {
                if (counts.ContainsKey(c))
                {
                    counts[c]++;
                }
                else
                {
                    counts.Add(c, 1);
                    
                }

                if (counts[c] > level.charDict[c])
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    public List<string> GetWordsByLength(int lDesired)
    {
        List<string> words = subWords;
        words = words.FindAll(x => x.Length == lDesired);

        return words;
    }

    public static IEnumerable<string> SortWordsByLength(IEnumerable<string> ws) {
        ws = ws.OrderBy(s => s.Length);
        return ws;
    }

    public void SelectWords()
    {
        List<string> words = new List<string>();
        List<string> newWordList = new List<string>();
        int nWords = 12;
        int maxLetters = WordList.WORD_LENGTH_MAX();
        
        for (int i = 3; i < maxLetters; i++)
        {   
            words = GetWordsByLength(i).OrderBy(x => rnd.Next()).ToList();

            newWordList.AddRange(words.Take(nWords).ToList());
            nWords = nWords - i > 2 ? nWords - i : 2;
        }

        newWordList.Add(word);

        // Order alphabetically
        newWordList.Sort();

        // Order by Length
        newWordList = SortWordsByLength(newWordList).ToList();
        
        subWords = newWordList;
    }

}
