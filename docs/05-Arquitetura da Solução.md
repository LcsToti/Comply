# Arquitetura da Solução

Definição de como o software é estruturado em termos dos componentes que fazem parte da solução e do ambiente de hospedagem da aplicação.

<img width="1119" height="871" alt="image" src="https://github.com/user-attachments/assets/2f9da6d8-6b1b-41f9-8192-987559b4a81b" />

## Diagrama de Classes

O diagrama de classes ilustra graficamente como será a estrutura do software, e como cada uma das classes da sua estrutura estarão interligadas. Essas classes servem de modelo para materializar os objetos que executarão na memória.

### Diagrama de Classes 01 - Domínio Payments Service

<img width="933" height="507" alt="image" src="https://github.com/user-attachments/assets/234de7e4-338d-4fc1-acf0-40c245e40f7f" />

### Diagrama de Classes 02 - Domínio Listing Service

<img width="834" height="627" alt="image" src="https://github.com/user-attachments/assets/04e7cdac-163b-4111-b798-92c21fb1ab36" />

### Diagrama de Classes 03 - Domínio Notification Service

<img width="619" height="566" alt="image" src="https://github.com/user-attachments/assets/6f578855-a2b5-4119-9c6e-0d79aca6535b" />

### Diagrama de Classes 04 - Domínio User Service

<img width="748" height="486" alt="image" src="https://github.com/user-attachments/assets/ad9a885f-83bc-48ee-9994-dfd651749c86" />

### Diagrama de classes 05 - Domínio Sales Service

<img width="750" height="482" alt="image" src="https://github.com/user-attachments/assets/6ab05d07-3c26-487c-8975-badcab0e4987" />

### Diagrama de classes 06 - Dominio de Products Service

<img width="778" height="532" alt="image" src="https://github.com/user-attachments/assets/40e1351a-7838-46b4-8781-d27a5a0bcef7" />




## Documentação do Banco de Dados MongoDB

Este documento descreve a estrutura e o esquema do banco de dados não relacional utilizado por nosso projeto, baseado em MongoDB. O MongoDB é um banco de dados NoSQL que armazena dados em documentos JSON (ou BSON, internamente), permitindo uma estrutura flexível e escalável para armazenar e consultar dados.

## Esquema do Banco de Dados
### Coleção: `users`
Armazena os usuários do sistema e suas informações relacionadas, incluindo endereços de entrega, perfil, saldo e watchlist.

```Json
{
    "id": 35, 
    "name": "Ana Varella",
    "email": "ana@email.com",
    "passwordHash": "e679a61815b57d1fd6d043050e3ecac38ff23bcc86bcdcec6bd97ed801afcc7a",
    "phoneNumber": "+5511912345678",
    "profilePic": "https://example.com/profile.jpg",
    "deliveryAddresses": [
      {
        "street": "Rua Dr. Flôres",
        "number": "80",
        "city": "Anta Gorda",
        "state": "Rio Grande do Sul",
        "zipCode": "95980-000"
      }
    ],
    "role": ["User", "Moderator", "Admin"],
    "createdAt": "2025-09-29T14:27:34Z",
    "balance": 150.50,
    "supportTickets": [9],
    "watchList": ["product_id_1", "product_id_2"],
    "customerId": "ABC_123456789",
    "connectedAccountId": "XYZ_987654321"
}
```

#### Descrição dos Campos

> **id** (`string`)
> Identificador único do usuário no sistema.

> **name** (`string`)
> Nome completo do usuário.

> **email** (`string`)
> Endereço de e-mail do usuário, utilizado para login e comunicação.

> **passwordHash** (`string`)
> Hash da senha do usuário para autenticação segura.

> **phoneNumber** (`string`)
> Número de telefone do usuário, no formato internacional, usado para contato.

> **profilePic** (`string`)
> URL da imagem de perfil do usuário.

> **deliveryAddresses** (`array` de objetos)
> Lista de endereços de entrega do usuário:
>
> * **street** (`string`): Nome da rua.
> * **number** (`string`): Número do endereço.
> * **city** (`string`): Cidade.
> * **state** (`string`): Estado.
> * **zipCode** (`string`): Código postal.

> **role** (`enum`: `User` | `Moderator` | `Admin`)
> Papel do usuário dentro do sistema.

