# Replace $this.Icon in all WinForms .resx with the new logo.ico
Add-Type -AssemblyName System.Drawing

$icoPath = "C:\Silos\principal\logo.ico"
$bytes = [System.IO.File]::ReadAllBytes($icoPath)
$b64 = [Convert]::ToBase64String($bytes)

# Format base64 like typical ResX (line wraps ~80 chars)
$sb = New-Object System.Text.StringBuilder
$sb.AppendLine() | Out-Null
for ($i = 0; $i -lt $b64.Length; $i += 80) {
    $len = [Math]::Min(80, $b64.Length - $i)
    $sb.AppendLine(("    " + $b64.Substring($i, $len))) | Out-Null
}
$sb.Append("  ") | Out-Null
$formatted = $sb.ToString()

$resxFiles = Get-ChildItem "C:\Silos\principal\*.resx"
$count = 0

foreach ($file in $resxFiles) {
    $text = [System.IO.File]::ReadAllText($file.FullName)
    if ($text -notmatch 'name="\$this\.Icon"') { continue }

    $pattern = '(?s)(<data name="\$this\.Icon"[^>]*>\s*<value>)(.*?)(</value>)'
    $replacement = "`$1$formatted`$3"
    $newText = [regex]::Replace($text, $pattern, $replacement, 1)

    if ($newText -ne $text) {
        [System.IO.File]::WriteAllText($file.FullName, $newText)
        $count++
        Write-Output "Updated: $($file.Name)"
    }
}

Write-Output "Done. Updated $count resx files."
