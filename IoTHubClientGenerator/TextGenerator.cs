using System;
using System.Text;

namespace IoTHubClientGenerator
{
    public class TextGenerator
    {
        private int _nestingLevel;
        private readonly StringBuilder _sb = new();

        private class Back : IDisposable
        {
            private readonly TextGenerator _textGenerator;

            public Back(TextGenerator textGenerator)
            {
                _textGenerator = textGenerator;
            }

            public void Dispose()
            {
                --_textGenerator._nestingLevel;
            }
        }

        private class CloseStatement : IDisposable
        {
            private readonly TextGenerator _textGenerator;
            private readonly string _closingText;

            public CloseStatement(TextGenerator textGenerator, string closingText = "}")
            {
                _textGenerator = textGenerator;
                _closingText = closingText;
            }

            public void Dispose()
            {
                --_textGenerator._nestingLevel;
                _textGenerator._sb.AppendLine(_closingText);
            }
        }

        private class Nothing : IDisposable
        {
            public static readonly IDisposable Todo = new Nothing();

            public void Dispose()
            {
            }
        }

        public string Result => _sb.ToString();
        public void AppendLine(string line = "", bool condition = true)
        {
            if (!condition)
                return;
            _sb.Append('\t', _nestingLevel);
            _sb.AppendLine(line);
        }

        public void Append(string line = "", bool isIndented = false, bool condition = true)
        {
            if (!condition)
                return;

            if (isIndented)
                _sb.Append('\t', _nestingLevel);
            _sb.Append(line);
        }

        public void TrimEnd(int n = 1)
        {
            _sb.Remove(_sb.Length - n, n);
        }

        private IDisposable Statement(string statement, string expression, bool condition = true, string closingText = "}")
        {
            if (!condition)
                return Nothing.Todo;

            _sb.AppendLine(expression == null ? $"{statement}" : $"{statement} ({expression})");
            _sb.AppendLine("{");
            ++_nestingLevel;
            return new CloseStatement(this, closingText);
        }

        public IDisposable If(string expression, bool condition = true)
        {
            return Statement("if", expression, condition);
        }

        public IDisposable ElseIf(string expression, bool condition = true)
        {
            return Statement("else if", expression, condition);
        }

        public IDisposable Else(bool condition = true)
        {
            return Statement("else", null, condition);
        }

        public IDisposable While(string expression, bool condition = true)
        {
            return Statement("while", expression, condition);
        }

        public IDisposable For(string expression, bool condition = true)
        {
            return Statement("for", expression, condition);
        }

        public IDisposable Foreach(string expression, bool condition = true)
        {
            return Statement("foreach", expression, condition);
        }

        public IDisposable Switch(string expression, bool condition = true)
        {
            return Statement("switch", expression, condition);
        }

        public IDisposable Try(bool condition = true)
        {
            return Statement("try", null, condition);
        }

        public IDisposable Catch(string expression, bool condition = true)
        {
            return Statement("catch", expression, condition);
        }

        public IDisposable Do(string expression, bool condition = true)
        {
            return Statement("do", null, condition, $"}} while ({expression});");
        }

        public IDisposable Block(string closingText = "}")
        {
            return Block(true, closingText);
        }
        

        public IDisposable Block(bool condition, string closingText = "}")
        {
            if (!condition)
                return Nothing.Todo;

            _sb.AppendLine("{");
            ++_nestingLevel;
            return new CloseStatement(this, closingText);
        }
    }
}