# Documentação do Projeto de Trabalho em Equipe

Este é um exemplo de documento para a documentação das etapas e contribuição de cada membro. É importante salientar que todos os membros devem participar ativamente em todas as etapas e, nas etapas de desenvolvimento é impressindível que todos tenham atividades de desenvolvimento. <strong> Este arquivo deve refletir a execução das tarefas de cada aluno por etapa e estar em consonância com as ferramentas de gestão de tempo utilizadas pelo grupo, contando principalmente o tempo de início e termino de cada tarefa.</strong>

## Visão Geral

Este documento detalha as etapas e responsabilidades do trabalho em equipe para o desenvolvimento de um projeto. O projeto está dividido em cinco etapas principais, cada uma com suas respectivas tarefas e prazos. Cada membro da equipe é responsável por completar as tarefas atribuídas e colaborar com os demais para garantir o sucesso do projeto.

## Etapa 1: Levantamento

### Objetivo

Coletar e documentar todos os requisitos necessários para o desenvolvimento do projeto.

### Tarefas

- **Documentação do Contexto**: Entender e desenvolver melhor o Contexto, Objetivos, Problema, Público-Alvo.
- **Especificação do Problema e Personas**: Definir o problema, perfis de usuários, necessidades e personas.
- **Arquitetura Inicial e Requisitos**: Desenhar arquitetura distribuída inicial e documentar requisitos.

### Responsáveis

| **Nome** | **Contribuições** |
|---------|------------------|
| **Matheus Zeíta** | Papel de PO definido, participou com revisões e comentários gerais sobre andamento do time. |
| **Pedro Bezerra** | Organizou o GitHub (Kanban com issues e milestones), definiu diagrama da arquitetura, RNF, criou diagrama de casos de uso e ajudou nas regras de negócio. |
| **Luan Bezerra** | Documentou o contexto do projeto, criou histórias de usuário e personas, participou do levantamento de RF e definição de regras de negócio. |
| **Lucas Toti** | Levantou requisitos RF/RNF, ajudou na definição das regras de negócio, elaborou personas e histórias de usuário, contribuiu na identidade visual do projeto (logo). |
| **Pedro Machado** | Concluí com sucesso o desenvolvimento do backend para o sistema de notificação e suporte (tickets). O serviço está totalmente operacional, utilizando Clean Architecture e ferramentas robustas como RabbitMQ para mensageria, MongoDB e MediatR, garantindo um sistema eficiente e desacoplado. |


### Prazo

- **Data de conclusão**: 07/09/2025

---

## Etapa 2: Desenvolvimento Backend

### Objetivo

Desenvolver a lógica de negócio e os serviços de backend do projeto.

### Tarefas

- **Desenvolvimento da API**: Implementar as APIs necessárias para o funcionamento do sistema.
- **Gestão de Banco de Dados**: Projetar e implementar o banco de dados.
- **Testes Unitários**: Criar e executar testes unitários para garantir a qualidade do código backend.

### Responsáveis

- **Desenvolvedor Backend**: Matheus Zeíta Silva
- **Desenvolvedor Backend**: Luan Fernando dos Santos Bezerra
- **Desenvolvedor Backend**: Lucas Gabriel Ferreira Toti
- **Desenvolvedor Backend**: Pedro Fernando dos Santos Bezerra
- **Desenvolvedor Backend**: Pedro Henrique Silveira Leiva Machado
- **Engenheiro de Software**: Luan Fernando dos Santos Bezerra
- **Engenheiro de Software**: Pedro Fernando dos Santos Bezerra
- **Engenheiro de Software**: Lucas Gabriel Ferreira Toti
- **Scrum Master:**: Pedro Fernando dos Santos Bezerra

