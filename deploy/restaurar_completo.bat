@echo off
chcp 65001 >nul
setlocal

REM Restaura el backup COMPLETO (incluye recepciones/despachos de prueba)

set "PG_BIN=C:\Program Files\PostgreSQL\18\bin"
if not exist "%PG_BIN%\psql.exe" set "PG_BIN=C:\Program Files\PostgreSQL\17\bin"
if not exist "%PG_BIN%\psql.exe" set "PG_BIN=C:\Program Files\PostgreSQL\16\bin"

set "BACKUP_DIR=%~dp0backup"
set "DB_NAME=produccion_cliente_"
set "PGHOST=localhost"
set "PGPORT=5432"
set "PGUSER=postgres"

for /f "delims=" %%F in ('dir /b /o-d "%BACKUP_DIR%\sisges_completo_*.backup" 2^>nul') do (
  set "BACKUP_FILE=%BACKUP_DIR%\%%F"
  goto :found
)
echo No hay sisges_completo_*.backup
pause
exit /b 1

:found
echo Restaurar COMPLETO: %BACKUP_FILE%
set /p PGPASSWORD=Contraseña postgres: 
"%PG_BIN%\psql.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d postgres -c "CREATE DATABASE \"%DB_NAME%\" WITH ENCODING 'UTF8' TEMPLATE template0;" 2>nul
"%PG_BIN%\pg_restore.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %DB_NAME% --clean --if-exists --no-owner --no-privileges "%BACKUP_FILE%"
echo Fin.
pause
endlocal
