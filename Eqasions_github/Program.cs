/*
Задание
Операторы +, -, >, <, =, & и |, при этом нужно учитывать, что приоритет &
выше, чем |, остальные операторы имеют равный приоритет, но выше, чем
логические операторы & и |.

Приоритет операций может меняться круглыми скобками.

Данное приложение должно получать на вход что-то типа: (a + b > с | c < d)
& d = e, на выходе нужно вывести только логические операции в порядке их
выполнения, т.е.:

1. a + b > с | c < d

2. (a + b > с | c < d)  & d = e 
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace equasions
{
    class Comparator
    {
        //строка которая будет содержать введенное неравенство
        public string Expression { get; private set; }
        //массив строк после первого разделения по рангу операций
        public string[] Separated;
        //массив строк после второго разделения по рангу операций
        public string[] SeparatedFinal;
        //соответствующие индексаторы двух массивов
        private int SeparatedIndex = 0;
        private int SeparatedFinalIndex = 0;
        //специальные строки для проверки обрабатываемого символа
        //Вероятно, было бы лучше сделать их константами?
        private string MediumLevelActions = "|&";
        private string SpecialActions = "()|&";

        /// <summary>
        /// Считывание неравенства
        /// </summary>
        public void Process()
        {
            //убираем все пробелы
            Expression = Console.ReadLine().Replace(" ", "");
            //созание двух массивов на основе длинны введенного значения
            Separated = new string[Expression.Length];
            SeparatedFinal = new string[Expression.Length];

            //создание массива для карты скобок и передача управления
            int[,] brc = Brackets();
            FirstSeparator(brc);
            SecondSeparator();
            FinaL(brc);
        }


        /// <summary>
        /// Создание карты скобок, впоследствии послужит для облегчения нескольких участков обработки неравенства
        /// </summary>
        /// <returns>Возвращает полную карту скобок</returns>
        public int[,] Brackets()
        {
            //основываясь на том, что скобка, открытая позже других, закроется раньше всех, я выбрал стак для хранения начальной координаты открытой скобки
            Stack<int> OpBrackets = new Stack<int>();
            //Массив хранящий карту скобок, длинна массива равна кол-ву открытых скобок
            int[,] B_coords = new int[Expression.Count(x => x == '(') + 1, 2];
            //индексатор карты
            int Ib_coords = 0;

            //реализация принципа LIFO для открытых скобок и сопоставление с индексами открытия/закрытия
            for (int i = 0; i < Expression.Length; i++)
            {
                if (Expression[i] == '(')
                {
                    OpBrackets.Push(i);
                }


                if (Expression[i] == ')')
                {
                    B_coords[Ib_coords, 0] = OpBrackets.Pop();
                    B_coords[Ib_coords, 1] = i;
                    Ib_coords++;
                }
            }
            //возвращаем карту
            return B_coords;
        }

        /// <summary>
        /// Разбивает неравенство по приоритету операций и по скобкам, в конце "свободными" останутся только | и &
        /// </summary>
        /// <param name="map">Карта скобок</param>
        public void FirstSeparator(int[,] map)
        {
            //временная строка для хранения значений будущего массива первого прохода
            string temp = "";
            for (int i = 0; i < Expression.Length; i++)
            {
                //посимвольная проверка, все, что не является скобкой или |/& добавляется в временную строку
                if (!SpecialActions.Contains(Expression[i]))
                {
                    temp += Expression[i];
                }
                //при появлении открывающей скобки сразу сгружает все содержимое скобок в временную переменную и увеличивает итератор прохода на соответствующее число тактов
                if (Expression[i] == '(')
                {
                    for (int row = 0; row < map.Length / 2; row++)
                    {
                        if (map[row, 0] == i)
                        {
                            temp += Expression.Substring(map[row, 0], map[row, 1] - map[row, 0] + 1);
                            i += map[row, 1] - map[row, 0];
                            break;
                        }
                    }
                }

                //"столкновение" с сепараторами первого прохода записывает строку в массив первого разделения и опусташает временную строку, сам сепаратор записывается в отдельную ячейку для дальнейшего прохода
                if (MediumLevelActions.Contains(Expression[i]))
                {

                    Separated[SeparatedIndex] = temp;
                    SeparatedIndex++;

                    Separated[SeparatedIndex] = Expression[i].ToString();
                    SeparatedIndex++;

                    temp = "";
                }
            }
            //дозапись последнего отрезка в массив первого разделения
            Separated[SeparatedIndex] = temp;
        }

        //Второе разделение работает аналогично первому, но в меньших масштабах
        public void SecondSeparator()
        {
            string temp = "";
            //собираем в отдельные строки уже завершенные действия с &, | остается отдельно
            foreach (string a in Separated)
            {
                if (a != null)
                {
                    switch (a)
                    {
                        case "|":
                            SeparatedFinal[SeparatedFinalIndex] = temp;
                            SeparatedFinalIndex++;

                            SeparatedFinal[SeparatedFinalIndex] = a;
                            SeparatedFinalIndex++;

                            temp = "";
                            break;
                        default:
                            temp += a;
                            break;
                    }
                }
            }
            //дозапись последнего отрезка в массив второго разделения
            SeparatedFinal[SeparatedFinalIndex] = temp;
        }

        //Финальная стадия, вывод результата на экран
        public void FinaL(int[,] map)
        {
            //временная строка для разделения вывода
            string temp = "";

            //вначале на экран выведутся действия в скобках, которые априори делаются в первую очередь
            Console.WriteLine();
            Console.WriteLine("==========================PROCESSING BRACKETS==========================");
            for (int row = 0; row < map.Length / 2; row++)
            {
                string maprow = Expression.Substring(map[row, 0], map[row, 1] - map[row, 0] + 1);
                if (maprow.Contains('&') || maprow.Contains('|'))
                {
                    Console.WriteLine(maprow);
                }
            }

            //далее поочередный вывод действий приоритетом выше чем |
            Console.WriteLine();
            Console.WriteLine("==========================HIGHER PRIORITY==========================");
            foreach (string b in SeparatedFinal)
            {
                if (b != null && b.Contains('&'))
                {
                    Console.WriteLine(b);
                }
            }
            //индексатор для нумерации строк
            int ind = 1;
            //Вывод окончательного порядка выполнения, каждый раз дополняя по порядку выводимую строку.
            Console.WriteLine();
            Console.WriteLine("==========================OUTPUT - PROCESSED ORDER==========================");
            foreach (string a in SeparatedFinal)
            {
                if (a != null)
                {
                    switch (a)
                    {
                        case "|":
                            temp += a;
                            break;
                        default:
                            temp += a;
                            Console.WriteLine(ind + " " + temp);
                            ind++;
                            break;
                    }
                }
            }
        }

        //Несколько тестовых значений использованных во время тестирования работоспособности
        // ((a + b > с) & (c < d)) | (s+s) & (e-d|s) = e
        // ((a + b > с) & (c < d)) & (s+s) + (e-d|s) = e
        // ((a + b > с) & (c < d)) | (s+s) + (e-d|s) = e
        // a+b>c|c<d&(d=e)
        // a+b&a+b&a+b|a-s|a-s|a&b|a&s

        class Program
        {
            static void Main(string[] args)
            {
                Comparator a = new Comparator();
                a.Process();
            }
        }
    }
}
