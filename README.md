# FuzzySemQL: Fuzzy and Semantic Search Library for SQL Server

**FuzzySemQL** is a .NET-based SQL Server extension providing advanced fuzzy and semantic comparison functions. It enables efficient similarity search between text fields directly in SQL queries, leveraging both classic fuzzy-match algorithms and state-of-the-art semantic embeddings.

This project empowers SQL Server users with:
- **Fuzzy matching** (using Levenshtein distance and pattern-based LIKE matches),
- **Semantic similarity** (via word/sentence embeddings from [FastText](https://fasttext.cc)),
- **Combined matching**, facilitating robust record linkage, deduplication, data cleaning or semantic search scenarios.

---

## Features

- SQL CLR functions for direct use inside Microsoft SQL Server (from 2008 onward)
- High performance using efficient native C# implementations
- Extensible design for bilingual and multilingual support
- Embedding models easily customizable and shrinkable to your needs
- Secure, deterministic, and RAM-aware—designed for production

---

## Quick Start

**1. Requirements**  
- .NET Framework 3.5 or higher
- Microsoft SQL Server 2008 or later (2012, 2014, 2016, 2017, 2019, 2022...)
- Assembly signing (for SAFE or EXTERNAL_ACCESS deployment on SQL Server)
- Admin rights to deploy SQL CLR assemblies

**2. Building**  
- Build the project in Visual Studio (target: `AnyCPU`, .NET 3.5, signed assembly recommended)

**3. Signing the Assembly**  
Due to SQL Server's CLR security restrictions, signing your assembly is required:

- In Visual Studio: project properties → Signing → Check “Sign the assembly” → Choose or create a strong name key.
- Or via command-line: 

sn -k FuzzySemQL.snk
csc /keyfile:FuzzySemQL.snk ...

**4. Installing on SQL Server**

It can vary between versions

- Check compatibility

SELECT compatibility_level
FROM sys.databases
WHERE name = DB_NAME();

- At least 100

ALTER DATABASE {DB}
SET COMPATIBILITY_LEVEL = 100;

- Show options

sp_configure 'show advanced options', 1;
RECONFIGURE WITH OVERRIDE;

- Enable CLR

sp_configure 'clr enabled', 1;
RECONFIGURE WITH OVERRIDE;

- Check change

sp_configure 'clr enabled';

- Create login and key on master

USE master;
GO

CREATE ASYMMETRIC KEY FuzzySemQL
     FROM EXECUTABLE FILE = '{path}\FuzzySemQL\bin\Release\FuzzySemQL.dll';

CREATE LOGIN FuzzySemQL
    FROM ASYMMETRIC KEY FuzzySemQL;

GRANT UNSAFE ASSEMBLY TO FuzzySemQL;
GO

- Create Assembly + functions

USE {DB};
GO

CREATE ASSEMBLY FuzzySemQL
        FROM '{path}\FuzzySemQL\bin\Release\FuzzySemQL.dll'
        WITH PERMISSION_SET = UNSAFE;

CREATE FUNCTION dbo.FuzzySemantic
(
    @terma NVARCHAR(MAX),
    @termb NVARCHAR(MAX),
    @emba VARBINARY(MAX),
    @embb VARBINARY(MAX)
)
RETURNS FLOAT
AS EXTERNAL NAME FuzzySemQL.FuzzySemQL.FuzzySemantic
GO

CREATE FUNCTION dbo.GetEmbedding
(
    @texto NVARCHAR(MAX)
)
RETURNS VARBINARY(MAX)
AS EXTERNAL NAME FuzzySemQL.FuzzySemQL.GetEmbedding
GO

- Example usage

Create embedding column on any table and triggers for inserts and updates in order to fill it

UPDATE {table}
SET embedding = dbo.GetEmbedding({column})
WHERE embedding IS NULL

SELECT  dbo.FuzzySemantic('The boy is playing in the park', 'A boy played in the playground',null,null)

**5. Embedding Models: Customization and Credit**

This project leverages FastText pretrained or custom embedding models for semantic similarity. Full credit and thanks go to the FastText team for their open, high-quality models and software.

The model file is embedded as a project resource (default: cc.en.300.short.vec).
To optimize RAM usage, we provide vec_model_filter.py a python script to reduce full FastText models to only the most frequent words or words / phrases on vocabulary.txt, feel free to change any parameter on it or directly use your own model.

FastText Models are available at https://fasttext.cc/docs/en/crawl-vectors.html

**6. Model Embedding and Multilingual Roadmap**

The default model is referenced as an embedded resource by name.
Future versions will support:
Switching models/languages at runtime via function parameters
Integration of multiple models in a single assembly
Maintaining and recommending strict RAM usage limits for SQL Server safety

**6. Security and Resource Usage**

Only the most frequent or relevant vocabulary is loaded, keeping RAM usage in check (recommended: models ≤50MB for typical SQL Server instances).
Always measure RAM usage in your staging/production environment before integrating larger or multilingual models.
Assembly signing is required to comply with SQL Server security in production environments.

* The default .pfx password is FuzzySemQL

**7. Credits & License**

Word embeddings provided by FastText, developed by Facebook AI Research (FAIR).
You are free (and encouraged) to use, extend, or adapt FastText models according to their license.

@inproceedings{grave2018learning,
  title={Learning Word Vectors for 157 Languages},
  author={Grave, Edouard and Bojanowski, Piotr and Gupta, Prakhar and Joulin, Armand and Mikolov, Tomas},
  booktitle={Proceedings of the International Conference on Language Resources and Evaluation (LREC 2018)},
  year={2018}
}

This project is released under the MIT License.
See LICENSE for details.

**8. Contributing**s

Contributions, issue reports and feature requests are welcome!
See CONTRIBUTING.md for guidelines.

**9. References**

FastText: Library for efficient text representation and classification
Microsoft Docs: SQL CLR Integration

**10. Sponsor**

Tips appreciated! 

https://github.com/sponsors/OKESC
https://buymeacoffee.com/OKESC