> **createdAt** (`DateTime`)
> Data e hora de criação do usuário no sistema.

> **balance** (`decimal`)
> Saldo disponível na conta do usuário, utilizado para compras ou leilões.

> **supportTickets** (`array` de `int`)
> Lista de IDs dos tickets de suporte relacionados ao usuário. Mantém a referência à coleção `supportTickets`.

> **watchList** (`array` de `string`)
> Lista de IDs de produtos que o usuário adicionou à sua lista de observação.

> **customerId** (`string`)
> **connectedAccountId** (`string`)
> Identificadores do usuário no sistema utilizado para transações financeiras.

---

### Coleção: `supportTickets`
Chamados de suporte relacionados ao usuário.

```Json
{
    "id": 9,
    "userId": 35,
    "title": "Problema no pagamento",
    "description": "Tentei realizar um pagamento pelo Mercado Pago, mas a transação não foi concluída e o saldo não voltou para minha conta.",
    "status": "Open",
    "createdAt": "2025-09-29T14:27:34Z",
    "modId": 12,
    "comments": [
    {
        "authorId": 35,
        "comment": "Oi, estou com dificuldade em completar meu pagamento, podem verificar?",
        "createdAt": "2025-09-29T14:27:34Z"
      }
    ]
}
```

#### Descrição dos Campos

> * **id** (`string`): Identificador único do ticket.
> * **title** (`string`): Título ou assunto do ticket.
> * **description** (`string`): Descrição detalhada do problema.
> * **status** (`enum`: `Open` | `InProgress` | `Closed`): Status do ticket.
> * **createdAt** (`DateTime`): Data e hora de abertura do ticket.
> * **modId** (`string`): Identificador do moderador responsável pelo atendimento.
> * **comments** (`array` de objetos): Lista de comentários do ticket:
>
>   * **authorId** (`string`): Identificador do usuário que fez o comentário.
>   * **comment** (`string`): Texto do comentário.
>   * **createdAt** (`DateTime`): Data e hora do comentário.

### Coleção: products
Armazena as informações dos produtos disponíveis no sistema.

```Json
{
    {
  "_id": "<ObjectId>",
  "sellerId": "string",
  "productId": "string",
  "title": "string",
  "description": "string",
  "images": [
    "string"
  ],
  "locale": "string",
  "condition": "string",
  "category": "string",
  "characteristics": "string",
  "priceSettings": {
    "buyNowValue": "decimal",
    "startBidValue": "decimal"
  },
  "auctionSettings": {
    "startDate": "DateTime",
    "endDate": "DateTime",
  },
  "qna": [
    {
      "questionId": "string",
      "userId": "string",
      "text": "string",
      "askedAt": "DateTime",
      "likedCount": "int",
      "answer": {
        "text": "string",
        "answeredAt": "DateTime",
        "likedCount": "int"
      }
    }
  ],
  "status": "int",
  "watchListCount": "int",
  "createdAt": "DateTime",
  "updatedAt": "DateTime",
  "soldAt": "DateTime",
  "suspendedAt": "DateTime",
  "featured": "bool",
  "expirationDate": "DateTime"
  "deliveryPreferences": "int"
}
}
```

#### Descrição dos Campos

> **_id**  
> Identificador único do documento gerado automaticamente pelo MongoDB.

> **sellerId**  
> Identificador do usuário que está vendendo o produto.

> **buyerId**  
> Identificador do usuário que comprou o produto (se aplicável).

> **productId**  
> Identificador único do produto dentro do sistema.

> **title**  
> Título ou nome do produto.

> **description**  
> Descrição detalhada do produto.

> **images**  
> Lista de URLs ou caminhos das imagens do produto.

> **locale**  
> Localização do produto (cidade, estado ou país).

> **condition**  
> Estado do produto (novo, usado, recondicionado etc.).

> **category**  
> Categoria do produto para organização e filtros.

> **characteristics**  
> Características adicionais do produto, como tamanho, cor, peso, material etc.

> **priceSettings**  
> Objeto contendo informações de preço do produto:  
> - **buynow:** Preço para compra imediata.  
> - **startBid:** Valor inicial do leilão, se aplicável.  
> - **soldPrice:** Preço final pelo qual o produto foi vendido.

