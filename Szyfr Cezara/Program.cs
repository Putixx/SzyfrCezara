using System.Diagnostics;
using System.Text;
using System.IO;
using System;

namespace Szyfr_Cezara
{
    class Program
    {
        //Zmienne potrzebne do obsługi programu
        private const string lower = "aąbcćdeęfghijklłmnńoóprsśtuwyzźż";
        private const string upper = "AĄBCĆDEĘFGHIJKLŁMNŃOÓPRSŚTUWYZŹŻ";
        private const string digits = "0123456789";
        private const string dictionaryPath = "slowa.txt";
        private const string resultsPath = "results.txt";
        static private Stopwatch timer = new Stopwatch();
        static private StreamWriter s;

        //Zmiana Encode z Unicode na UTF-8
        static public string EncodeToUTF8(string value)
        {
            //Konwertowanie stringów na tablice byte
            byte[] valueUnicodeBytes = Encoding.Unicode.GetBytes(value);

            //Konwertowanie kodowania z unicode na utf8
            byte[] valueUtf8Bytes = Encoding.Convert(Encoding.Unicode, Encoding.UTF8, valueUnicodeBytes);

            //Konwertowanie tablicy Byte utf-8 na string
            char[] valueUtf8Chars = new char[Encoding.UTF8.GetCharCount(valueUtf8Bytes, 0, valueUtf8Bytes.Length)];
            Encoding.UTF8.GetChars(valueUtf8Bytes, 0, valueUtf8Bytes.Length, valueUtf8Chars, 0);
            string utf8String = new string(valueUtf8Chars);

            return utf8String;
        }

        //Szyfrowanie słowa
        static public void Szyfruj(string value, byte key)
        {
            //Zmienna do przechowania szyfru, zmieniamy jej typ kodowania na UTF-8
            string szyfr = "";
            szyfr = EncodeToUTF8(szyfr);

            //Szyfrowanie słowa
            for (int i = 0; i < value.Length; i++)
            {
                //Jeśli w ciągu są małe litery
                if (lower.Contains(value[i]))
                {
                    for (int j = 0; j < lower.Length; j++)
                    {
                        if (lower[j] == value[i])
                        {
                            szyfr += lower[(j + key) % lower.Length];
                            break;
                        }
                    }
                }
                //Jeśli w ciągu są duże litery
                if (upper.Contains(value[i]))
                {
                    for (int j = 0; j < upper.Length; j++)
                    {
                        if (upper[j] == value[i])
                        {
                            szyfr += upper[(j + key) % upper.Length];
                            break;
                        }
                    }
                }
                //Jeśli w ciągu są cyfry
                if (digits.Contains(value[i]))
                {
                    for (int j = 0; j < digits.Length; j++)
                    {
                        if (digits[j] == value[i])
                        {
                            szyfr += digits[(j + key) % digits.Length];
                            break;
                        }
                    }
                }
            }

            //Wyświetlenie zaszyfrowanego słowa
            Console.Write("\n\n--------------------------------------------");
            Console.Write("\nWynik: " + szyfr + "\n");
            Console.Write("--------------------------------------------\n\n");
        }

