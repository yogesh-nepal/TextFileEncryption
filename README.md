# Text File Encryption in C#

This repository contains a C# project that generates large text files and encrypts them using multi-threading for efficient processing. The encryption uses the AES algorithm and leverages the number of available CPU cores to speed up both the file creation and encryption processes.

## Features

- **File Generation**: Generates large text files with random content, using multiple threads to improve performance.
- **File Encryption**: Encrypts large text files using the AES encryption algorithm, dividing the files into chunks and processing them in parallel.

## Prerequisites

- .NET 6.0 SDK or higher
- Visual Studio or any C# compatible IDE
