param(
  [string]$Owner = "ellioenay21",
  [string]$Repo = "PetShop-IA",
  [string]$ProjectPath = "C:\Users\ellion\Documents\Petshop",
  [switch]$Private
)

function Read-Pat {
  $sec = Read-Host "Cole seu GitHub PAT (escopo repo) e pressione Enter" -AsSecureString
  $ptr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($sec)
  $pat = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($ptr)
  [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($ptr) | Out-Null
  return $pat
}

$pat = Read-Pat
if (-not $pat) { Write-Error "PAT não fornecido. Abortando."; exit 1 }

$headers = @{ Authorization = "token $pat"; "User-Agent" = "PowerShell" }

# 1) Criar repo (se não existir)
$createBody = @{ name = $Repo }
if ($Private) { $createBody.Add("private", $true) }
$createJson = $createBody | ConvertTo-Json
try {
  Invoke-RestMethod -Uri "https://api.github.com/user/repos" -Method Post -Headers $headers -Body $createJson -ContentType "application/json"
  Write-Host "Repositório criado: $Owner/$Repo"
} catch {
  if ($_.Exception.Response -and $_.Exception.Response.StatusCode.Value__ -eq 422) {
    Write-Host "Repositório já existe. Prosseguindo com upload dos arquivos..."
  } else {
    Write-Error "Falha ao criar repositório: $($_.Exception.Message)"
    exit 1
  }
}

# 2) Upload de arquivos (usa API Create/Update file contents)
# Ignorar padrões comuns (bin/obj/db/dist/node_modules)
$ignore = @('bin','obj','.vs','.vscode','node_modules','dist')
$files = Get-ChildItem -Path $ProjectPath -Recurse -File | Where-Object {
  foreach ($i in $ignore) { if ($_.FullName -like "*\\$i\\*") { return $false } }
  if ($_.Length -gt 50MB) { Write-Host "Ignorando grande: $($_.FullName)"; return $false }
  return $true
}

foreach ($f in $files) {
  $relPath = $f.FullName.Substring($ProjectPath.Length).TrimStart('\\') -replace '\\','/'
  $contentBytes = [System.IO.File]::ReadAllBytes($f.FullName)
  $b64 = [Convert]::ToBase64String($contentBytes)
  $putBody = @{
    message = "Add $relPath"
    content = $b64
    branch = "main"
  } | ConvertTo-Json

  $uri = "https://api.github.com/repos/$Owner/$Repo/contents/$relPath"
  try {
    Invoke-RestMethod -Uri $uri -Method Put -Headers $headers -Body $putBody -ContentType "application/json"
    Write-Host "Enviado: $relPath"
  } catch {
    try {
      $existing = Invoke-RestMethod -Uri $uri -Method Get -Headers $headers -ErrorAction Stop
      $putBody2 = @{
        message = "Update $relPath"
        content = $b64
        sha = $existing.sha
        branch = "main"
      } | ConvertTo-Json
      Invoke-RestMethod -Uri $uri -Method Put -Headers $headers -Body $putBody2 -ContentType "application/json"
      Write-Host "Atualizado: $relPath"
    } catch {
      Write-Warning "Falha ao enviar $relPath : $($_.Exception.Message)"
    }
  }
}

Write-Host "Upload completo. Verifique: https://github.com/$Owner/$Repo"
