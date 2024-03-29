﻿using System;
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
            ClearAll();
            TINY_Compiler.Start_Compiling(textBox1.Text);
            PrintTokens();
            treeView1.Nodes.Add(Parser.PrintParseTree(TINY_Compiler.treeroot));
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
            ClearAll();
            textBox1.Text ="";
        }

        private void ClearAll()
        {
            textBox2.Text = "";
            TINY_Compiler.TokenStream.Clear();
            dataGridView1.Rows.Clear();
            treeView1.Nodes.Clear();
            Errors.Error_List.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.AcceptsTab = true;
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
