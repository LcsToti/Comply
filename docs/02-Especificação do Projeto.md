# Especificações do Projeto

Definição do problema e ideia de solução a partir da perspectiva do usuário. É composta pela definição do  diagrama de personas, histórias de usuários, requisitos funcionais e não funcionais além das restrições do projeto.

Tópicos:
- [Personas](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#personas)
- [Histórias de Usuários](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#hist%C3%B3rias-de-usu%C3%A1rios)
- [Modelagem do Processo de Negócio](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#modelagem-do-processo-de-neg%C3%B3cio)
- [Indicadores de Desempenho](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#indicadores-de-desempenho)
- [Requisitos](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#requisitos)
- [Restrições](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#restri%C3%A7%C3%B5es) 
- [Diagramas de Casos de Uso](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#diagrama-de-casos-de-uso)
- [Matriz de Rastreabilidade](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#matriz-de-rastreabilidade)
- [Gerenciamento de Projeto](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#gerenciamento-de-projeto)
- [Gestão de Orçamento](https://github.com/ICEI-PUC-Minas-PMV-ADS/pmv-ads-2025-2-e4-proj-infra-t4-comply/blob/main/docs/02-Especifica%C3%A7%C3%A3o%20do%20Projeto.md#gest%C3%A3o-de-or%C3%A7amento)
## Personas

|NOME |IDADE |OCUPAÇÃO |PERFIL |OBJETIVO |PROBLEMA | COMPORTAMENTO | 
|-----|------|---------|-------|---------|---------|---------------| 
| **1. Ana Varella** | 28 anos | Analista de Marketing | **`Vendedora casual`** | Vender itens que já não usa, como eletrônicos ou roupas, e antiguidades. Deseja um valor justo pelos produtos. | Não sabe estabelecer um preço razoável para o que vende. Não tem tempo de anunciar em múltiplas plataformas. Tem medo de cair em golpes. | Usa redes sociais diariamente e apps de venda, como OLX e Enjoei. Prefere soluções simples, mas seguras. | 
| **2. Maria Testoni** | 22 anos | Estudante universitária | **`Compradora iniciante`** | Deseja comprar itens por preços justos, bem como itens exclusivos. Tem curiosidade em começar a participar de vendas por lances. | Não conhece as regras desse tipo de venda ou como dar lances corretamente. Tem medo de cometer erros ou perder dinheiro por falta de experiência. | Navega muito em redes sociais e apps novos por curiosidade. Prefere interfaces intuitivas, tutoriais rápidos e feedback visual claro. | 
| **3. Lucas Corrêa** | 24 anos | Estudante de engenharia | **`Comprador impaciente`** | Adquirir produtos desejados rapidamente sem esperar o final da venda por lances. Evitar perder produtos para outros concorrentes. | Impaciência com vendas por lances que sejam demoradas. Receio de perder tempo dando lances apenas para não ganhar. | Frequenta apps de marketplace e e-commerce diariamente. Valoriza facilidade de pagamento e notificações em tempo real. |
| **4. Rafael Pechin** | 32 anos | Designer Gráfico | **`Caçador de descontos`** | Conseguir produtos por preços mais baixos que os de mercado. Obter a melhor oferta possível de um produto. | Dificuldade de encontrar produtos por preços baixos. Dificuldade em saber se está realmente fazendo um bom negócio. | Pesquisa preços e compara produtos em múltiplas plataformas antes de comprar. Participa de comunidades online de descontos e promoções. Gosta de negociar. | 

## Histórias de Usuários

Com base na análise das personas forma identificadas as seguintes histórias de usuários:

|EU COMO... `PERSONA`| QUERO/PRECISO ... `FUNCIONALIDADE` |PARA ... `MOTIVO/VALOR`                 |
|--------------------|------------------------------------|----------------------------------------|
| Ana Varella<br>`Vendedora casual` | Anunciar rapidamente meus itens, com fotos e descrições | Não perder tempo anunciado em várias plataformas |
| Ana Varella<br>`Vendedora casual` | Oferecer meus produtos para que compradores façam lances de valor | Conseguir o melhor preço possível pelo que vendo |
| Ana Varella<br>`Vendedora casual` | Definir um valor mínimo para cada produto | Ter segurança de que não vou vender por um preço abaixo do desejado |
| Ana Varella<br>`Vendedora casual` | Receber meu pagamento de forma segura e garantida pela plataforma | Não correr risco de cair em golpes |
| Maria Testoni<br>`Compradora iniciante` | Ter um tutorial sobre como participar das vendas com lances | Entender as regras e não cometer erros ao dar propostas por produtos |
| Maria Testoni<br>`Compradora iniciante` | Receber feedback visual claro ao dar lances | Para que eu saiba que meu lance foi registrado corretamente |
| Maria Testoni<br>`Compradora iniciante` | Fazer perguntas ao vendedor sobre um produto antes de dar um lance | Ter certeza de que o item atende às minhas expectativas e evitar problemas futuros |
| Lucas Corrêa<br>`Comprador impaciente` | Ter a opção de devolver um produto caso ele não corresponda à descrição do anúncio | Ter segurança na minha compra e não perder dinheiro com um item inadequado |
| Lucas Corrêa<br>`Comprador impaciente` | Receber notificações em tempo real quando alguém fizer um lance maior que o meu | Decidir se aumento o lance sem perder tempo |
| Lucas Corrêa<br>`Comprador impaciente` | Ter a opção de compra imediata, sem esperar o leilão terminar | Garantir rapidamente o produto desejado |
| Rafael Pechin<br>`Caçador de descontos` | Ordenar resultados por produtos que tiveram seu preço (imediato ou mínimo) reduzido recentemente | Participar de ofertas vantajosas |
| Rafael Pechin<br>`Caçador de descontos` | Ordenar resultados por produtos que foram anunciados há muito tempo e não tiveram lances | Encontrar vendedores dispostos a negociar o valor de seus produtos |

## Requisitos

As tabelas que se seguem apresentam os requisitos funcionais e não funcionais que detalham o escopo do projeto. Para determinar a prioridade de requisitos, aplicar uma técnica de priorização de requisitos e detalhar como a técnica foi aplicada.

### Requisitos Funcionais

| ID   | Descrição                                                                 | Prioridade | Responsável | Complexidade | Status |
|------|---------------------------------------------------------------------------|------------|-------------|--------------|--------|
| RF01 | O sistema deve permitir cadastro e autenticação de usuários | Alta | Matheus Zeíta, Pedro Bezerra | Simples | Concluído |
| RF02 | O sistema deve permitir que vendedores cadastrem e gerenciem seus produtos | Alta | Luan Bezerra | Complexo | Concluído |
| RF03 | O sistema deve permitir que o vendedor configure o modelo de lances de seus produtos, como o prazo e o valor de lance mínimo e lance vencedor | Alta | Lucas Toti | Simples | Concluído |
| RF04 | O sistema deve permitir que os usuários adquiram produtos por meio de lances | Alta | Lucas Toti | Complexo | Concluído |
| RF05 | O sistema deve permitir que os usuários adquiram produtos por meio da compra imediata | Alta | Lucas Toti | Simples | Concluído |
| RF06 | O sistema deve enviar notificações para vendas, lances, compras e outras ações na aplicação | Média | Pedro Bezerra, Lucas Toti | Complexo | Concluído |
| RF07 | O sistema deve permitir ordenação e filtros de produtos por suas características | Baixa | Lucas Toti | Moderado | Concluído |
| RF08 | O sistema deve permitir ordenação e filtros de produtos por leilões sendo os mais antigos, mais lances e sem lances | Baixa | Lucas Toti | Moderado | Concluído |
| RF09 | O sistema deve integrar sistema de pagamentos seguro com repasse garantido ao vendedor | Alta | Pedro Bezerra | Complexo | Concluído |
| RF10 | O sistema deve oferecer métodos de administração para controle e proteção dos produtos | Média | Matheus Zeíta | Simples | Rejeitado |
| RF11 | O sistema deve permitir que usuários solicite a devolução de uma compra dentre os termos legais | Média | Pedro Bezerra | Complexo | Concluído |
| RF12 | O sistema deve disponibilizar um campo de perguntas e respostas em cada produto | Baixa | Luan Bezerra | Simples | Concluído |
| RF13 | O sistema deve fornecer ao usuário a emissão de tickets de suporte e atendimento | Baixa | Pedro Henrique | Moderada | Concluído |
| RF14 | O sistema deve permitir que os usuários possam confirmar o envio e recebimento de um produto. | Alta | Pedro Bezerra | Simples | Concluído |
| RF15 | O sistema deve permitir que o usuário abra uma disputa para relatar problemas com sua compra. | Baixa | Pedro Bezerra | Simples | Concluído |

### Requisitos não Funcionais

|ID     | Descrição do Requisito  |Prioridade |
|-------|-------------------------|----|
| RNF01 | O sistema deve ser responsivo para rodar em diferentes resoluções e telas | MÉDIA | 
| RNF02 | O sistema deve oferecer versão mobile desenvolvida em React Native, garantindo execução em dispositivos Android e iOS nativos | MÉDIA |
| RNF03 | O sistema deve processar requisições do usuário em no máximo 3s |  BAIXA | 
| RNF04 | O site deve ser compatível em navegadores como Chrome, Edge, Firefox | ALTA |
| RNF05 | O serviço deve estar disponível 99% do tempo | ALTA |
| RNF06 | A aplicação deve guiar o usuário sobre o funcionamento do sistema de lances | ALTA |
| RNF07 | O sistema deve fornecer feedback visual imediato e claro para as ações do usuário, como a confirmação de um lance, mensagens de erro e sucesso. | ALTA |

## Restrições

O projeto está restrito pelos itens apresentados na tabela a seguir.

|ID| Restrição                                             |
|--|-------------------------------------------------------|
|01| O projeto deverá ser entregue até o final do semestre |
|02| O front-end da aplicação web deve ser desenvolvido utilizando o framework React.        |
|03| A versão mobile deve ser desenvolvida obrigatoriamente em React Native, para garantir a compatibilidade com Android e iOS. |
|04| O sistema de pagamentos será simulado, não envolvendo transações financeiras reais.        |
|05| A equipe é fixa e composta pelos membros do grupo, sem possibilidade de terceirização.        |

## Diagrama de Casos de Uso

<img width="1021" height="659" alt="image" src="https://github.com/user-attachments/assets/fd4eb9d6-54a0-4f6e-a4bf-eea17617a21e" />


## Gerenciamento de Projeto

De acordo com o PMBoK v6 as dez áreas que constituem os pilares para gerenciar projetos, e que caracterizam a multidisciplinaridade envolvida, são: Integração, Escopo, Cronograma (Tempo), Custos, Qualidade, Recursos, Comunicações, Riscos, Aquisições, Partes Interessadas. Para desenvolver projetos um profissional deve se preocupar em gerenciar todas essas dez áreas. Elas se complementam e se relacionam, de tal forma que não se deve apenas examinar uma área de forma estanque. É preciso considerar, por exemplo, que as áreas de Escopo, Cronograma e Custos estão muito relacionadas. Assim, se eu amplio o escopo de um projeto eu posso afetar seu cronograma e seus custos.

### Gerenciamento de Tempo

![image](https://github.com/user-attachments/assets/2683c802-c097-404f-b218-60a1af688708)

![image](https://github.com/user-attachments/assets/93b71eef-c36a-4569-acb8-c95103889157)


### Gerenciamento de Equipe

O gerenciamento adequado de tarefas contribuirá para que o projeto alcance altos níveis de produtividade. Por isso, é fundamental que ocorra a gestão de tarefas e de pessoas, de modo que os times envolvidos no projeto possam ser facilmente gerenciados. 

![Simple Project Timeline](img/02-project-timeline.png)

## Gestão de Orçamento

O processo de determinar o orçamento do projeto é uma tarefa que depende, além dos produtos (saídas) dos processos anteriores do gerenciamento de custos, também de produtos oferecidos por outros processos de gerenciamento, como o escopo e o tempo.

| **Recursos Necessários** | **Valor (R$)** |
|---------------------------|----------------|
| Recursos Humanos (Dev, UX/UI, QA, PM – 6 meses) | 120.000,00 |
| Hardware/Infraestrutura (servidores, máquinas dev) | 15.000,00 |
| Rede / Hospedagem Cloud (AWS/Azure/GCP – 6 meses) | 6.000,00 |
| Software (licenças, ferramentas de design, IDEs, SaaS) | 8.000,00 |
| Serviços Terceirizados (consultorias, marketing inicial) | 12.000,00 |
| **TOTAL** | **161.000,00** |
