# Registro de Testes de Integrações

Este documento detalha o ambiente de testes de integração, as ferramentas utilizadas e os cenários de teste executados para o **`ProductsService.API`**.

---

## 🧪 Ambiente e Configuração de Teste

Os testes de integração simulam o ambiente de produção/desenvolvimento mais fielmente possível, utilizando a própria `CustomWebApplicationFactory` do ASP.NET Core para hospedar o servidor da API.

### ⚙️ Ferramentas Utilizadas

| Ferramenta | Descrição |
| :--- | :--- |
| **xUnit** | Framework de testes unitários e de integração principal. |
| **FluentAssertions** | Biblioteca para tornar as asserções (verificações) mais legíveis e expressivas. |
| **Testcontainers** | Utilizado para provisionar containers **Docker** gerenciados (**MongoDB** e **RabbitMQ**) durante os testes, garantindo um ambiente limpo e isolado. |
| **MongoDB** | Banco de dados NoSQL utilizado para persistência dos dados de produtos. Um container dedicado é iniciado para os testes. |
| **RabbitMQ** | Message Broker utilizado para comunicação assíncrona. Um container dedicado é iniciado. |
| **AWS S3** | **Serviço real de armazenamento de objetos** (cloud) utilizado para salvar as imagens dos produtos. |
| **Modelo Gemini (ou similar)** | API de IA (mencionada na configuração como `Gemini__Key`) utilizada para **validação de conteúdo impróprio** (`Title` e `Description`) antes de permitir a persistência ou atualização de um produto. |
| **JWT (FakeJwtGenerator)** | Criação de tokens JWT falsos para simular a autenticação de um usuário (`Admin`) em cada requisição. |

### 🛠️ Configuração Inicial (`ProductsControllerTests` Constructor)

1.  Containers de **MongoDB** e **RabbitMQ** são iniciados.
2.  Variáveis de ambiente (Connection Strings, Credenciais AWS, Chave Gemini) são configuradas dinamicamente usando portas mapeadas do Docker.
3.  Um token **JWT** é gerado para um usuário *Admin* (`_userId`).
4.  O `HttpClient` é configurado com o token JWT (Header `Authorization: Bearer`).
5.  A coleção **`Products`** no MongoDB é **limpa** antes de cada execução de classe.

---

## 📝 Casos de Teste de Integração

Foram desenvolvidos 4 cenários de teste principais para cobrir a criação (`POST`) e a atualização (`PATCH`) de produtos, incluindo validações de regras de negócio com o serviço de IA.

### 1. Criar Produto Válido e Persistir (Happy Path)

* **Método:** `POST /api/v1/products`
* **Nome do Teste:** `Post_CreateProduct_ShouldReturnCreated_AndPersistInMongo`
* **Ação:** Envia uma requisição `multipart/form-data` com todos os campos de produto preenchidos (ex: Placa de Vídeo GTX Titan) e conteúdo **apropriado**.
* **Verificações (ASSERT):**
    * O *Status Code* da resposta deve ser **`201 Created`**.
    * O produto deve ser **persistido** no MongoDB.
    * As URLs das imagens no objeto persistido devem conter o domínio do **AWS S3** (`.s3.amazonaws.com/`).

### 2. Tentar Criar Produto com Conteúdo Impróprio (Validação de IA)

* **Método:** `POST /api/v1/products`
* **Nome do Teste:** `Post_CreateProductWithInappropriateContent_ShouldReturnBadRequest_AndNotPersistInMongo`
* **Ação:** Envia uma requisição com Título e Descrição contendo material **proibido** (ex: "AK-47 Original"). O serviço de IA (`Gemini`) deve rejeitar a requisição.
* **Verificações (ASSERT):**
    * O *Status Code* da resposta deve ser **`500 InternalServerError`**.
    * O produto **NÃO** deve ser persistido no MongoDB.

### 3. Atualizar Produto Existente com Sucesso (PATCH)

