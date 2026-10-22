# Migração para lucastoti.com

O frontend usa `https://comply.lucastoti.com` e a API usa `https://complyapi.lucastoti.com`.

Os arquivos `.env` reais são ignorados pelo Git: o merge e o `git pull` não atualizam os valores na VM. Preserve as demais variáveis e credenciais existentes.

## 1. Atualizar os arquivos de ambiente na VM

Em `src/web/.env.production`, definir:

```dotenv
VITE_API_URL=https://complyapi.lucastoti.com/api/v1/
```

No arquivo de ambiente usado pelo Compose da API (neste repositório, `src/api/.env.dev`), definir os retornos de produção do Stripe:

```dotenv
StripeConfiguration__RefreshUrl=https://comply.lucastoti.com/
StripeConfiguration__ReturnUrl=https://comply.lucastoti.com/register/success?type=stripeSuccess
```

As URLs localhost do `.env.example` continuam sendo os padrões para desenvolvimento local. Os comentários mostram os valores de produção.

## 2. Publicar o frontend

Depois do merge e do `git pull`, executar na raiz do repositório:

```sh
cd src/web
npm ci
npm run build
docker compose -f infra/prod/docker-compose.yml up -d
```

O Compose monta `src/web/dist` no Nginx. É necessário gerar esse build novamente para incorporar a nova URL da API.

## 3. Aplicar os retornos do Stripe e a regra de CORS

Com os demais serviços da API já em execução, executar a partir da raiz do repositório:

```sh
cd src/api
docker compose --env-file .env.dev -f docker-compose.dev.yml up -d --no-deps --force-recreate payment-service nginx
```

Se a VM usa outro arquivo Compose ou de ambiente, mantenha os arquivos usados no seu deploy. O serviço de pagamentos precisa ser recriado para receber os valores novos; o gateway Nginx precisa recarregar a configuração de CORS atualizada. Não é necessário reconstruir as imagens dos microsserviços por esta alteração.

## 4. Conferir

Abrir `https://comply.lucastoti.com`, verificar as chamadas para `https://complyapi.lucastoti.com/api/v1/` e conferir se o navegador não bloqueia as requisições por CORS. No fluxo de cadastro Stripe, conferir os retornos para o domínio novo.

DNS, HTTPS e encaminhamentos no Nginx Proxy Manager devem corresponder a esses dois hosts.
