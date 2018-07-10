using System;

namespace WdTG
{
    class Zad10 //Gra orzeł-reszka
    {
        static void Main(string[] args)
        {
            string[] coinGame = //Spis gier jakie są w odpowiedziach do zadania
            {
                "OROROROROR", //Odpowiedź A
                "OROOORRORR", //Odpowiedź B
                "OOROOORROR", //Odpowiedź C
                "RORROORRROOORRROORR", //Odpowiedź D
                "OROORROOOOROOOORROR" //Odpowiedź E
            };

            char[] answers = //Spis liter, żeby było czytelniej
            {
                'A',
                'B',
                'C',
                'D',
                'E'
            };

            int i = 0;
            bool winnableForFirstPlayer = false;

            while(i < coinGame.Length) //Szukanie gry, którą może wygrać gracz rozpoczynający
            {
                winnableForFirstPlayer = CheckFirstPlayerWin(coinGame[i]);
                if (winnableForFirstPlayer)
                    break;
                i++;
            }

            Console.WriteLine("Gra, w której wygrywa gracz rozpoczynający to gra z odpowiedzi: " + answers[i]); //Wynik jest literą odpowiedzi w zadaniu (jest czytelniej)
            Console.ReadKey(); //Do zatrzymania konsoli
        }

        private static bool CheckFirstPlayerWin(string _coinGame) //Sprawdzane jest, czy istnieje możliwość przegrania gry i jeżeli tak, gra jest odrzucana
        {
            bool gameStatus = false; //Status tej gry - wygrana = true, przegrana = false
            Random rand = new Random(); //Random potrzebny do losowego wybierania reszek - tails

            while(_coinGame.Contains("R")) //Dopóki istnieją reszki w grze
            {
                int maxTails = _coinGame.Split('R').Length - 1; //Najdalsza reszka patrząc od lewej
                int randomTails = rand.Next(maxTails); //Losowa reszka od lewej do najdalszej
                int tailsCounter = -1; //Licznik zamienionych reszek w grze

                for(int i = 0; i < _coinGame.Length; i++) //Zamiana orłów na reszki i na odwrót
                {
                    if (_coinGame[i].Equals('O'))
                        _coinGame = _coinGame.Remove(i, 1).Insert(i, "R");
                    else
                    {
                        _coinGame = _coinGame.Remove(i, 1).Insert(i, "O");
                        tailsCounter++;
                    }
                    if (tailsCounter == randomTails)
                        break;
                }

                gameStatus = !gameStatus; //Kolejny ruch
            }

            return gameStatus;
        }
    }
}
