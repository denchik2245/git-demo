﻿using System.Globalization;

namespace LabsForCsu
{
    // Базовый класс для всех токенов
    public abstract class Token { }

    // Класс для представления чисел
    public class Number : Token
    {
        public double Value { get; private set; }

        public Number(double value)
        {
            Value = value;
        }
    }

    // Класс для представления операций
    public class Operation : Token
    {
        public char Symbol { get; private set; }
        public int Priority => Symbol switch
        {
            '*' or '/' => 2,
            '+' or '-' => 1,
            _ => 0
        };

        public Operation(char symbol)
        {
            Symbol = symbol;
        }
    }

    // Класс для представления скобок
    public class Parenthesis : Token
    {
        public char Symbol { get; private set; }

        public Parenthesis(char symbol)
        {
            Symbol = symbol;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите математическое выражение:");
            var input = Console.ReadLine();

            var tokens = Tokenize(input);

            Console.WriteLine("\nОбратная польская запись: ");
            var postfix = ConvertToPostfix(tokens);
            foreach (var token in postfix)
            {
                if (token is Number number)
                    Console.Write($"{number.Value} ");
                else if (token is Operation operation)
                    Console.Write($"{operation.Symbol} ");
            }

            Console.WriteLine("\n\nРезультат вычисления: ");
            Console.WriteLine(EvaluatePostfix(postfix));
        }

        // Метод для токенизации входной строки
        static List<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();
            var currentNum = "";

            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];

                if (char.IsDigit(ch) || ch == '.')
                {
                    currentNum += ch;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(currentNum))
                    {
                        tokens.Add(new Number(double.Parse(currentNum, CultureInfo.InvariantCulture)));
                        currentNum = "";
                    }

                    if ("+-*/".Contains(ch))
                    {
                        tokens.Add(new Operation(ch));
                    }
                    else if (ch == '(' || ch == ')')
                    {
                        tokens.Add(new Parenthesis(ch));
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(currentNum))
            {
                tokens.Add(new Number(double.Parse(currentNum, CultureInfo.InvariantCulture)));
            }

            return tokens;
        }

        // Метод для преобразования списка токенов в ОПЗ
        static List<Token> ConvertToPostfix(List<Token> tokens)
        {
            var postfix = new List<Token>();
            var operationStack = new Stack<Operation>();

            foreach (var token in tokens)
            {
                if (token is Number number)
                {
                    postfix.Add(number);
                }
                else if (token is Operation operation)
                {
                    while (operationStack.Count != 0 && operationStack.Peek().Priority >= operation.Priority)
                    {
                        postfix.Add(operationStack.Pop());
                    }
                    operationStack.Push(operation);
                }
                else if (token is Parenthesis parenthesis)
                {
                    if (parenthesis.Symbol == '(')
                    {
                        operationStack.Push(new Operation(parenthesis.Symbol)); // Это временное решение для скобок, их нужно будет обработать отдельно.
                    }
                    else
                    {
                        while (operationStack.Count > 0 && operationStack.Peek().Symbol != '(')
                        {
                            postfix.Add(operationStack.Pop());
                        }
                        if (operationStack.Count > 0 && operationStack.Peek().Symbol == '(')
                        {
                            operationStack.Pop();
                        }
                    }
                }
            }

            while (operationStack.Count != 0)
            {
                postfix.Add(operationStack.Pop());
            }

            return postfix;
        }

        // Метод для вычисления значения выражения в ОПЗ
        static double EvaluatePostfix(List<Token> postfix)
        {
            var values = new Stack<double>();

            foreach (var token in postfix)
            {
                if (token is Number number)
                {
                    values.Push(number.Value);
                }
                else if (token is Operation operation)
                {
                    double a = values.Pop();
                    double b = values.Pop();
                    values.Push(ApplyOperation(operation.Symbol, b, a));
                }
            }

            return values.Pop();
        }

        // Метод для вычисления значения двух переменных
        static double ApplyOperation(char op, double a, double b)
        {
            switch (op)
            {
                case '+': return a + b;
                case '-': return a - b;
                case '*': return a * b;
                case '/':
                    if (b == 0)
                        throw new DivideByZeroException("Попытка деления на ноль.");
                    return a / b;
                default:
                    throw new ArgumentException($"Неподдерживаемая операция: {op}");
            }
        }
    }
}