# CodaParser

![NuGet](https://img.shields.io/nuget/v/CodaParser)
![License](https://img.shields.io/github/license/phmatray/CodaParser)
![.NET](https://img.shields.io/badge/.NET-Standard%202.0-blue.svg)
![Conventional Commits](https://img.shields.io/badge/commit-message-conventional%20commits-green.svg)

CodaParser is a modern C# .NET library for parsing Belgian CODA banking files. It is a fork of [supervos/coda-parser](https://github.com/supervos/coda-parser) by Christophe Devos, bringing enhanced functionality and updated dependencies to the .NET ecosystem.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [API Reference](#api-reference)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgements](#acknowledgements)

## Features

- **.NET Standard 2.0**: Compatible with both .NET Framework and .NET Core.
- **Modern C# Features**: Utilizes the latest C# language enhancements for improved performance and readability.
- **Updated Dependencies**: Leveraging the latest NuGet packages for better stability and security.
- **Easy Integration**: Simple API for parsing CODA files.
- **Extensible**: Easily extendable to support additional features or custom processing.
- **Reliable**: Based on the well-established [supervos/coda-parser](https://github.com/supervos/coda-parser) project, ensuring accuracy and reliability.

## Installation

CodaParser will be available as a NuGet package soon. In the meantime, you can download the source code and build it yourself.

### NuGet Package (Coming Soon)

```bash
Install-Package CodaParser
```

### Building from Source

If you prefer to build the library yourself, follow these steps:

1. **Clone the Repository**

   ```bash
   git clone https://github.com/yourusername/CodaParser.git
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

   Reference the built DLLs in your project.

## Usage

Using CodaParser is straightforward. Below is a simple example demonstrating how to parse a CODA file and iterate through the statements and transactions.

```csharp
using System;
using CodaParser;

class Program
{
    static void Main(string[] args)
    {
        var parser = new Parser();
        var statements = parser.ParseFile("path/to/coda-file.cod");
        
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
    }
}
```

## API Reference

### `Parser` Class

Provides methods to parse CODA files into structured statements.

- **Methods**
    - `IEnumerable<Statement> Parse(IEnumerable<string> codaLines)`: Parses a collection of CODA lines.
    - `IEnumerable<Statement> ParseFile(string codaFile)`: Parses a CODA file from the specified path.
    - `IEnumerable<IEnumerable<ILine>> GroupTransactionsPerStatement(IEnumerable<ILine> lines)`: Groups lines belonging to the same statement.

### `Statement` Class

Represents a single banking statement extracted from a CODA file.

- **Properties**
    - `Date`: The date of the statement.
    - `Transactions`: A collection of transactions within the statement.
    - `NewBalance`: The balance after the statement.

*For a complete API reference, please refer to the [documentation](https://github.com/yourusername/CodaParser/wiki).*

## Contributing

Contributions are welcome! We follow the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/) guidelines to ensure a consistent commit history.

1. **Fork the Repository**
2. **Create a Feature Branch**

   ```bash
   git checkout -b feature/YourFeature
   ```

3. **Commit Your Changes**

   Use conventional commit messages. For example:

   ```
   feat(parser): add support for new transaction types
   ```

4. **Push to the Branch**

   ```bash
   git push origin feature/YourFeature
   ```

5. **Open a Pull Request**

Please ensure your code follows the project's coding standards and includes appropriate tests.

## License

This project is licensed under the [GNU GENERAL PUBLIC LICENSE Version 2](LICENSE).

## Acknowledgements

- Forked from [supervos/coda-parser](https://github.com/supervos/coda-parser) by Christophe Devos.
- Inspired by the [wimverstuyf/php-coda-parser](https://github.com/wimverstuyf/php-coda-parser) project.
- Thanks to the open-source community for their invaluable contributions and support.

---

*Happy Parsing!*