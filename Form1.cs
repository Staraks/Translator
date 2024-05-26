using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Lexer;

namespace Translator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void PrintLexerResult(List<string> source)
        {
            resultTextBox.Text = string.Empty;
            foreach (string item in source)
            {
                resultTextBox.Text += $"{item}\n";
            }
        }

        private List<string> InitMatrix(List<string> rpn)
        {
            List<string> matrix = new List<string>();
            Stack<string> operands = new Stack<string>();
            string buffer = string.Empty;
            int mark;

            foreach (string item in rpn)
            {
                matrix.Add("\n" + item);
                mark = 0;
                if (item.Length == 2)
                {
                    matrix.Add($"M{mark}   {item}");
                    continue;
                }
                for (int i = 0; i < item.Length; i++)
                {
                    if (item[i] != ' ' && i < item.Length - 1)
                    {
                        buffer += item[i];
                        continue;
                    }
                    
                    if ((buffer.Length == 1 && !Lexer.IsSpecialSymbol(buffer[0])) || (buffer.Length > 1 && !Lexer.IsSpecialWord(buffer)))
                    {
                        operands.Push(buffer);
                        buffer = string.Empty;
                    }
                    else
                    {
                        matrix.Add($"M{mark}   {buffer} {operands.Pop()} {operands.Pop()}");
                        operands.Push($"M{mark}");
                        mark++;
                        buffer = string.Empty;
                    }
                }
            }
            return matrix;
        }

        private void ShowExprResult(List<string> matrix)
        {
            exprTextBox.Clear();
            foreach (string item in matrix)
            {
                exprTextBox.AppendText(item + '\n');
            }
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            string source = sourceTextBox.Text;
            Lexer lexer = new Lexer(source);
            try
            {
                List<string> res = lexer.Analyze();
                PrintLexerResult(res);
                lexer.LexemeType.Add(TokenType.E);
                LRParser parser = new LRParser(lexer.LexemeType, res);
                parser.Analyze();
                ShowExprResult(InitMatrix(parser.ReversePolishNotation));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                sourceTextBox.Text = File.ReadAllText(dialog.FileName);
            }
        }
    }
}