* **Método:** `PATCH /api/v1/products/{productId}`
* **Nome do Teste:** `Patch_UpdateProduct_ShouldReturnOk_AndPersistUpdateInMongo`
* **Ação:** Um produto existente é atualizado com novos dados e conteúdo **apropriado**.
* **Verificações (ASSERT):**
    * O *Status Code* da resposta deve ser **`200 OK`**.
    * O produto deve ser **atualizado** no MongoDB.

### 4. Tentar Atualizar Produto com Conteúdo Proibido (Validação de IA no PATCH)

* **Método:** `PATCH /api/v1/products/{productId}`
* **Nome do Teste:** `Patch_UpdateProduct_ShouldReturn500_WhenForbiddenWeapon`
* **Ação:** Um produto é atualizado com um Título e Descrição que configuram material **proibido** (ex: "AK47 Real - Alta Letalidade").
* **Verificações (ASSERT):**
    * O *Status Code* da resposta deve ser **`500 InternalServerError`** (rejeição pela validação de IA).
    * O produto no MongoDB **NÃO** deve ser alterado (os dados devem permanecer os originais).

---

## 📸 Resultados dos Testes

Todos os cenários de integração, incluindo a persistência de dados no MongoDB, a interação com o AWS S3 para upload de imagens e as validações de regras de negócio (conteúdo impróprio) via serviço de IA, foram concluídos com sucesso.

Imagem com o resultado de todos os testes de integração:

- **Status**: Todos os testes passaram.

- **Duração total**: 32,6 segundos.

<img width="1857" height="295" alt="Captura de tela 2025-11-16 234413" src="https://github.com/user-attachments/assets/09eb2f54-171f-44bf-9eb7-68d7fd4a8275" />

<img width="583" height="125" alt="Captura de tela 2025-11-16 232820" src="https://github.com/user-attachments/assets/dcf9bc9f-2eb9-4597-9a34-564bf838e481" />

---

### Resultados individuais:

#### 1. Criar Produto Válido e Persistir

- **Status**: Teste passou.

- **Duração total**: 16,2 segundos.

- **Duração do teste**: 4,9 segundos.

<img width="1510" height="187" alt="Captura de tela 2025-11-16 233101" src="https://github.com/user-attachments/assets/b8d87aab-df1c-4fc7-8ceb-a5d014448a1f" />

<img width="588" height="113" alt="Captura de tela 2025-11-16 232955" src="https://github.com/user-attachments/assets/815e1904-d8f7-40e4-8b8d-46ff894dc3a0" />

#### 2. Tentar Criar Produto com Conteúdo Impróprio (Validação de IA)

- **Status**: Teste passou.

- **Duração total**: 16,5 segundos.

- **Duração do teste**: 3,1 segundos.

<img width="1852" height="185" alt="Captura de tela 2025-11-16 233348" src="https://github.com/user-attachments/assets/9c1f95fe-1cb3-4520-8252-eda7090a0e7b" />

<img width="606" height="145" alt="Captura de tela 2025-11-16 233433" src="https://github.com/user-attachments/assets/ce87b131-0803-4a88-bebf-504998666b17" />

#### 3. Atualizar Produto Existente com Sucesso (PATCH)

- **Status**: Teste passou.

- **Duração total**: 15,9 segundos.

- **Duração do teste**: 4,7 segundos.

<img width="1739" height="179" alt="Captura de tela 2025-11-16 233900" src="https://github.com/user-attachments/assets/576421f4-d758-4748-bfca-1670957a27e0" />

<img width="584" height="111" alt="Captura de tela 2025-11-16 233816" src="https://github.com/user-attachments/assets/0a98e853-370e-467c-8917-39fae98dae57" />

#### 4. Tentar Atualizar Produto com Conteúdo Proibido (Validação de IA no PATCH)

- **Status**: Teste passou.

- **Duração total**: 15,8 segundos.

- **Duração do teste**: 3,4 segundos.

<img width="1858" height="214" alt="Captura de tela 2025-11-16 234033" src="https://github.com/user-attachments/assets/708121ee-1c6d-455a-8dd5-14a74c21a887" />

<img width="589" height="111" alt="Captura de tela 2025-11-16 234135" src="https://github.com/user-attachments/assets/19353c2b-7a40-4f4d-b5c2-1b16ff5690a6" />