| **Nome** | **Contribuições** |
|---------|------------------|
| **Matheus Zeíta** | Eu refinei os requisitos funcionais, criei o diagrama de classes do User Service e o esquema de banco de dados. Desenvolvi esse projeto do meu serviço baseado em Clean Architecture e Domain-Driven Design, usando .NET. Meu serviço inclui autenticação e cadastro, tickets de ajuda, implementação de cargos como usuário, moderador e administrador, além de gerenciamento de endereços para os usuários. |
| **Pedro Bezerra** | Atuando como Scrum Master, foi responsável por organizar o fluxo de trabalho da equipe através da gestão de issues no quadro Kanban, documentação da metodologia e remoção de impedimentos. No desenvolvimento, implementou de ponta a ponta o microsserviço de pagamentos aplicando os princípios de Clean Architecture e DDD, integrando o gateway Stripe para transações seguras e o RabbitMQ para a liberação assíncrona de valores ao vendedor, garantindo a robustez da solução com a criação de testes unitários focados no domínio. |
| **Luan Bezerra** | Criou um microserviço de produtos que gerencia produtos e imagens, verifica conteúdo com IA, usa MongoDB e S3, processa eventos com RabbitMQ e possui código limpo com testes unitários. |
| **Lucas Toti** | Requisitos funcionais, modelagem de classes (Sales e Auction), criação/correção de collections (Auctions, ListedProducts), definição da lógica de negócios e interação entre microsserviços, integração com RabbitMQ (MassTransit), estruturação do API Gateway, desenvolvimento do Listing Service com Clean Architecture e DDD, testes unitários com xUnit no Domain, colaboração na solution e sugestão de versionamento. |
| **Pedro Machado** | Concluí com sucesso o desenvolvimento do backend para o sistema de notificação e suporte (tickets). O serviço está totalmente operacional, utilizando Clean Architecture e ferramentas robustas como RabbitMQ para mensageria, MongoDB e MediatR, garantindo um sistema eficiente e desacoplado. |


### Prazo

- **Data de conclusão**: 2025-10-05

---

## Etapa 3: Desenvolvimento Web

### Objetivo

Desenvolver a interface web do projeto, garantindo uma experiência de usuário eficaz, intuitiva e responsiva, além de assegurar a integração estável com os microsserviços do backend.

### Tarefas

* **Desenvolvimento Frontend**: Implementar a interface de usuário utilizando React, TypeScript, Vite e Tailwind CSS.
* **Integração com Backend**: Estabelecer comunicação entre o frontend e os microsserviços via API Gateway e mensageria.
* **Design UI/UX**: Criar o design visual e estrutural da aplicação, garantindo modularidade, consistência e usabilidade.
* **Testes de Interface**: Realizar testes de usabilidade e verificação de responsividade em diferentes dispositivos e resoluções.

### Responsáveis

* **Desenvolvedor Frontend**: Matheus Zeíta Silva
* **Desenvolvedor Frontend**: Pedro Henrique Silveira Leiva Machado
* **Desenvolvedor Frontend**: Luan Fernando dos Santos Bezerra
* **Designer UI/UX**: Luan Fernando dos Santos Bezerra
* **Engenheiro de Software / Backend de Apoio**: Pedro Fernando dos Santos Bezerra
* **Engenheiro de Software / Backend de Apoio**: Lucas Gabriel Ferreira Toti

---

### Contribuições Individuais

