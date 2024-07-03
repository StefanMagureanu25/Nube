﻿namespace Nube.Typo_Correction
{
    public class LevenshteinAutomaton
    {
        private readonly string inputWord;
        private readonly int editDistance;
        private readonly int[] firstRow;
        public LevenshteinAutomaton(string inputWord, int editDistance)
        {
            this.inputWord = inputWord;
            this.editDistance = editDistance;
            this.firstRow = Enumerable.Range(0, inputWord.Length + 1).ToArray();
        }

        public LevenshteinAutomaton() { }

        public LevenshteinAutomaton(int editDistance)
        {
            this.editDistance = editDistance;
        }
        public int[] Start()
        {
            return firstRow;
        }
        public int[] Step(int[] aboveRow, char c)
        {
            int[] newRow = new int[aboveRow.Length];
            newRow[0] = aboveRow[0] + 1;
            for (int i = 0; i < aboveRow.Length - 1; i++)
            {
                int substitutionCost = 0;
                if (inputWord[i] != c)
                {
                    substitutionCost = 1;
                }
                newRow[i + 1] = minimumValue(newRow[i] + 1, aboveRow[i] +

                substitutionCost, aboveRow[i + 1] + 1);
            }
            return newRow;
        }
        private int minimumValue(int x, int y, int z)
        {
            return Math.Min(Math.Min(x, y), z);
        }
        public bool IsMatch(int[] row)
        {
            return row[row.Length - 1] <= editDistance;
        }
        public bool CanMatch(int[] row)
        {
            return row.Min() <= editDistance;
        }
    }
}