> **auctionSettings**  
> Objeto contendo configurações do leilão:  
> - **startDate:** Data e hora de início do leilão.  
> - **endDate:** Data e hora de término do leilão.  
> - **incrementValue:** Valor mínimo de incremento em cada lance.

> **qna**  
> Lista de perguntas e respostas relacionadas ao produto:  
> - **questionId:** Identificador único da pergunta.  
> - **userId:** Identificador do usuário que fez a pergunta.  
> - **text:** Texto da pergunta.  
> - **askedAt:** Data e hora em que a pergunta foi feita.  
> - **likedCount:** Quantidade de curtidas da pergunta.  
> - **answer:** Objeto com a resposta da pergunta:  
>   - **text:** Texto da resposta.  
>   - **answeredAt:** Data e hora em que a resposta foi fornecida.  
>   - **likedCount:** Quantidade de curtidas da resposta.

> **status**  
> Status do produto (ativo, vendido, suspenso etc.).

> **watchListCount**  
> Quantidade de usuários que adicionaram o produto à lista de observação.

> **createdAt**  
> Data e hora de criação do produto.

> **updatedAt**  
> Data e hora da última atualização do produto.

> **soldAt**  
> Data e hora em que o produto foi vendido.

> **suspendedAt**  
> Data e hora em que o produto foi suspenso (se aplicável).

> **featured**  
> Booleano indicando se o produto está em destaque.

> **expirationFeatureDate**  
> Data de expiração do período de destaque do produto.

---

### Coleção: `listedProducts`
Armazena os agregados de `ListedProduct`, que representam os itens individuais listados na plataforma.
```json
{
  "_id": "ObjectId('5f7e1bbf9b2a4f1a9c38b9a1')",
  "productId": "f0e9d8c7-b6a5-4321-fedc-ba9876543210",
  "status": "InAuction",
  "buyPrice": 500.00,
  "buyerId": null,
  "auctionId": "b1c2d3e4-f5a6-b789-c123-d456e789f0ab",
  "listedAt": "2025-09-18T21:50:00Z",
  "updatedAt": "2025-09-18T21:55:00Z"
}
```

#### Descrição dos Campos

>   - **\_id:** Identificador único (Guid) do produto listado.
>   - **productId:** Identificador (Guid) do produto original (potencialmente de outro microsserviço/domínio).
>   - **status:** Status atual da listagem (ex: Available, InAuction, Sold).
>   - **buyerId:** Identificador (Guid) do comprador caso a compra direta tenha ocorrido (pode ser nulo).
>   - **buyPrice:** Preço para compra imediata do item (pode ser nulo).
>   - **auctionId:** Identificador (Guid) do leilão associado a esta listagem (pode ser nulo). É a referência para o documento na coleção `auctions`.
>   - **listedAt:** Data e hora em que o produto foi listado pela primeira vez.
>   - **updatedAt:** Data e hora da última atualização dos dados da listagem.

---

### Coleção: `auctions`

Armazena os agregados de `Auction`, incluindo suas configurações e a lista de lances recebidos.

#### Estrutura do Documento

```json
{
  "_id": "ObjectId('6a8f2ccd9b3a4e2b8d27a8b2')",
  "listedProductId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
  "status": "Open",
  "settings": {
    "startBidValue": 150.00,
    "startDate": "2025-09-19T10:00:00Z",
    "endDate": "2025-09-26T10:00:00Z"
  },
  "startedAt": "2025-09-19T10:00:00Z",
  "endedAt": null,
  "bids": [
    {
      "_id": "d1e2f3a4-b5c6-d789-e123-f456a789b0cd",
      "auctionId": "6a8f2ccd9b3a4e2b8d27a8b2",
      "bidderId": "e1f2a3b4-c5d6-e789-f123-a456b789c0de",
      "bidValue": 150.00,
      "status": "Outbid",
      "biddedAt": "2025-09-19T11:05:12Z",
      "outbiddedAt": "2025-09-19T11:12:45Z",
      "wonAt": null
    },
    {
      "_id": "c1d2e3f4-a5b6-c789-d123-e456f789a0bc",
      "auctionId": "6a8f2ccd9b3a4e2b8d27a8b2",
      "bidderId": "f1a2b3c4-d5e6-f789-a123-b456c789d0ef",
      "bidValue": 165.50,
      "status": "Active",
      "biddedAt": "2025-09-19T11:12:45Z",
      "outbiddedAt": null,
      "wonAt": null
    }
  ]
}
```