| **Nome**                                  | **Contribuições**                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               |
| ----------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Matheus Zeíta Silva**                   | Configurou a arquitetura do projeto web utilizando **React**, **TypeScript**, **Vite** e **Tailwind CSS**, estabelecendo uma base sólida e escalável para o desenvolvimento. Implementou componentes essenciais como **Header**, **Footer** e subcomponentes associados, com foco em responsividade e experiência do usuário. Atuou também na correção de bugs pontuais no **User Service**, garantindo estabilidade na integração frontend-backend. Além disso, contribuiu na documentação e no **mapeamento do fluxo de usuário**, descrevendo as principais interações e rotas da aplicação.                                                                                                                                                                                                                                 |
| **Lucas Gabriel Ferreira Toti**           | Focou na **arquitetura, desenvolvimento e robustez dos microserviços de Listing, Auction e Product**, aplicando **Clean Architecture** e **DDD**. Refatorou o **ProductService** para **CQRS**, centralizando regras de negócio em *Command Handlers* e padronizando a mensageria com **MassTransit** sobre **RabbitMQ**. Orquestrou o ciclo de vida dos leilões com mensagens agendadas, eliminando dependência de *schedulers*, e implementou o padrão **Read Model** para otimizar consultas. Garantiu resiliência com **Polly**, integrou IA (Gemini) para validação de produtos e resolveu *race conditions* em lances simultâneos. No **DevOps**, dockerizou todo o ambiente multi-serviço, unificou *docker-compose* e padronizou configurações via *.env*. Finalizou com uma suíte de testes unitários determinísticos. |
| **Luan Fernando dos Santos Bezerra**      | Responsável pelo **design de interface** do *Comply*, criou todas as telas no **Figma**, estruturadas de forma modular e orientadas a componentes, o que facilitou o desenvolvimento posterior. Implementou diversas telas no **frontend web**, incluindo **login**, **registro**, **criação de produtos**, **meus anúncios**, **minha lista** e **detalhes do produto**. Configurou o **React Router** e instalou bibliotecas de componentes para garantir padronização, escalabilidade e agilidade no desenvolvimento.                                                                                                                                                                                                                                                                                                        |
| **Pedro Fernando dos Santos Bezerra**     | Trabalhou na **melhoria e integração de microsserviços** no backend, seguindo **Clean Architecture** e **DDD**. Refinou o **UserService**, simplificando autenticação JWT e gerenciamento de perfis, e aprimorou o **PaymentService** com **Stripe**, **RabbitMQ** e suporte a reembolsos. Implementou o **ApiGateway com Ocelot**, garantindo roteamento centralizado e integração entre frontend e backend. Também colaborou na **organização do Kanban** e na distribuição de tarefas entre os membros do time.                                                                                                                                                                                                                                                                                                              |
| **Pedro Henrique Silveira Leiva Machado** | Ajustou o **serviço de notificação** no backend e contribuiu significativamente para o **frontend web** com a implementação do **card de produtos**. Tornou o componente dinâmico, adaptando sua aparência e comportamento de acordo com o tipo de anúncio, status do leilão e tempo restante. Garantiu exibição correta das informações financeiras e temporais, como **valor do lance atual**, **compra instantânea** e **datas de início e fim**, além de assegurar consistência visual e responsividade.                                                                                                                                                                                                                                                                                                                    |

---

### Prazo

* **Data de conclusão**: 2025-10-26

---

## Etapa 4: Desenvolvimento Mobile

### Objetivo

Desenvolver a aplicação mobile do projeto Comply, garantindo que as funcionalidades essenciais presentes na versão web fossem adaptadas para dispositivos móveis com fluidez, responsividade, segurança e integração completa com os microsserviços do backend.

### Tarefas

- **Fundação do App Mobile**: Estruturar o projeto utilizando Expo, incluindo navegação, organização modular, fluxo de autenticação e gerenciamento de estado.
- **Desenvolvimento Mobile**: Implementar telas, componentes e funcionalidades da aplicação, garantindo aderência ao design definido.
- **Integração com Backend**: Conectar as funcionalidades mobile às APIs e microsserviços existentes.
- **Testes em Dispositivos**: Validar responsividade, desempenho e compatibilidade em diferentes dispositivos Android e iOS.
- **Entrega do Build**: Gerar e disponibilizar o APK final para testes e homologação.

### Responsáveis

- **Desenvolvedor Mobile / UI Designer**: Matheus Zeíta Silva  
- **Desenvolvedor Mobile**: Pedro Henrique Silveira Leiva Machado  
- **Desenvolvedor Mobile / Engenheiro de Software**: Luan Fernando dos Santos Bezerra  
- **Desenvolvedor Mobile / Engenheiro de Software**: Pedro Fernando dos Santos Bezerra  
- **Engenheiro de Software / QA & Integração**: Lucas Gabriel Ferreira Toti  

---

### Contribuições Individuais

