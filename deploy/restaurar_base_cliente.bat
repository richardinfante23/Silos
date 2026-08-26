@echo off
chcp 65001 >nul
setlocal EnableExtensions

REM ============================================================
REM  Restaura la base lista para cliente (maestros, sin movimientos)
REM  Ejecutar en el PC donde está PostgreSQL (como Administrador)
REM ============================================================

set "PG_BIN=C:\Program Files\PostgreSQL\18\bin"
if not exist "%PG_BIN%\psql.exe" set "PG_BIN=C:\Program Files\PostgreSQL\17\bin"
if not exist "%PG_BIN%\psql.exe" set "PG_BIN=C:\Program Files\PostgreSQL\16\bin"
if not exist "%PG_BIN%\psql.exe" set "PG_BIN=C:\Program Files\PostgreSQL\15\bin"
if not exist "%PG_BIN%\psql.exe" set "PG_BIN=C:\Program Files\PostgreSQL\14\bin"

if not exist "%PG_BIN%\psql.exe" (
  echo No se encontro psql. Instale PostgreSQL o edite PG_BIN en este .bat
  pause
  exit /b 1
)

set "BACKUP_DIR=%~dp0backup"
set "DB_NAME=produccion_cliente_"
set "PGHOST=localhost"
set "PGPORT=5432"
set "PGUSER=postgres"

REM Buscar el backup base mas reciente
for /f "delims=" %%F in ('dir /b /o-d "%BACKUP_DIR%\sisges_cliente_base_*.backup" 2^>nul') do (
  set "BACKUP_FILE=%BACKUP_DIR%\%%F"
  goto :found
)
echo No hay archivo sisges_cliente_base_*.backup en:
echo   %BACKUP_DIR%
pause
exit /b 1

:found
echo.
echo PostgreSQL : %PG_BIN%
echo Host/Puerto: %PGHOST%:%PGPORT%
echo Base       : %DB_NAME%
echo Backup     : %BACKUP_FILE%
echo.
set /p PGPASSWORD=Contraseña del usuario postgres: 
echo.

echo [1/3] Creando base de datos si no existe...
"%PG_BIN%\psql.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d postgres -c "SELECT 1 FROM pg_database WHERE datname='%DB_NAME%'" | findstr /C:"1" >nul
if errorlevel 1 (
  "%PG_BIN%\psql.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d postgres -c "CREATE DATABASE \"%DB_NAME%\" WITH ENCODING 'UTF8' TEMPLATE template0;"
) else (
  echo La base ya existe. Se restaurara sobre ella (puede pedir confirmacion).
)

echo [2/3] Restaurando backup (estructura + maestros)...
"%PG_BIN%\pg_restore.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %DB_NAME% --clean --if-exists --no-owner --no-privileges "%BACKUP_FILE%"
if errorlevel 1 (
  echo.
  echo ADVERTENCIA: pg_restore reporto mensajes. Revise si la app inicia.
  echo En muchas instalaciones aparecen avisos de "already exists" o ownership.
)

echo [3/3] Verificacion rapida...
"%PG_BIN%\psql.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %DB_NAME% -c "SELECT login, nombre FROM usuarios ORDER BY id;"
"%PG_BIN%\psql.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %DB_NAME% -c "SELECT id, nombre FROM productos;"
"%PG_BIN%\psql.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %DB_NAME% -c "SELECT id, nombre FROM asociaciones;"
"%PG_BIN%\psql.exe" -h %PGHOST% -p %PGPORT% -U %PGUSER% -d %DB_NAME% -c "SELECT id, nombre FROM transportes;"

echo.
echo Listo. Siguiente paso: abrir "Sisges Configuracion.exe" y probar conexion.
echo.
pause
endlocal