        //Deszyfrowanie słowa
        static public void Deszyfruj(string value)
        {
            //Zmienne potrzebne do zapisania czasu jaki minął podczas deszyfrowania
            TimeSpan ts;
            string time;

            //Zmienna do zapisu deszyfru
            string deszyfr = "";
            deszyfr = EncodeToUTF8(deszyfr);

            //Licznik ile słów pasuje
            int licznik = 0;

            //Sprawdzenie ile linijek ma słownik
            int dictionaryLength = 0;
            foreach (string line in File.ReadLines(dictionaryPath))
                if (line != String.Empty) ++dictionaryLength;

            //Przejście przez wszystkie możliwe klucze
            for (int key = 0; key < lower.Length; key++)
            {
                //Zaczynamy mierzyć czas dla jednego przejścia
                timer.Start();

                //Zmienna do zapisu deszyfru
                deszyfr = "";

                //Zmiana każdej litery w zaszyfrowanym słowie o przesunięcie klucza
                for (int i = 0; i < value.Length; i++)
                {
                    //Jeśli w ciągu są małe litery
                    if (lower.Contains(value[i]))
                    {
                        for (int j = 0; j < lower.Length; j++)
                        {
                            if (lower[j] == value[i])
                            {
                                deszyfr += lower[(j + key) % lower.Length];
                                break;
                            }
                        }
                    }
                    //Jeśli w ciągu są cyfry
                    if (digits.Contains(value[i]))
                    {
                        for (int j = 0; j < digits.Length; j++)
                        {
                            if (digits[j] == value[i])
                            {
                                deszyfr += digits[(j + key) % digits.Length];
                                break;
                            }
                        }
                    }
                }

                //Sprawdzenie czy istnieje plik słownika
                if (!File.Exists(dictionaryPath))
                {
                    Console.WriteLine($"{dictionaryPath} nie istnieje!");
                    return;
                }

                //Jeśli plik słownika istnieje
                else
                {
                    //Sprawdzenie czy istnieje plik wyników, jeśli nie to tworzymy go
                    if (!File.Exists(resultsPath))
                        File.Create(resultsPath).Close();

                    //Jeśli plik wyników istnieje
                    else
                    {
                        for (int j = 0; j < deszyfr.Length; j++)
                        {
                            for (int k = 0; k < deszyfr.Length - j; k++)
                            {
                                //Sprawdzamy literka po literce istniejące w ciągu słowa
                                string subDeszyfr = deszyfr.Substring(j, k);

                                //Sprawdzenie wszystkich linii pliku słownika i sprawdzenie zgodnych słów, a następnie wypisanie ich w konsoli razem z czasem potrzebnym na odszyfrowanie ich
                                foreach (string line in File.ReadLines(dictionaryPath))
                                {
                                    if (line == subDeszyfr)
                                    {
                                        ts = timer.Elapsed;
                                        time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
                                        Console.WriteLine($"Czas: {time}, sprawdzono {key + 1} kluczy. Słowo: {line}");
                                        licznik++;
                                        break;
                                    }
                                }
                            }
                        }
                        //Czas potrzebny do sprawdzenia jednego klucza
                        ts = timer.Elapsed;
                        time = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

                        //Zapis do pliku wyników
                        s = File.AppendText(resultsPath);
                        s.WriteLine($"Czas: {time}, Sprawdzany klucz: {key + 1}, value: {deszyfr}, ilość pasujących słów: {licznik}");
                        s.Close();
                    }
                }
                //Zatrzymanie timera
                timer.Stop();
            }
            //Dopisanie do pliku wyników oraz pustego entera dla lepszej przejrzystości
            s = File.AppendText(resultsPath);
            s.WriteLine($"Wynik działania: ilość sprawdzonych kluczy: {lower.Length}, łączna ilość pasujących słów: {licznik}, procentowa wartość pasujących, do wszystkich słów: {String.Format("{0:N10}", ((double)licznik / dictionaryLength) * 100)}%");
            s.WriteLine("");
            s.Close();
        }


        static void Main(string[] args)
        {
            Console.Write("Szyfr cezara. \n1. Szyfruj. \n2. Deszyfruj. \nWybierz co chcesz zrobić: ");

            //Zmienne potrzebne do obsługi programu
            bool isTrue = true;
            byte choose = 0, key;
            string value;

            //Pętla while dopóki użytkownik nie wpisze poprawnej wartości
            while (isTrue)
            {
                if (!Byte.TryParse(Console.ReadLine(), out choose))
                    Console.Write("\nWprowadzono błędną wartość. Proszę spróbować ponownie: ");

                else
                {
                    if (!(choose == 1 || choose == 2))
                        Console.Write("\nWprowadzono błędną wartość. Proszę spróbować ponownie: ");
                    else
                        isTrue = false;
                }
            }

            //Po otrzymaniu od użytkownika odpowiedniej wartości są dwie możliwości
            switch (choose)
            {
                //Przypadek kiedy słowo będzie szyfrowane
                case 1:
                    {
                        isTrue = true;
                        Console.Write("Podaj słowo do zaszyfrowania: ");
                        value = Console.ReadLine();
                        Console.Write("\nPodaj klucz/zmiane 1-32: ");

                        //Pętla while dopóki użytkownik nie wpisze poprawnej wartości
                        while (isTrue)
                        {
                            if (!Byte.TryParse(Console.ReadLine(), out key))
                                Console.Write("\nWprowadzono błędną wartość. Proszę spróbować ponownie: ");
                            else
                            {
                                if (key < 1 || key > 32)
                                    Console.Write("\nWprowadzono błędną wartość. Proszę spróbować ponownie: ");

                                else
                                {
                                    //Wywołanie funkcji szyfrowania
                                    Szyfruj(EncodeToUTF8(value), key);
                                    isTrue = false;
                                }
                            }
                        }
                        break;
                    }
                //Przypadek kiedy słowo będzie deszyfrowane
                case 2:
                    {
                        Console.Write("Podaj słowo do zdeszyfrowania: ");

                        //Wywołanie funkcji deszyfrowania
                        Deszyfruj(EncodeToUTF8(Console.ReadLine().ToLower()));
                        break;
                    }
                default:
                    break;
            }
        }
    }
}