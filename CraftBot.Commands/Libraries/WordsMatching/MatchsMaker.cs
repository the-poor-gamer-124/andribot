/*
Matching two strings
Author: Thanh Ngoc Dao - Thanh.dao@gmx.net
Copyright (c) 2005 by Thanh Ngoc Dao, All rights reserved.
---------
Thanks to Malcolm Crowe,Troy Simpson, Jeff Martin for the .NET WordNet port
Thanks to Carl Mercier for his french characters conversion function.

Please test carefully before using.
*/


using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WordsMatching
{
    /// <summary>
    /// Summary description for StringMatcher.
    /// </summary>
    /// 
    public delegate float Similarity(string s1, string s2);

    public class MatchsMaker
    {
        private string _lString, _rString;
        private string[] _leftTokens, _rightTokens;
        private int leftLen, rightLen;
        private float[,] cost;
        Similarity getSimilarity;

        private readonly bool _accentInsensitive;

        public MatchsMaker(string left, string right) : this(left, right, false) { }
        public MatchsMaker(string left, string right, bool accentInsensitive)
        {
            _accentInsensitive = accentInsensitive;

            _lString = left;
            _rString = right;

            if (_accentInsensitive)
            {
                _lString = this.StripAccents(_lString);
                _rString = this.StripAccents(_rString);
            }

            this.MyInit();
        }


        private string StripAccents(string input)
        {
            string beforeConversion = "�����������������������������������ǒ�";
            string afterConversion = "aAaAaAaAeEeEeEeEiIiIiIoOoOoOuUuUuUcC'n";

            var sb = new System.Text.StringBuilder(input);

            for (int i = 0; i < beforeConversion.Length; i++)
            {
                char beforeChar = beforeConversion[i];
                char afterChar = afterConversion[i];

                _ = sb.Replace(beforeChar, afterChar);
            }

            _ = sb.Replace("�", "oe");
            _ = sb.Replace("�", "ae");

            return sb.ToString();
        }

        private void MyInit()
        {
            ISimilarity editdistance = new Leven();
            getSimilarity = new Similarity(editdistance.GetSimilarity);

            //ISimilarity lexical=new LexicalSimilarity() ;
            //getSimilarity=new Similarity(lexical.GetSimilarity) ;


            var tokeniser = new Tokeniser();
            _leftTokens = tokeniser.Partition(_lString);
            _rightTokens = tokeniser.Partition(_rString);
            if (_leftTokens.Length > _rightTokens.Length)
            {
                string[] tmp = _leftTokens;
                _leftTokens = _rightTokens;
                _rightTokens = tmp;
                string s = _lString; _lString = _rString; _rString = s;
            }

            leftLen = _leftTokens.Length - 1;
            rightLen = _rightTokens.Length - 1;
            this.Initialize();

        }

        private void Initialize()
        {
            cost = new float[leftLen + 1, rightLen + 1];
            for (int i = 0; i <= leftLen; i++)
            {
                for (int j = 0; j <= rightLen; j++)
                {
                    cost[i, j] = getSimilarity(_leftTokens[i], _rightTokens[j]);
                }
            }
        }


        public float GetScore()
        {
            var match = new BipartiteMatcher(_leftTokens, _rightTokens, cost);
            return match.Score;
        }


        public float Score => this.GetScore();

    }
}
