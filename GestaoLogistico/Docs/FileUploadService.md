# Serviço de Upload de Arquivos

## Visão Geral

O `FileUploadService` é responsável por gerenciar o upload, armazenamento e exclusão de arquivos no sistema.

## Funcionalidades

### 1. **SaveFileAsync**
Salva um arquivo a partir de um array de bytes.

```csharp
Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string folder = "uploads")
```

**Parâmetros:**
- `fileBytes`: Array de bytes do arquivo
- `fileName`: Nome do arquivo
- `folder`: Pasta onde o arquivo será salvo (padrão: "uploads")

**Retorna:** Caminho relativo do arquivo salvo (ex: `/uploads/users/guid_filename.jpg`)

### 2. **DeleteFileAsync**
Remove um arquivo do sistema.

```csharp
Task<bool> DeleteFileAsync(string filePath)
```

**Parâmetros:**
- `filePath`: Caminho relativo do arquivo

**Retorna:** `true` se o arquivo foi deletado com sucesso, `false` caso contrário

### 3. **ValidateFileSize**
Valida o tamanho do arquivo.

```csharp
bool ValidateFileSize(byte[] fileBytes, long maxSizeInMB = 5)
```

**Parâmetros:**
- `fileBytes`: Array de bytes do arquivo
- `maxSizeInMB`: Tamanho máximo permitido em MB (padrão: 5MB)

**Retorna:** `true` se o arquivo está dentro do limite, `false` caso contrário

### 4. **ValidateFileType**
Valida a extensão do arquivo.

```csharp
bool ValidateFileType(string fileName, string[] allowedExtensions)
```

**Parâmetros:**
- `fileName`: Nome do arquivo
- `allowedExtensions`: Array de extensões permitidas (ex: `[".jpg", ".png"]`)

**Retorna:** `true` se a extensão é válida, `false` caso contrário

### 5. **GetFileUrl**
Retorna a URL formatada do arquivo.

```csharp
string GetFileUrl(string filePath)
```

**Parâmetros:**
- `filePath`: Caminho do arquivo

**Retorna:** URL formatada do arquivo

## Uso no Controller

### Exemplo de Endpoint com Upload de Arquivo

```csharp
[HttpPut("EditUser")]
public async Task<IActionResult> EditUser([FromForm] UserEditFormDTO formDto)
{
    var dto = new UserEditCreateDTO
    {
        Nome = formDto.Nome,
        Email = formDto.Email,
        // ... outras propriedades
    };

    if (formDto.PhotoFile != null)
    {
        using (var memoryStream = new MemoryStream())
        {
            await formDto.PhotoFile.CopyToAsync(memoryStream);
            dto.UrlPhoto = memoryStream.ToArray();
        }
    }

    var result = await _userService.EditUser(dto);
    return Ok(result);
}
```

## Uso no Service

```csharp
public async Task<UserEditCreateDTO> EditUser(UserEditCreateDTO dto)
{
    // ... código de validação

    if (dto.UrlPhoto != null && dto.UrlPhoto.Length > 0)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var fileName = $"user_{user.Id}.jpg";

        // Validar tamanho (máximo 5MB)
        if (!_fileUploadService.ValidateFileSize(dto.UrlPhoto, 5))
        {
            throw new ArgumentException("O arquivo excede o tamanho máximo de 5MB.");
        }

        // Deletar foto antiga se existir
        if (!string.IsNullOrEmpty(user.UrlFoto))
        {
            await _fileUploadService.DeleteFileAsync(user.UrlFoto);
        }

        // Salvar novo arquivo
        var filePath = await _fileUploadService.SaveFileAsync(dto.UrlPhoto, fileName, "uploads/users");
        user.UrlFoto = filePath;
    }

    // ... restante do código
}
```

## Estrutura de Pastas

```
wwwroot/
└── uploads/
    └── users/
        ├── guid_user_123.jpg
        └── guid_user_456.png
```

## DTOs

### UserEditFormDTO (para receber do frontend)
```csharp
public class UserEditFormDTO
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public IFormFile? PhotoFile { get; set; }  // Arquivo enviado via multipart/form-data
}
```

### UserEditCreateDTO (para processamento interno)
```csharp
public class UserEditCreateDTO
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public byte[]? UrlPhoto { get; set; }  // Array de bytes do arquivo
}
```

### UserDTOcompleto (para retornar ao frontend)
```csharp
public class UserDTOcompleto
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string PhotoUrl { get; set; }  // Caminho da imagem (ex: /uploads/users/guid_user.jpg)
    // ... outras propriedades
}
```

## Modelo de Banco de Dados

```csharp
public class Usuario
{
    public string? UrlFoto { get; set; }  // Armazena o caminho relativo do arquivo
    // ... outras propriedades
}
```

## Exemplo de Requisição (Frontend)

### JavaScript/Fetch
```javascript
const formData = new FormData();
formData.append('Nome', 'João Silva');
formData.append('Email', 'joao@example.com');
formData.append('PhotoFile', fileInput.files[0]);

fetch('/api/Usuario/EditUser', {
    method: 'PUT',
    headers: {
        'Authorization': `Bearer ${token}`
    },
    body: formData
});
```

### cURL
```bash
curl -X PUT "https://localhost:5001/api/Usuario/EditUser" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "Nome=João Silva" \
  -F "Email=joao@example.com" \
  -F "PhotoFile=@/caminho/para/foto.jpg"
```

## Configurações Importantes

1. **Program.cs** - Certifique-se de ter:
```csharp
// Registrar o serviço
builder.Services.AddScoped<IfileUploadService, FileUploadService>();

// Habilitar arquivos estáticos
app.UseStaticFiles();
```

2. **Extensões Permitidas** - Configure conforme necessário:
```csharp
var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
```

3. **Tamanho Máximo** - Ajuste conforme necessário:
```csharp
_fileUploadService.ValidateFileSize(fileBytes, 5); // 5MB
```

## Segurança

- ✅ Validação de tipo de arquivo
- ✅ Validação de tamanho de arquivo
- ✅ Nome de arquivo único (GUID)
- ✅ Armazenamento em pasta protegida
- ✅ Logging de operações
- ✅ Tratamento de exceções

## Acesso aos Arquivos

Os arquivos são acessíveis via:
- **Desenvolvimento**: `https://localhost:5001/uploads/users/guid_user_123.jpg`
- **Produção**: `https://seudominio.com/uploads/users/guid_user_123.jpg`
