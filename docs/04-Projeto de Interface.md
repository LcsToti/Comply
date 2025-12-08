
# Projeto de Interface

<span style="color:red">Pré-requisitos: <a href="2-Especificação do Projeto.md"> Documentação de Especificação</a></span>

Visão geral da interação do usuário pelas telas do sistema e protótipo interativo das telas com as funcionalidades que fazem parte do sistema (wireframes), tanto da versão web como para a versão mobile.

A proposta do projeto é oferecer uma plataforma de anúncios e leilões online, voltada para compra, venda e gerenciamento de produtos de maneira segura, intuitiva e eficiente. O design da interface foi elaborado para atender aos requisitos funcionais e não funcionais definidos anteriormente, com foco em **usabilidade, clareza visual e fluidez de navegação**.

## Diagrama de Fluxo e Wireframes

O diagrama de fluxo representa o caminho percorrido pelo usuário ao interagir com o sistema, demonstrando as principais telas, menus e funcionalidades, bem como a relação entre elas. Essa representação auxilia na compreensão das jornadas de uso e orienta o design das telas no Figma.

### Estrutura Geral do Fluxo de Usuário

1. **Tela Inicial (Landing Page)**

   * Exibe produtos em destaque e menu superior com opções: *Acessar perfil*, *Buscar produtos*, *Publicar anúncio*.
   * Acesso direto a login e cadastro.

2. **Login / Cadastro**

   * 1. Login via e-mail e senha.
   
   * 2. Cadastro básico com nome, e-mail, senha e confirmação.
   * 3. Cadastro com Stripe.

3. **Página de Produto**

   * 1. Fotos do item, valor mínimo, botão de “Dar lance”, botão de “Compra imediata”.
   * 2. Área de cadastro de produto.
   * 3. Área de edição do produto.
   
4. **Dashboard do Usuário**

   * 1. Acesso aos produtos anunciados.
   * 2. Acesso às minhas vendas.
   * 3. Acesso aos lances realizados, notificações e suporte.

5. **Fluxo de Compra / Lance**

   * Usuário insere valor e confirma.
   * Feedback visual (notificação de sucesso, aviso de erro, tempo restante, etc.).

---

### Representação Visual (Exemplo de Fluxo)

```mermaid
flowchart TD
    A[Tela Inicial Landing Page]
    A --> B[Login ou Cadastro]
    A --> C[Dashboard do Usuario]
    A --> D[Pagina de Produto]

    %% Login / Cadastro
    B --> B1[Login via email e senha]
    B --> B2[Cadastro basico: nome, email, senha]
    B --> B3[Cadastro com Stripe]

    %% Dashboard
    C --> C1[Meus Produtos]
    C --> C2[Minhas Vendas]
    C --> C3[Lances Realizados]
    C --> C4[Notificacoes]
    C --> C5[Suporte]

    %% Pagina de Produto
    D --> D1[Fotos do item, valor minimo]
    D --> D2[Botao Dar Lance]
    D --> D3[Botao Compra Imediata]
    D --> D4[Area de cadastro do produto]
    D --> D5[Area de edicao do produto]

    %% Fluxo de Compra / Lance
    D2 --> E[Usuario insere valor e confirma]
    D3 --> E
    E --> F[Feedback visual: sucesso, erro, tempo restante]
```

## Wireframes

Os wireframes foram desenvolvidos para representar a estrutura visual das principais telas do sistema. Eles priorizam a clareza na disposição dos elementos, a hierarquia das informações e a consistência entre as versões web e mobile.

 ### Tela Inicial
 
<img width="1931" height="1334" alt="image" src="https://github.com/user-attachments/assets/b60ab707-edb9-4f57-a5ca-fbf1049966d7" />

<img width="1970" height="1310" alt="image" src="https://github.com/user-attachments/assets/82f47ae7-bda4-4eb2-a137-fe42c9733028" />

 #### Pesquisa

 <img width="2304" height="1342" alt="image" src="https://github.com/user-attachments/assets/77def8c5-db12-473d-8578-1248416c4bb7" />

 ### Login e cadastro
 
 <img width="2354" height="1344" alt="image" src="https://github.com/user-attachments/assets/420516a6-cc21-4be4-aee9-cfbb0550fc66" />

 <img width="1512" height="1228" alt="image" src="https://github.com/user-attachments/assets/0394d65d-1f0e-472b-8676-6d4e7da367ac" />

 ### Dashboard do usuário

 <img width="2103" height="1334" alt="image" src="https://github.com/user-attachments/assets/e2c7e611-ff7e-4874-8596-ac9e758db1f7" />

 #### Meus anúncios

 <img width="1603" height="1336" alt="image" src="https://github.com/user-attachments/assets/af62b96e-71f0-483e-a6db-3c4585932af4" />

 #### Editar perfil

 <img width="1956" height="1048" alt="image" src="https://github.com/user-attachments/assets/32fef429-2ab3-48ec-8f5e-ed2d7f128932" />

 ### Página do Produto

 <img width="1615" height="1466" alt="image" src="https://github.com/user-attachments/assets/f348f6d8-e4f2-4756-a9c0-4e71f9ba11a5" />

 #### Cadastro de produto

 <img width="1020" height="1180" alt="image" src="https://github.com/user-attachments/assets/0b1d84bf-f614-4b56-a41d-80680f768f31" />

<img width="1035" height="1233" alt="image" src="https://github.com/user-attachments/assets/848df69f-49ec-4a41-80ae-cd7d3e83dd78" />

<img width="1721" height="978" alt="image" src="https://github.com/user-attachments/assets/9436c226-ce7a-452e-adf0-82af1df57996" />


 #### Edição de produto

 <img width="1334" height="1463" alt="image" src="https://github.com/user-attachments/assets/3c30d2d1-0719-400f-99c1-a1a68f28e550" />



 



