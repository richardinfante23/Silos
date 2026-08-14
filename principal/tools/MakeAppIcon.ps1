# Convert PNG to multi-resolution ICO
Add-Type -AssemblyName System.Drawing

$src = "C:\Users\Richard\.cursor\projects\c-Silos\assets\app_icon_source.png"
$dst = "C:\Silos\principal\logo.ico"

$img = [System.Drawing.Image]::FromFile($src)
$sizes = @(16, 24, 32, 48, 64, 128, 256)
$images = New-Object System.Collections.Generic.List[byte[]]

foreach ($s in $sizes) {
    $bmp = New-Object System.Drawing.Bitmap $s, $s
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $g.Clear([System.Drawing.Color]::Transparent)
    $g.DrawImage($img, 0, 0, $s, $s)
    $g.Dispose()

    $pngMs = New-Object System.IO.MemoryStream
    $bmp.Save($pngMs, [System.Drawing.Imaging.ImageFormat]::Png)
    $images.Add($pngMs.ToArray())
    $pngMs.Dispose()
    $bmp.Dispose()
}

$ms = New-Object System.IO.MemoryStream
$bw = New-Object System.IO.BinaryWriter($ms)
$bw.Write([Int16]0)
$bw.Write([Int16]1)
$bw.Write([Int16]$sizes.Count)

$offset = 6 + (16 * $sizes.Count)
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $s = $sizes[$i]
    $b = $images[$i]
    $w = 0
    $h = 0
    if ($s -lt 256) { $w = $s; $h = $s }
    $bw.Write([byte]$w)
    $bw.Write([byte]$h)
    $bw.Write([byte]0)
    $bw.Write([byte]0)
    $bw.Write([Int16]1)
    $bw.Write([Int16]32)
    $bw.Write([Int32]$b.Length)
    $bw.Write([Int32]$offset)
    $offset += $b.Length
}

foreach ($b in $images) {
    $bw.Write($b)
}

$bw.Flush()
[System.IO.File]::WriteAllBytes($dst, $ms.ToArray())
$bw.Dispose()
$ms.Dispose()
$img.Dispose()

$bin = "C:\Silos\principal\bin\Debug\logo.ico"
if (Test-Path (Split-Path $bin)) {
    Copy-Item $dst $bin -Force
}

Write-Output ("ICO written: {0} bytes" -f (Get-Item $dst).Length)
