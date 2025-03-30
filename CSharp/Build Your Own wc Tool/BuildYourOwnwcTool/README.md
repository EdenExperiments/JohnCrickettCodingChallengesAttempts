# 🧮 ccwc – Word Count Tool in C#

A simple, cross-platform recreation of the Unix `wc` (word count) utility built in C# using .NET.

This command-line tool lets you count the number of **lines**, **words**, **characters**, and **bytes** in a file – with similar functionality and flags to the original `wc`.

---

## 🚀 Features

    - Count **bytes**, **characters**, **words**, and **lines**
    - Supports common flags: `-c`, `-m`, `-w`, `-l`
- Written in C# (.NET 8)
- Installable as a **.NET global tool**
    - Custom `--help` output

        ---

## 🛠 Installation (via `dotnet tool`)

### 1. Package the project

    Run the following command from the project root:

    ```bash
    dotnet pack -c Release

### 2. Make available globally in the terminal 

    dotnet tool install --global --add-source bin\Release ccwc

## Useage

    ccwc [option] <file>

    ccwc file.txt                 # Outputs bytes, lines, words
    ccwc -c file.txt              # Outputs only byte count
    ccwc -m file.txt              # Outputs only character count
    ccwc -l file.txt              # Outputs only line count
    ccwc -w file.txt              # Outputs only word count
    ccwc --help                   # Displays usage info

