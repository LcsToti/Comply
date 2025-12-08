
# Metodologia

<span style="color:red">Pré-requisitos: <a href="2-Especificação do Projeto.md"> Documentação de Especificação</a></span>

Este documento descreve a metodologia de trabalho adotada pela equipe no desenvolvimento do **Comply**, incluindo os ambientes utilizados, o fluxo de versionamento e integração de código, a organização das tarefas e papéis da equipe. A metodologia visa garantir **clareza, padronização e eficiência** em todas as etapas do projeto.

## Relação de Ambientes de Trabalho

| Ambiente               | Plataforma/Ferramenta       | Link de Acesso / Observações                       |
|------------------------|----------------------------|---------------------------------------------------|
| Desenvolvimento Backend| Visual Studio / Rider (C#/.NET) | Ambiente local                                          |
| Desenvolvimento Web Frontend | Visual Studio Code (React + Vite + TypeScript + Tailwind + shadcn/ui)    | Ambiente local / navegador                                        |
| Desenvolvimento Mobile Frontend | Visual Studio Code + Android Studio (React Native) | Ambiente local / Emulador Android                        |
| Design e Protótipos    | Figma                      | [https://www.figma.com](https://www.figma.com)   |
| Diagramas e Modelagem  | LucidChart                 | [https://www.lucidchart.com](https://www.lucidchart.com) |
| Controle de Versão     | GitHub                     | [https://github.com](https://github.com)         |


## Controle de Versão
O repositório Git segue um modelo baseado em **GitFlow**, garantindo organização e estabilidade no ciclo de desenvolvimento.

`main`: Contém a versão estável e em produção do software. Nenhum desenvolvimento é feito diretamente aqui.

`feat/{nome-da-feature}`: Branches criadas para o desenvolvimento de novas funcionalidades. Ex: feature/login-usuario.

`fix/{nome-do-bug}`: Branches criadas para corrigir bugs específicos. Ex: bugfix/erro-calculo-frete.

`release/{versao}`: Branches temporárias para preparar uma nova versão para lançamento. Ex: release/v1.0.0.

### Gerência de Branches, Commits e Merges

- Commits: A equipe adota o padrão de Conventional Commits para padronizar as mensagens, facilitando a leitura do histórico e a automação de changelogs. Exemplos: feat: Adiciona autenticação JWT, fix: Corrige validação de e-mail no cadastro.

- Merges: Toda e qualquer integração de código nas branches dev e main é realizada exclusivamente através de Pull Requests (PRs). Cada PR deve ser revisado por pelo menos um outro membro da equipe de desenvolvimento antes do merge, garantindo a qualidade e o compartilhamento de conhecimento.

- Tags e Versionamento: O projeto adota o Versionamento Semântico (SemVer) no formato MAJOR.MINOR.PATCH (ex: v1.0.0).

    - PATCH (0.0.1): Incrementado para correções de bugs retrocompatíveis.

    - MINOR (0.1.0): Incrementado para adição de novas funcionalidades retrocompatíveis.

    - MAJOR (1.0.0): Incrementado para mudanças que quebram a compatibilidade (breaking changes).
      
### Gerência de Issues
As issues são categorizadas com labels específicas, que permitem fácil rastreio do progresso:

| Label         | Descrição                                                   |
|---------------|-------------------------------------------------------------|
| API           | Endpoints, integração de serviços, documentação de API      |
| Back-end      | Tarefas relacionadas ao backend (C#/.NET)                 |
| Bug           | Correção de erros ou comportamentos inesperados            |
| Database      | Modelagem, queries, migrations, SQL/NoSQL                  |
| Design / UX   | Wireframes, mockups, protótipos, experiência do usuário    |
| DevOps / Infra| CI/CD, cloud, deploy, containers, monitoramento            |
| Documentation | Documentação de código, API ou processos                   |
| Feature       | Implementação de novas funcionalidades                     |
| Front Mobile  | Tarefas de front-end mobile (React Native)                |
| Front Web     | Tarefas de front-end web (React)                           |
| Refactor      | Melhorias ou refatoração de código                         |
| Tests         | Testes unitários, integração, sistema e usabilidade        |

### Milestones
O projeto possui **15 milestones**, incluindo desde a definição de arquitetura inicial até a apresentação final. As milestones são monitoradas no GitHub, garantindo rastreabilidade e cumprimento de prazos. Exemplos:

- **1.1 – Documento de Contexto**  
- **1.2 – Especificação do Problema e Personas**  
- **1.3 – Arquitetura Inicial e Requisitos**  
- **2.1 – Arquitetura de APIs e Banco de Dados**  
- **2.2 – Implementação de Microsserviços Core**  
- **2.3 – Integração e Deploy Back-end Inicial**  
- **3.1 – Projeto de Interface Web**  
- **3.2 – Implementação de Telas Web**  
- **3.3 – Testes de Integração Web**  
- **4.1 – Projeto de Interface Mobile**  
- **4.2 – Implementação de Telas Mobile**  
- **4.3 – Testes de Sistema Mobile**  
- **5.1 – Projeto Final Completo**  
- **5.2 – Documentação e Considerações Finais**  
- **5.3 – Apresentação Final**  

## Gerenciamento de Projeto

### Divisão de Papéis

A equipe utiliza o Scrum como base para a definição de papéis, promovendo organização e responsabilidades claras.

- Scrum Master: Pedro Bezerra
- Product Owner: Lucas Toti
- Equipe de Design: Luan Bezerra
- Equipe de Desenvolvimento: Pedro Bezerra, Lucas Toti, Luan Bezerra, Matheus Zeíta e Pedro Machado.

### Processo

O processo de gerenciamento é executado através do GitHub Projects, configurado como um quadro Kanban. Este quadro visual centraliza o fluxo de trabalho e é composto pelas seguintes colunas: Backlog, Ready, In Progress, In Review e Done.

O planejamento é estruturado em 15 Milestones, que funcionam como sprints. Para cada Milestone, é criado um backlog completo de Issues, contendo todas as descrições técnicas e requisitos necessários para a implementação daquela etapa.

As Labels fornecidas são utilizadas para categorizar as issues por área (ex: Back-end, Front-Mobile), tipo (ex: Bug, Feature) ou contexto (API, Database), permitindo uma filtragem rápida e uma visão clara do trabalho a ser feito em cada frente do projeto.

### Ferramentas

| Ferramenta         | Finalidade / Justificativa                                     |
|-------------------|---------------------------------------------------------------|
| Visual Studio / Rider | Desenvolvimento backend em C#/.NET, integração nativa com Git |
| Visual Studio Code | Desenvolvimento web e mobile (React / React Native), leve e extensível |
| Android Studio     | Emulação e testes da aplicação mobile                         |
| Figma              | Criação de wireframes e protótipos, colaboração em tempo real |
| LucidChart         | Criação de diagramas e documentação visual                    |
| Slack              | Comunicação interna da equipe, integração com GitHub          |
| GitHub             | Controle de versão, gerenciamento de issues, milestones e Kanban |