#### Descrição dos Campos

>    - **_id:** Identificador único do documento do leilão (ObjectId do MongoDB ou Guid).
>    - **listedProductId:** Identificador (Guid) do produto listado ao qual este leilão pertence.
>    - **status:** Status atual do leilão (ex: Awaiting, Open, Completed, Canceled).
>    - **settings**: Objeto embutido com as configurações do leilão, representa o **ValueObject** `AuctionSettings`: 
>        - **startBidValue:** Valor mínimo de lance inicial.
>        - **startDate:** Data e hora que o leilão está agendado para começar.
>        - **endDate:** Data e hora que o leilão irá encerrar.
>    - **startedAt:** Data e hora em que o leilão foi iniciado (pode ser nulo).
>    - **endedAt:** Data e hora em que o leilão foi finalizado (pode ser nulo).
>    - **bids:** Array de objetos embutidos, onde cada objeto representa um lance (entidade Bid) com os campos:
>        - **_id:** Identificador único do lance (Guid).
>        - **auctionId:** Identificador (Guid) do leilão ao qual o lance pertence.
>        - **bidderId:** Identificador (Guid) do usuário que fez o lance.
>        - **bidValue:** Valor do lance.
>        - **status:** Status do lance (Active, Outbid, Won).
>        - **biddedAt:** Data e hora em que o lance foi feito.
>        - **outbiddedAt:** Data e hora em que o lance foi superado (pode ser nulo).
>        - **wonAt:** Data e hora em que o lance venceu (pode ser nulo).

---

### Coleção: payments
Armazena o registro histórico e o estado atual de todas as transações financeiras processadas pelo sistema.

```JSON
{
  "id": "guid",
  "paymentStatus": "string (enum)",
  "withdrawalStatus": "string (enum)",
  "paymentMethod": "string",
  "payerId": "string",
  "sellerId": "string",
  "amount": {
    "total": "decimal",
    "fee": "decimal",
    "net": "decimal",
    "currency": "string"
  },
  "timestamps": {
    "createdAt": "DateTime",
    "processedAt": "DateTime",
    "updatedAt": "DateTime",
    "withdrawnAt": "DateTime"
  },
  "source": {
    "type": "string",
    "id": "guid"
  },
  "gateway": {
    "name": "string",
    "apiPaymentId": "string",
    "apiChargeId": "string"
  },
  "refunds": [
    {
      "id": "guid",
      "apiRefundId": "string",
      "refundAmount": "decimal",
      "reason": "string",
      "refundStatus": "string",
      "createdAt": "DateTime"
    }
  ]
}
```

#### Descrição dos Campos
> **id** 
> Identificador único do documento gerado automaticamente pelo C#.

> **paymentStatus**
> Status do pagamento. Valores possíveis: PENDING, SUCCEEDED, FAILED, CANCELED.

> **withdrawalStatus** 
> Status do saque/retirada do pagamento. Valores possíveis: PENDING, APPROVED, WITHDRAWN.

> **paymentMethod** 
> Identificador do método de pagamento do gateway.

> **payerId** 
> Identificador único do pagador no gateway do sistema.

> **sellerId** 
> Identificador único do vendedor/recebedor no sistema.

> **amount** 
> Objeto que centraliza todos os valores financeiros da transação.
> - **total**: O valor bruto pago pelo comprador.
> - **fee**: A taxa cobrada pelo gateway de pagamento.
> - **net**: O valor líquido recebido após a dedução das taxas (total - fee).
> - **currency**: Moeda da transação (ex.: BRL, USD).

> **timestamps**
> Objeto que agrupa os registros de data e hora do ciclo de vida do pagamento.
> - **createdAt**: Data e hora em que o registro de pagamento foi criado no nosso sistema (status PENDING).
> - **processedAt**: Data e hora em que o pagamento foi finalizado (status SUCCEEDED ou FAILED).
> - **updatedAt**: Data e hora da última modificação no documento.
> - **withdrawnAt**: Data e hora em que o valor foi efetivamente transferido/sacado para o vendedor.

> **source** 
> Objeto que identifica a origem da transação, o que motivou este pagamento. Essencial para rastreabilidade.
> - **type**: O tipo da fonte (ex: LEILÃO, COMPRA IMEDIATA.).
> - **id**: O identificador da fonte (ex: o bidId ou saleId).

