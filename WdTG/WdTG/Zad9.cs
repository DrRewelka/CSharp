using System;

namespace WdTG
{
    class Zad9 //Gra na szachownicy
    {
        private const int checkerSize = 8; //Rozmiar szachownicy
        private static char[,] checker = new char[checkerSize, checkerSize]; //Utworzenie tablicy - szachownicy, dwuwymiarowej, o odpowiednim rozmiarze

        static void Main(string[] args)
        {
            int numberOfWinningPositions = CalculateNumberOfWinningPositions(); //Obliczenie liczby wygrywających pól

            Console.WriteLine("Liczba pól, z których pierwszy gracz wygra: " + numberOfWinningPositions); //Wyświetla liczę wygrywających pozycji dla rozpoczynającego gracza
            Console.ReadKey(); //Zatrzymanie konsoli
        }

        private static int CalculateNumberOfWinningPositions()
        {
            int numberOfWinningPositions = checkerSize * checkerSize; //Obliczenie maksymalnej liczby wygrywających pól (cała szachownica)
            int i = 0; //Cztery zmienne pomocnicze, żeby nie trzeba było deklarować ich później
            int j = 0;
            int k = 0;
            int l = 0;

            while (i < checkerSize) //Wypełnienie szachownicy znakami '*' - łatwiej się porównuje i nie wymaga tak dużej ilości kodu
            {
                while (j < checkerSize)
                {
                    checker[i, j] = '*';
                    j++;
                }
                j = 0;
                i++;
            }

            i = 0;

            checker[checkerSize - 1, 0] = '0'; //Pierwsza przegrana sytuacja gdy gracz zacznie jedno pole nad lewym dolnym rogiem szachownicy
            numberOfWinningPositions--;

            while(i < checkerSize) //Uzupełnienie szachownicy w górę, w prawo oraz po przekątnej od lewego dolnego rogu sytuacjami wygrywającymi - gracz może wygrać jeżeli tam zacznie
            {
                if (checker[checkerSize - 1, i].Equals('*'))
                    checker[checkerSize - 1, i] = '1';
                if (checker[checkerSize - 1 - i, 0].Equals('*'))
                    checker[checkerSize - 1 - i, 0] = '1';
                if (checker[checkerSize - 1 - i, i].Equals('*'))
                    checker[checkerSize - 1 - i, i] = '1';

                i++;
            }

            i = 0;

            while(i < checkerSize) //Sprawdzanie każdego pola na szachownicy w poszukiwaniu sytuacji, w której gracz na pewno przegra jeżeli tam zacznie
            {
                while(j < checkerSize)
                {
                    while(k < checkerSize)
                    {
                        if (k > 0 && j < 7) //Warunek potrzebny, aby nie wyjść poza indeksy tablicy
                        {
                            if (checker[j + 1, k].Equals('1') && checker[j + 1, k - 1].Equals('1') && checker[j, k - 1].Equals('1'))
                            {
                                if (checker[j, k].Equals('*')) //Sprawdzenie pola i jego sąsiadów, jeżeli jest "puste" (ma '*'), a każda sytuacja w jego sąsiedztwie
                                {                              //w kierunku lewego dolnego rogu szachownicy jest równa 1, to jest to sytuacja przegrana
                                    checker[j, k] = '0';
                                    numberOfWinningPositions--;

                                    while (l < checkerSize) //Sprawdzane są ruchy z wybranego wcześniej pola w górę i prawo czy nie są "puste", jeżeli tak to wpisywana jest 1, ponieważ sytuacja będzie wygrana
                                    {
                                        if (j - l >= 0) //Warunki potrzebne aby nie wyjść poza zakres tablicy
                                        {
                                            if (checker[j - l, k].Equals('*'))
                                                checker[j - l, k] = '1';
                                        }
                                        if (j - l >= 0 && k + l <= 7)
                                        {
                                            if (checker[j - l, k + l].Equals('*'))
                                                checker[j - l, k + l] = '1';
                                        }
                                        if(k + l <= 7)
                                        { 
                                            if (checker[j, k + l].Equals('*'))
                                                checker[j, k + l] = '1';
                                        }

                                        l++;
                                    }
                                }
                            }
                        }
                        
                        l = 0;
                        k++;
                    }

                    k = 0;
                    j++;
                }

                j = 0;
                i++;
            }

            return numberOfWinningPositions;
        }
    }
}