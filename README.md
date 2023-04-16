# tiny-lang

**tiny-lang** is a simple programming language developed as a part of a college project for the subject Compiler Theory. It includes the implementation of the lexical and syntax analysis phases of a compiler. The purpose of this project is to demonstrate how a compiler can take a stream of characters and convert it into a sequence of tokens, which can then be analyzed by the syntax analyzer to determine whether the input is a valid program.

## Features

The **tiny-lang** interpreter supports the following features:

- Lexical analysis of input stream to generate tokens
- Syntax analysis of tokens to determine the validity of the input
- Error handling for invalid input
- Output of the input program's abstract syntax tree (AST)

## Installation

To install the **tiny-lang** interpreter locally, follow these steps:

1. Clone the repository: `git clone https://github.com/Omar-Yasser/tiny-lang.git`
2. Open the solution file `tiny-lang.sln` using Visual Studio or a similar C# IDE.
3. Build the solution by clicking on the Build option from the menu.
4. Run the interpreter by setting the input file path and clicking on the Run button.

Note: Please ensure that you have Visual Studio or a similar C# IDE installed on your system in order to build the solution.

## Usage

To run a **tiny-lang** program, create a file with the `.tiny` extension and include your code. For example:
```c++
// example tiny
int main() {
int x = 5;
if (x > 3) {
Console.WriteLine("Hello, World!");
}
return 0;
}
```
To execute the program, set the input file path in the `Program.cs` file and run the project.


## Team Members
- [@mh084449](https://github.com/mh084449)
- [@tenafrangelos](https://github.com/tenafrangelos)
- [@Omar-Yasser](https://github.com/Omar-Yasser) 
- [@Yosef-Mahmoud](https://github.com/Silverhorse7) 
- [@Salma-Ayman](https://github.com/SalmaAlassal)
