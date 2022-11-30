using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TINY_Compiler
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            //string Code=textBox1.Text.ToLower();
            string Code = textBox1.Text;
            TINY_Compiler.Start_Compiling(Code);
            PrintTokens();
            //   PrintLexemes();

            PrintErrors();
        }
        void PrintTokens()
        {
            for (int i = 0; i < TINY_Compiler.Tiny_Scanner.Tokens.Count; i++)
            {
                dataGridView1.Rows.Add(TINY_Compiler.Tiny_Scanner.Tokens.ElementAt(i).lex, TINY_Compiler.Tiny_Scanner.Tokens.ElementAt(i).token_type);
            }
        }

        void PrintErrors()
        {
            for (int i = 0; i < Errors.Error_List.Count; i++)
            {
                textBox2.Text += Errors.Error_List[i];
                textBox2.Text += "\r\n";
            }
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            textBox2.Clear();
            Errors.Error_List.Clear();
            TINY_Compiler.TokenStream.Clear();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        /*  void PrintLexemes()
{
for (int i = 0; i < TINY_Compiler.Lexemes.Count; i++)
{
textBox2.Text += TINY_Compiler.Lexemes.ElementAt(i);
textBox2.Text += Environment.NewLine;
}
}*/
    }
}
