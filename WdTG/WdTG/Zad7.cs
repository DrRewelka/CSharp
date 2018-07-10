using System;
using System.Collections.Generic;
using System.Text;

namespace WdTG
{
    class Zad7 //Gra Kayles - wynik to ruchy wygrywające, ze wszystkich znaleziono ten, który występuje w odpowiedziach do zadania
    {
        private static int[] nimValues = { 0, 1, 2, 3, 1, 4, 3, 2, 1, 4, 2, 6, 4, 1, 2, 7, 1, 4, 3, 2, 1 }; //Z Wikipedii: https://en.wikipedia.org/wiki/Kayles
        private const string kaylesGame = "II IIIIIIIIIIIIIIIIIIII"; //Gra z polecenia

        static void Main(string[] args)
        {
            int i = 0;
            string kaylesGameCopy = String.Empty; //Do przechowywania kopii gry Kayles po podmianie znaków

            Console.WriteLine("Wygrywające ruchy:");
            Console.WriteLine();

            while(i < kaylesGame.Length) //Według zasad - można zabrać 1 "pion" lub 2 stojące obok siebie
            {
                StringBuilder kaylesHelper1 = new StringBuilder(kaylesGame); //Pomocniczy builder do podmiany znaków dla 1 "piona"
                StringBuilder kaylesHelper2 = new StringBuilder(kaylesGame); //Analogicznie, ale dla 2 "pionów"

                if (kaylesHelper1[i].Equals('I')) //Zabieranie 1 "piona"
                {
                    kaylesHelper1[i] = ' ';
                    kaylesGameCopy = kaylesHelper1.ToString();
                    FindWinningMove(kaylesGameCopy); //Sprawdzenie czy ruch jest wygrywający - musi mieć wartość 0 nim
                }

                if (i < kaylesGame.Length - 1) //Zabezpieczenie przed wyjściem poza tablicę
                {
                    if (kaylesHelper2[i].Equals('I') && kaylesHelper2[i + 1].Equals('I')) //Zabieranie 2 "pionów"
                    {
                        kaylesHelper2[i] = ' ';
                        kaylesHelper2[i + 1] = ' ';
                        kaylesGameCopy = kaylesHelper2.ToString();
                        FindWinningMove(kaylesGameCopy); //Sprawdzenie czy ruch jest wygrywający - musi mieć wartość 0 nim
                    }
                }

                i++;
            }

            Console.ReadKey(); //Do zatrzymania konsoli
        }

        private static void FindWinningMove(string _kaylesGame) //Szuka wygrywających ruchów
        {
            int nimValue = 0; //Wartość nim, musi wynosić 0 dla danego ruchu, aby był on wygrywającym
            int rowValue = 0; //Wartość rzędu
            int i = 0; //Pomocnicza wartość, do warunku pętli while i indeksów

            List<int> valuesHolder = new List<int>(); //Dodatkowy kontener na wartości pobierane z głównej tablicy wartości nim (na początku programu)

            while(i <= _kaylesGame.Length)
            {
                if (i < _kaylesGame.Length) //Zabezpieczenie przed wyjściem poza zakres tablicy
                {
                    if (_kaylesGame[i].Equals('I')) //Obliczanie wartości rzędu
                        rowValue++;
                    else
                    {
                        if (rowValue != 0)
                            valuesHolder.Add(nimValues[rowValue]); //Dopisywanie wartości do kontenera
                        rowValue = 0;
                    }
                }
                else
                {
                    if (rowValue != 0)
                        valuesHolder.Add(nimValues[rowValue]);
                    rowValue = 0;
                }

                i++;
            }

            i = 0;
            while (i < valuesHolder.Count) //Przejście po kontenerze wartości i przypisanie wartości nim do ruchu
            {
                nimValue ^= valuesHolder[i];
                i++;
            }

            if (nimValue == 0) //Jeżeli ruch jest wygrywający, wypisze go na konsolę
                Console.WriteLine(_kaylesGame);
        }
    }
}
