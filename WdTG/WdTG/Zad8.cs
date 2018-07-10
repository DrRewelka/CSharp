using System;

namespace WdTG
{
    class Zad8 //Gra z dwoma urnami
    {
        static void Main(string[] args)
        {
            int firstUrn = 50; //Początkowe liczby kul w urnach
            int secondUrn = 50;
            int ballsNumber = 50; //Ogólna liczba kul przeznaczonych do użytku (jedna urna będzie pusta)
            int movesCounter = 0; //Licznik ruchów - na końcu jest dzielony przez 2, ponieważ ruch wykonują naprzemiennie dwaj gracze
            Random rand = new Random();

            while (Math.Max(firstUrn, secondUrn) > 1)
            {
                int urnPicker = rand.Next(2); //Losowy wybór urny: 0 - pierwsza, 1 - druga

                if (firstUrn == 2) //Warunki do wybrania optymalnej urny
                    urnPicker = 1;
                else if (secondUrn == 2)
                    urnPicker = 0;
                else if (firstUrn > secondUrn)
                    urnPicker = 1;
                else if (secondUrn > firstUrn)
                    urnPicker = 0;

                if (urnPicker == 0)
                {
                    //Faza pierwsza - opróżnienie urny
                    firstUrn = 0; //Opróżnienie wybranej urny

                    //Faza druga - rozdzielenie kul z drugiej urny
                    ballsNumber = secondUrn; //Wyciągnięcie kul z innej urny niż wybrana
                    secondUrn = 0;

                    if (ballsNumber > 5)
                    {
                        if (ballsNumber == 6)
                        {
                            firstUrn += 1;
                            secondUrn += 5;
                        }
                        else
                        {
                            firstUrn += 1; //Przypisanie po jednej kuli do każdej urny
                            secondUrn += 1;
                            ballsNumber -= 2; //Dwie kule zostały zabrane

                            firstUrn += ballsNumber / 2; //Rozłożenie kul po równo do każdej urny
                            secondUrn += ballsNumber - ballsNumber / 2;
                        }
                    }
                    else //Strategie wygrywające dla małej liczby kul
                    {
                        if (ballsNumber == 4)
                        {
                            firstUrn += 3;
                            secondUrn += 1;
                        }
                        else
                        {
                            firstUrn += 1;
                            secondUrn += ballsNumber - firstUrn;
                        }
                    }                    
                }
                else
                {
                    //Faza pierwsza - opróżnienie urny
                    secondUrn = 0; //Opróżnienie wybranej urny

                    //Faza druga - rozdzielenie kul z drugiej urny
                    ballsNumber = firstUrn; //Wyciągnięcie kul z innej urny niż wybrana
                    firstUrn = 0;

                    if (ballsNumber > 5)
                    {
                        if (ballsNumber == 6)
                        {
                            firstUrn += 5;
                            secondUrn += 1;
                        }
                        else
                        {
                            firstUrn += 1; //Przypisanie po jednej kuli do każdej urny
                            secondUrn += 1;
                            ballsNumber -= 2; //Dwie kule zostały zabrane

                            firstUrn += ballsNumber / 2; //Rozłożenie kul po równo do każdej urny
                            secondUrn += ballsNumber - ballsNumber / 2;
                        }
                    }
                    else //Strategie wygrywające dla małej liczby kul
                    {
                        if (ballsNumber == 4)
                        {
                            firstUrn += 3;
                            secondUrn += 1;
                        }
                        else
                        {
                            secondUrn += 1;
                            firstUrn += ballsNumber - secondUrn;
                        }
                    }       
                }

                Console.WriteLine(firstUrn + ", " + secondUrn);
                movesCounter++;

                if (firstUrn == 1 && secondUrn == 1) //Jeżeli po dodaniu kul do urn nadal występuje po 1 kuli na urnę, następuje koniec gry
                    break;
            }

            Console.WriteLine("Wartość gry: *" + movesCounter / 2); //Podzielone przez 2 ponieważ jest dwóch graczy, ruszają się naprzemiennie, co należy zasymulować
                                                                    //Wynikiem jest liczba ruchów, które doprowadzają do gry typu pierwszy gracz wygrywa
            Console.ReadKey();
        }
    }
}
