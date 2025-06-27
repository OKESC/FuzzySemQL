# Contributing to FuzzySemQL

Thank you for considering contributing to FuzzySemQL!  
Your help is greatly appreciated.  
This document provides guidelines for contributing, bug reporting, and feature proposals.

---

## Table of Contents

- [How Can I Contribute?](#how-can-i-contribute)
  - [Reporting Bugs](#reporting-bugs)
  - [Requesting Features](#requesting-features)
  - [Submitting Pull Requests](#submitting-pull-requests)
- [Coding Guidelines](#coding-guidelines)
- [Code of Conduct](#code-of-conduct)
- [Attribution](#attribution)

---

## How Can I Contribute?

### Reporting Bugs

If you find a bug, please:

- Search [existing issues](https://github.com/OKESC/FuzzySemQL/issues) first to see if it’s already reported.
- Open a new issue and use a clear, descriptive title.
- Include steps to reproduce, your environment (OS, .NET/SQL Server version), and any relevant logs or error messages.

### Requesting Features

Feature requests and ideas are very welcome!  
Please:

- Check the [current issues](https://github.com/OKESC/FuzzySemQL/issues) to avoid duplicates.
- Open a new issue and use the `Feature request` label.
- Describe your use case, the problem it solves, and (if possible) how you envision it working.

### Submitting Pull Requests

We welcome pull requests (PRs) for bug fixes, code improvements, documentation, or new features.

**To submit a PR:**

1. Fork the repository and create your branch from `main`.
2. Write clear, concise commit messages.
3. Include tests where applicable.
4. Ensure existing tests pass (`dotnet test` or manual steps for .NET/SQL Server).
5. If you add or change public APIs, update the documentation (code comments, `README.md`).
6. Open a PR, describe your changes and reference related issues.

---

## Coding Guidelines

- Follow the [Microsoft .NET coding conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
- Use descriptive variable and function names.
- Document new classes, methods, and public members using [XML comments](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/).
- Format code consistently (spaces, indentation, etc.).
- Keep changes focused — one PR per feature or fix is preferred.

---

## Code of Conduct

This project and everyone participating is expected to uphold the [Contributor Covenant Code of Conduct](https://www.contributor-covenant.org/).  
Please be respectful and constructive in all discussions and reviews.

---

## Attribution

This project integrates and extends [FastText](https://fasttext.cc) models.  
Please give credit where it’s due when reusing or distributing code or models.

---

Thank you for helping make FuzzySemQL better for everyone!