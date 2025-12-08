@echo off
echo 🌐 Criando complyapi-network se não existir...
docker network inspect complyapi-network >nul 2>&1
if errorlevel 1 (
    docker network create complyapi-network
)

echo 🚀 Iniciando RabbitMQ...
call RabbitMq\infra\dev\docker-start.bat

echo 🔄 Aguardando RabbitMQ iniciar...
timeout /t 5 /nobreak >nul

echo 🚀 Iniciando ApiGateway...
call ApiGateway\infra\dev\docker-start.bat

echo 🚀 Iniciando microsserviços em paralelo...

start "ListingService" cmd /c "ListingService\infra\dev\docker-start.bat"
start "NotificationService" cmd /c "NotificationService\infra\dev\docker-start.bat"
start "PaymentService" cmd /c "PaymentService\infra\dev\docker-start.bat"
start "ProductService" cmd /c "ProductService\infra\dev\docker-start.bat"
start "SaleService" cmd /c "SaleService\infra\dev\docker-start.bat"
start "UserService" cmd /c "UserService\infra\dev\docker-start.bat"
start "ApiGateway" cmd /c "ApiGateway\infra\dev\docker-start.bat"

echo ✅ Todos os microsserviços estão sendo iniciados em paralelo!
echo 💡 Use "docker ps" para ver o status.