> **gateway**
> Objeto contendo as informações do provedor de pagamento que processou a transação. Fundamental para disputas e reconciliação.
> - **name**: Nome do gateway (ex: STRIPE).
> - **apiPaymentId**: Identificador do pagamento no gateway.
> - **apiChargeId**: Identificador da cobrança/charge no gateway.

> **refunds** 
> Lista de estornos associados a este pagamento. Um pagamento pode ter múltiplos estornos parciais.
> - **Id**: Identificador único do estorno (GUID) no nosso sistema.
> - **apiRefundId**: O ID do estorno retornado pelo gateway.
> - **refundAmount**: O valor que foi estornado.
> - **reason**: O motivo do estorno.
> - **refundStatus**: O status do estorno (PROCESSING, SUCCEEDED).
> - **createdAt**: Data e hora em que o estorno foi solicitado.

---

### Coleção: notifications
Armazena informações das notificações do sistema.

```JSON
{
  "_id": "<ObjectId>",
  "userId": "string",
  "type": "string",
  "source": {
    "type": "string",
    "id": "string"
  },
  "message": "string",
  "read": "boolean",
  "timestamps": {
    "createdAt": "DateTime"
  }
}
```

#### Descrição dos Campos

> **_id** 
> Identificador único do documento gerado automaticamente pelo MongoDB.

> **userId** 
> Identificador do usuário que deve receber a notificação.

> **type** 
> O tipo da notificação, que determina o evento que a disparou.
> - **productSold**: Produto vendido.
> - **auctionEndingSoon**: Leilão encerrando em breve.
> - **outbid**: Lance superado.
> - **newBid**: Novo lance no produto.
> - **buyNowSale**: Produto comprado via "Compra Imediata".

> **source** 
> Objeto que identifica a origem do evento de notificação.
> - **type**: O tipo da fonte (bid, sale, product, payment).
> - **id**: O identificador da fonte (bidId, saleId, productId, paymentId).

> **message** 
> A mensagem de texto da notificação, a ser exibida para o usuário.

> **read** 
> Booleano que indica se a notificação foi lida pelo usuário.

> **timestamps** 
> Objeto para registrar o momento de criação da notificação.
> - **createdAt**: Data e hora que a notificação foi criada.

### Boas Práticas

1. **Validação de Dados**
- Prática: Garantir que os dados salvos no banco sejam válidos e consistentes, aplicando regras de negócio na aplicação antes de persistir.
- Ferramenta: `FluentValidation (biblioteca para C#)`.

2. **Monitoramento e Logs**
- Prática: Acompanhar a performance do banco para identificar gargalos e registrar eventos importantes para diagnosticar problemas.
- Monitoramento: Painéis do MongoDB Atlas.
- Logs: `Biblioteca Serilog (em C#)`.

3. **Escalabilidade**
- Prática: Planejar o crescimento do banco de dados para manter a aplicação rápida e sempre disponível.
- Replicação (Alta Disponibilidade): Usar um Replica Set no MongoDB para ter cópias do banco de dados, evitando que o site saia do ar em caso de falha.

4. **Gerenciamento de Índices**
- Prática: Criar índices para acelerar as buscas, evitando que o banco de dados precise ler a coleção inteira para encontrar um dado.
- Ferramentas: MongoDB Compass (para gerenciamento visual) e o comando explain() (para analisar performance).

## Tecnologias Utilizadas

### Linguagens e Frameworks
- **C# / .NET**: desenvolvimento do backend e microsserviços, incluindo APIs e lógica de negócio.
- **React**: desenvolvimento do frontend web, garantindo interfaces dinâmicas e responsivas.
- **React Native**: desenvolvimento do frontend mobile para Android e iOS.
- **NoSQL(MongoDb)**: gerenciamento de banco de dados não relacionais.

