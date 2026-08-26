@echo off
chcp 65001 >nul
REM Genera de nuevo los backups desde la BD local de desarrollo.
REM Ejecutar en el PC de desarrollo (con PostgreSQL y BD produccion_cliente_).

set "PG_BIN=C:\Program Files\PostgreSQL\18\bin"
set "OUT=%~dp0backup"
set "DB=produccion_cliente_"
set "PGHOST=localhost"
set "PGPORT=5432"
set "PGUSER=postgres"

if not exist "%OUT%" mkdir "%OUT%"
for /f %%I in ('powershell -NoProfile -Command "Get-Date -Format yyyyMMdd"') do set STAMP=%%I

set /p PGPASSWORD=Contraseña postgres: 

echo Generando completo...
"%PG_BIN%\pg_dump.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %DB% -Fc -b -f "%OUT%\sisges_completo_%STAMP%.backup"

echo Generando base cliente (sin movimientos)...
"%PG_BIN%\pg_dump.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %DB% -Fc -b ^
  --exclude-table-data=public.recepciones ^
  --exclude-table-data=public.despachos ^
  --exclude-table-data=public.detalle_analisis_recepcion ^
  --exclude-table-data=public.detalle_analisis_despacho ^
  --exclude-table-data=public.historico_articulos ^
  --exclude-table-data=public.detalle_historico_articulos ^
  --exclude-table-data=public.tasa_diaria ^
  -f "%OUT%\sisges_cliente_base_%STAMP%.backup"

echo OK en %OUT%
dir "%OUT%\*%STAMP%*"
pause
