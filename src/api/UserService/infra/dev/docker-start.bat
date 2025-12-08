@echo off
echo 🚀 Iniciando UserService...

pushd %~dp0

docker compose down
if errorlevel 1 (
    echo ❌ Erro ao derrubar o container.
    pause
    exit /b
)

docker compose up -d --build
if errorlevel 1 (
    echo ❌ Erro ao iniciar o container.
    pause
    exit /b
)

echo ✅ UserService iniciado!
popd