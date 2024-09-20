---
uid: api-reference
title: API Reference
---

# API Reference

CodaParser provides a clean and intuitive API for parsing CODA files into structured data. Below is an overview of the key components and interfaces.

## `IParser<T>` Interface

The `IParser<T>` interface defines the contract for parsing CODA files into a specific type.

```csharp
namespace CodaParser;

/// <summary>
/// Interface for parsing a list of strings or a file to a specific type.
/// </summary>
/// <typeparam name="T">The result type of the parsing.</typeparam>
public interface IParser<out T>
{
    /// <summary>
    /// Parses a collection of CODA lines into their matching type.
    /// </summary>
    /// <param name="codaLines">The strings to parse.</param>
    /// <returns>The parsed result.</returns>
    IEnumerable<T> Parse(IEnumerable<string> codaLines);

    /// <summary>
    /// Reads the content of the specified file and parses it into the matching type.
    /// </summary>
    /// <param name="codaFile">Location of the file to parse.</param>
    /// <returns>The parsed result.</returns>
    IEnumerable<T> ParseFile(string codaFile);
}
```

### Implementing `IParser<T>`

To create a custom parser, implement the `IParser<T>` interface:

```csharp
using CodaParser;
using System.Collections.Generic;

public class CustomParser : IParser<Statement>
{
    public IEnumerable<Statement> Parse(IEnumerable<string> codaLines)
    {
        // Implement your parsing logic here
    }

    public IEnumerable<Statement> ParseFile(string codaFile)
    {
        var lines = File.ReadAllLines(codaFile);
        return Parse(lines);
    }
}
```

## `Parser` Class

The `Parser` class is a concrete implementation of the `IParser<Statement>` interface, providing methods to parse CODA files into `Statement` objects.

### Methods

- **`IEnumerable<Statement> Parse(IEnumerable<string> codaLines)`**

  Parses a collection of CODA lines and returns the corresponding `Statement` objects.

- **`IEnumerable<Statement> ParseFile(string codaFile)`**

  Reads the content of the specified CODA file and parses it into `Statement` objects.

## `Statement` Class

Represents a single banking statement extracted from a CODA file.

### Properties

- **`Date`** (`DateTime`): The date of the statement.

- **`Account`** (`Account`): The account information associated with the statement.

- **`InitialBalance`** (`decimal`): The initial balance of the account at the start of the statement period.

- **`NewBalance`** (`decimal`): The balance of the account after all transactions have been processed.

- **`InformationalMessage`** (`string`): Any informational messages included in the statement.

- **`Transactions`** (`IEnumerable<Transaction>`): A collection of transactions executed during the statement period.

### Example

```csharp
public class Statement
{
    public DateTime Date { get; set; }
    public Account Account { get; set; }
    public decimal InitialBalance { get; set; }
    public decimal NewBalance { get; set; }
    public string InformationalMessage { get; set; }
    public IEnumerable<Transaction> Transactions { get; set; }
}
```

## `Account` Class

Represents account details within a statement.

### Properties

- **`Name`** (`string`): The name of the account holder.
- **`Number`** (`string`): The account number.
- **`Type`** (`string`): The type of account (e.g., Checking, Savings).

## `Transaction` Class

Represents a single transaction within a statement.

### Properties

- **`Date`** (`DateTime`): The date of the transaction.
- **`Description`** (`string`): A description of the transaction.
- **`Amount`** (`decimal`): The amount involved in the transaction.
- **`Balance`** (`decimal`): The account balance after the transaction.

---

*For more detailed information and advanced usage, refer to the [official documentation](https://phmatray.github.io/CodaParser/).*