### Ferramentas e IDEs
- **Visual Studio / Rider**: IDEs para desenvolvimento backend (C#/.NET).
- **Visual Studio Code**: IDE para desenvolvimento frontend web e mobile (React / React Native).
- **Android Studio**: emulação e testes de aplicações mobile.
- **Figma**: criação de wireframes, protótipos e design de interface.
- **LucidChart**: documentação de diagramas de arquitetura e fluxos do sistema.
- **GitHub**: controle de versão, gerenciamento de issues, milestones e Kanban.

### Bibliotecas e Serviços
- **Axios / Fetch**: para integração com APIs no frontend.
- **Stripe / Payment Gateways**: processamento de pagamentos.
- **Firebase**: notificações e push notifications no mobile.
- **RabbitMq**: para comunicação entre serviços

## Hospedagem

A hospedagem será feita utilizando a Cloud da Oracle. A infraestrutura será composta por um cluster de VMs (Virtual Machines) para garantir alta disponibilidade e escalabilidade.

### Estrutura dos Serviços no Cluster Kubernetes
Cada componente da aplicação será implantado como um recurso do Kubernetes, distribuído entre as VMs do cluster.

- React (Front-end): Será implantado como um Deployment no cluster Kubernetes.

- API Gateway (.NET): Será implantado como um Deployment no cluster Kubernetes.

- Microsserviços (.NET): Cada serviço será implantado como um Deployment individual no cluster Kubernetes.

- RabbitMQ (Mensageria): Será implantado como um StatefulSet no cluster Kubernetes para garantir a persistência das filas e mensagens.

- MongoDB (Banco de Dados): Será implantado como um StatefulSet no cluster Kubernetes para garantir a persistência e a estabilidade dos dados.

## Qualidade de Software

A equipe se compromete com a entrega de um software de alta qualidade, utilizando a norma ISO/IEC 25010 como guia para as características e sub-características a serem priorizadas. A qualidade será construída continuamente ao longo do processo de desenvolvimento, com foco em atender e superar as expectativas dos usuários e stakeholders.

As sub-características escolhidas como norteadoras do projeto, suas justificativas e métricas de avaliação são apresentadas na tabela abaixo.

| Característica Principal | Sub-característica       | Justificativa da Escolha                                                                                                                                           | Métricas de Avaliação                                                                                     |
|--------------------------|-------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------|
| Adequação Funcional      | Completude Funcional    | Garantir que todas as funcionalidades especificadas nos requisitos e user stories sejam implementadas e funcionem corretamente, entregando o valor prometido ao usuário. | • Percentual de requisitos do backlog implementados. <br> • Taxa de sucesso nos casos de teste de aceitação. |
| Eficiência de Desempenho | Comportamento em Relação ao Tempo | A aplicação web e mobile deve ter tempos de resposta rápidos para proporcionar uma experiência de usuário fluida e evitar frustração.                             | • Tempo médio de resposta da API (< 200ms). <br> • Tempo de carregamento das telas principais (< 3s).  |
| Usabilidade              | Reconhecimento da Adequação | Os usuários devem conseguir entender e utilizar a aplicação de forma intuitiva, sem a necessidade de treinamento, reconhecendo facilmente como completar suas tarefas. | • Taxa de sucesso na conclusão de tarefas por novos usuários. <br> • Redução no número de cliques para executar ações-chave. |
| Confiabilidade           | Tolerância a Falhas     | Em uma arquitetura de microsserviços, a falha de um componente não crítico não deve derrubar todo o sistema. A aplicação deve lidar com erros de forma resiliente.  | • Percentual de uptime do sistema (meta: 99.5%). <br> • Ausência de falhas em cascata durante testes.   |
| Segurança                | Confidencialidade       | Proteger os dados sensíveis dos usuários contra acesso não autorizado é uma prioridade máxima, especialmente em funcionalidades de autenticação e pagamento.     | • Zero vulnerabilidades críticas em scans de segurança (OWASP). <br> • Criptografia de dados em trânsito (HTTPS) e em repouso. |
| Manutenibilidade         | Modularidade            | A arquitetura de microsserviços e a componentização do front-end devem ser mantidas para permitir que a equipe desenvolva, teste e implante partes do sistema de forma independente. | • Baixo acoplamento entre os serviços. <br> • Aderência aos princípios SOLID e de design do código, verificado em code reviews. |
| Manutenibilidade         | Testabilidade           | O código deve ser escrito de forma a facilitar a criação de testes automatizados, que são essenciais para garantir a qualidade contínua e a segurança ao realizar alterações. | • Cobertura de código por testes unitários e de integração (meta: > 80%). <br> • Tempo de execução da suíte de testes automatizados. |

