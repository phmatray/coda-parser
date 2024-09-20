---
uid: getting-started
title: Getting Started
---

# Getting Started

This guide will help you set up and start using CodaParser in your .NET projects.

## Prerequisites

- **.NET SDK:** Ensure you have the .NET 8 SDK installed.
- **Git:** Required for cloning the repository and version control.
- **Optional Tools:**
    - **DocFX:** For generating documentation.
    - **Nuke:** For build automation (if using the provided Nuke build script).

## Installation

### NuGet Package

CodaParser is available as a NuGet package. Install it using the following commands:

#### Using .NET CLI

```bash
dotnet add package CodaParser
```

#### Using Package Manager Console

```powershell
Install-Package CodaParser
```

### Building from Source

If you prefer to build the library yourself, follow these steps:

1. **Clone the Repository**

   ```bash
   git clone https://github.com/phmatray/CodaParser.git
   cd CodaParser
   ```

2. **Restore Dependencies**

   Ensure you have the .NET SDK installed. Then run:

   ```bash
   dotnet restore
   ```

3. **Build the Project**

   ```bash
   dotnet build
   ```

4. **Use the Library**

   Reference the built DLLs in your project by adding them to your project's dependencies.

## Basic Usage

Below is a simple example demonstrating how to parse a CODA file and iterate through the statements and transactions using the `IParser<T>` interface.

```csharp
using CodaParser;
using System.Collections.Generic;

// Instantiate the parser (assuming a concrete implementation is available)
IParser<Statement> parser = new Parser();

// Parse the CODA file
IEnumerable<Statement> statements = parser.ParseFile("path/to/coda-file.cod");

// Iterate through the statements and transactions
foreach (var statement in statements)
{
    Console.WriteLine($"Date: {statement.Date:yyyy-MM-dd}");
    
    foreach (var transaction in statement.Transactions)
    {
        Console.WriteLine($"{transaction.Account.Name}: {transaction.Amount}");
    }
    
    Console.WriteLine($"New Balance: {statement.NewBalance}");
    Console.WriteLine(new string('-', 40));
}
```

### Minimal API Example (.NET 8)

Leveraging .NET 8's minimal API features, you can create a streamlined application without explicit `Program` and `Main` methods:

```csharp
using CodaParser;
using System.Collections.Generic;

IParser<Statement> parser = new Parser();

IEnumerable<Statement> statements = parser.ParseFile("path/to/coda-file.cod");

foreach (var statement in statements)
{
    Console.WriteLine($"Date: {statement.Date:yyyy-MM-dd}");
    
    foreach (var transaction in statement.Transactions)
    {
        Console.WriteLine($"{transaction.Account.Name}: {transaction.Amount}");
    }
    
    Console.WriteLine($"New Balance: {statement.NewBalance}");
    Console.WriteLine(new string('-', 40));
}
```

## Running the Example

1. **Create a New .NET Project**

   ```bash
   dotnet new console -n CodaParserDemo
   cd CodaParserDemo
   ```

2. **Add CodaParser Package**

   ```bash
   dotnet add package CodaParser
   ```

3. **Replace `Program.cs` with the Example Code**

4. **Run the Application**

   ```bash
   dotnet run
   ```

Ensure that the path to your CODA file is correct. The application will parse the file and display the transactions in the console.