| **Nome** | **Contribuições** |
|---------|------------------|
| **Matheus Zeíta Silva** | Atuação no desenvolvimento do **Front-End Mobile**, incluindo a criação completa do **design das telas no Figma**, definindo identidade visual, componentes e fluxos da aplicação. Implementou as telas de **Login** e **Cadastro**, garantindo fidelidade ao layout planejado e experiência de usuário fluida. Contribuiu também com a **documentação oficial da etapa**, consolidando entregas e processos do time. |
| **Pedro Henrique Silveira Leiva Machado** | Participou ativamente no desenvolvimento mobile e web. Na versão mobile, implementou as **telas de criação de produtos**, adaptando fluxo e interface ao ambiente mobile utilizando **React Native + Expo**, além de ajustar componentes fundamentais do projeto. Ficou responsável por realizar a **build do APK final**, entregando a versão instalável da aplicação. |
| **Luan Fernando dos Santos Bezerra** | Responsável por montar **toda a fundação do app mobile**. Estruturou o projeto com **Expo Router**, criou toda a arquitetura de pastas, rotas, navegação por abas e wrappers. Implementou a camada de dados com **React Query**, desenvolvendo hooks para produtos, anúncios, pagamentos, notificações, favoritos, vendas, perfil e saldo já integrados à API. Configurou o fluxo completo de **autenticação com Redux + React Query**, com persistência via AsyncStorage, e desenvolveu **login** e **registro** usando React Hook Form. Entregou a base estrutural, lógica e operacional de todo o aplicativo. |
| **Pedro Fernando dos Santos Bezerra** | Implementou funcionalidades essenciais da versão mobile, como a página **Minhas Listagens** (incluindo integração completa com a API) e **todo o sistema de Leilões**, abrangendo criação de leilões e lances rápidos. Desenvolveu a **Gestão de Perfil do Usuário**, integrando pagamentos, retiradas e endereços. Contribuiu para melhorias no fluxo de notificações e nas funcionalidades de entregas e disputas. Atuou também com **correções no backend** e colaborou na **coordenação e delegação de tarefas** da equipe. |
| **Lucas Gabriel Ferreira Toti** | Focado na **estabilização da plataforma**, corrigiu bugs críticos relacionados às flags de bloqueio (IsProcessingXXXX), garantindo atomicidade e confiabilidade nas operações de leilão. Implementou as funcionalidades da seção **Minha Conta** na versão mobile e desenvolveu o **sistema de Refresh Token**, elevando a segurança da aplicação. Concluiu ainda uma **suíte completa de Testes de Integração End-to-End** utilizando Test Containers, validando o comportamento dos microsserviços e assegurando qualidade e robustez para todo o ecossistema mobile-backend. |

---

### Prazo

- **Data de conclusão**: 2025-11-16

---

## Etapa 5: Homologação e Apresentação da Solução Produzida

### Objetivo

Garantir que a solução desenvolvida esteja pronta para produção e apresentar o projeto às partes interessadas.

### Tarefas

- **Homologação**: Realizar testes finais com os stakeholders para validar a solução.
- **Correção de Bugs**: Corrigir quaisquer problemas encontrados durante a homologação.
- **Apresentação Final**: Preparar e conduzir a apresentação da solução final para os stakeholders.

### Responsáveis

- **Líder de Homologação**: Nome do líder
- **Gerente de Projeto**: Nome do gerente

### Prazo

- **Data de conclusão**: YYYY-MM-DD

---

## Comunicação e Colaboração

### Ferramentas de Comunicação

- **Slack**: Para comunicação diária e rápida.
- **Trello**: Para gerenciamento de tarefas e acompanhamento do progresso.
- **GitHub**: Para versionamento de código e revisão de pull requests.

### Reuniões Regulares

- **Reunião semanal**: Toda segunda-feira às 10h.
- **Reunião de revisão**: Ao final de cada etapa para revisar o progresso e ajustar o plano.

---

<img width="930" height="1168" alt="image" src="https://github.com/user-attachments/assets/6ce5b616-60c8-450f-ba6f-3d5929baad9c" />



