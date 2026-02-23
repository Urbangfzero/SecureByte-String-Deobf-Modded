# SecureByte String Deobfuscator

Tool to reverse engineer SecureByte-protected .NET assemblies by automatically detecting and decrypting encrypted strings.


---

## Before & After

### Before Deobfuscation
![Before Image](images/before.png)

### After Deobfuscation
![After Image](images/after.png)

---

## Features

- Automatic detection of SecureByte string decryptor
- XOR-based string decryption support
- Resource-based encrypted string handling
- Clean replacement of encrypted calls with `ldstr`
- Simplifies string expressions after decryption
- User-friendly console output
- Built with dnlib for reliable IL editing

---

## How It Works

1. Loads the protected assembly.
2. Detects SecureByte’s string decryptor method.
3. Extracts encrypted strings from embedded resources.
4. Decrypts strings using the original algorithm.
5. Replaces encrypted calls with clean plaintext `ldstr` instructions.
6. Saves a fully deobfuscated assembly.

If the assembly contains compressed resources, this project uses:

SecureByte Resource Decompressor  
https://github.com/Urbangfzero/SecureByte-Resource-Decompressor  

to properly extract and prepare embedded data before string restoration.

---

## Requirements

- .NET 4.8
- Windows (recommended)

---

## Usage

```bash
SecureByte-String-Deobf.exe <ProtectedAssembly.exe>
```

Example:

```bash
SecureByte-String-Deobf.exe Target.exe
```

The tool will generate a cleaned version of the assembly in the same directory.

---

## Credits

This project would not be possible without:

- **dnlib**  
  [![GitHub](https://img.shields.io/badge/GitHub-dnlib-black?style=for-the-badge&logo=github)](https://github.com/0xd4d/dnlib)

- **Colorful.Console**  
  [![GitHub](https://img.shields.io/badge/GitHub-Colorful.Console-black?style=for-the-badge&logo=github)](https://github.com/tomakita/Colorful.Console)

- **Cheetah0xf** – Original SecureByte string decryptor inspiration  
  [![GitHub](https://img.shields.io/badge/GitHub-Cheetah0xf%20Repo-black?style=for-the-badge&logo=github)](https://github.com/Cheetah0xf/SecureByte-String-Deobf)

- **SecureByte Resource Decompressor (by Urban)**  
  [![GitHub](https://img.shields.io/badge/GitHub-Resource%20Decompressor-black?style=for-the-badge&logo=github)](https://github.com/Urbangfzero/SecureByte-Resource-Decompressor)

---

## Disclaimer

This tool is intended strictly for:

- Malware analysis  
- Reverse engineering research  
- Educational purposes  
- Software protection research  

Do NOT use this tool on software you do not have permission to analyze.

---

If this project helped you, consider giving it a star ⭐