using System;

namespace WdTG
{
    class Zad6 //Gra z krążkami na stosie - wynik to wartość gry
    {
        private const string stack = "WBWBB"; //Stos początkowy z polecenia, patrząc od dołu

        static void Main(string[] args)
        {
            float gameValue = 0.0f;
            gameValue += CalculateGameValue(0, stack[0]); //Rekurencyjne obliczanie wartości gry

            Console.WriteLine("Wartość gry: " + stack + " wynosi: " + gameValue); //Wyświetla wynik
            Console.ReadKey(); //Do zatrzymania konsoli
        }

        private static float CalculateGameValue(int _index, char _firstColor, bool _stillFirstColor = true, float _divider = 1)
        {
            if(_index < stack.Length) //Tak długo aż nie dojdzie do końca stringa przechowującego grę
            {
                if (!stack[_index].Equals(_firstColor)) //Sprawdzenie czy obecny krążek jest tego samego koloru co pierwszy
                    _stillFirstColor = false;

                if(_stillFirstColor) //Dodawanie wartości: 1 jeżeli krążek czarnego leży na dole; -1 jeżeli krążek białego leży na dole
                {
                    if (stack[_index].Equals('B'))
                        return CalculateGameValue(_index + 1, _firstColor) + 1;
                    else if (stack[_index].Equals('W'))
                        return CalculateGameValue(_index + 1, _firstColor) - 1;
                }
                else //W innym przypadku obliczanie kolejnych ruchów (zdejmowania krążków)
                {
                    _divider *= 0.5f; //Jest dwoje graczy - przez co wartość gry będzie zmieniała się za każdym razem o ten sam współczynnik zmniejszany o połowę przy każdym ruchu
                    if (stack[_index].Equals('B'))
                        return CalculateGameValue(_index + 1, _firstColor, false, _divider) + _divider;
                    else if (stack[_index].Equals('W'))
                        return CalculateGameValue(_index + 1, _firstColor, false, _divider) - _divider;
                }
            }
            return 0; //Po zakończeniu wszystkich ruchów zwraca 0
        }
    }
}
