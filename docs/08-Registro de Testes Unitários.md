# Testes Unitários no Backend

### Payments Service Domain Tests

Os testes unitários realizados no projeto Payments.Domain.Tests demonstram uma boa cobertura sobre os principais elementos do domínio de pagamentos.
Foram implementados 49 testes unitários, todos executados com sucesso.


O foco foi validar tanto os Objetos de Valor (VOs) quanto a Entidade Payment, garantindo consistência, integridade e aderência às regras de negócio.
Essa abordagem facilita a evolução do sistema e reduz riscos de falhas em produção, tornando o backend mais robusto e confiável.

<img width="504" height="183" alt="image" src="https://github.com/user-attachments/assets/54fea9eb-ded4-4e77-8225-eccaa04040b8" />

### Listing Service Domain Tests

Os testes unitários realizados no projeto `Listing.Domain.Tests` garantem uma cobertura abrangente e rigorosa sobre o núcleo do domínio do serviço de listagens. O foco foi validar a aderência aos princípios do **Domain-Driven Design (DDD)** e da **Clean Architecture**, assegurando que toda a lógica de negócio encapsulada nos agregados, entidades e objetos de valor se comporte como esperado.
Foram implementados **72 testes unitários**, todos executados com sucesso, cobrindo os seguintes cenários críticos:
A maior parte dos testes concentrou-se no ciclo de vida e nas regras de negócio do leilão:
- **Ciclo de Vida e Transições de Estado:** Validação completa da lógica de mudança de status (`Awaiting` -> `Active` -> `Ending` -> `Success`/`Failed`), garantindo que as transições ocorram apenas sob as condições corretas de tempo, baseadas nos métodos `CalculateCurrentStatus()` e `UpdateStatus()`.
- **Regras de Negócio de Lances (`Bid`):** Testes que asseguram que um lance só pode ser feito em leilões ativos, que o valor do lance é válido e superior ao lance atual, e que a lista de lances (`Bids`) é mantida de forma consistente.
- **Criação e Validação:** Garantia de que um `Auction` só pode ser criado com `AuctionSettings` válidos (ex: `StartBidValue` positivo, `EndDate` posterior a `StartDate`).
- Testes focados no estado do produto listado, validando as regras que permitem (ou não) que um produto seja associado a um novo leilão, garantindo a integridade da relação entre os agregados.
- Verificação de que os eventos de domínio, como `AuctionStartedEvent` e `AuctionEndingSoonEvent`, são corretamente disparados durante as transições de estado apropriadas, permitindo o desacoplamento de lógicas futuras.

<img width="1029" height="347" alt="Captura de tela 2025-10-05 232705" src="https://github.com/user-attachments/assets/18ac3baf-2290-4784-867a-083573912abb" />

### Product Service Domain Tests

Os testes unitários para o domínio de produtos garantem a robustez e a consistência das regras de negócio do serviço. Com um total de 59 testes, a cobertura abrange o ciclo de vida completo do produto, focando em validações de dados, permissões de usuário e integridade de estado, conforme os princípios de Domain-Driven Design (DDD).

Os **77 testes** validam cenários críticos, incluindo:
- Criação e Validação do Produto: Garante que produtos só sejam criados com dados válidos e consistentes.
- Atualização de Dados: Assegura que apenas o vendedor possa modificar as informações do produto, mantendo o controle de acesso.
- Gerenciamento de Imagens e Q&A: Valida as regras para adicionar, remover e gerenciar imagens e perguntas/respostas, aplicando as devidas permissões de usuário.
- Destaque do Produto: Cobre a lógica de negócio para destacar um produto, incluindo as regras de tempo e autorização.

<img width="907" height="282" alt="image" src="https://github.com/user-attachments/assets/1d971c0c-cf5c-44f9-ad6f-39228c69812f" />

### Notification Service Domain Tests

O projeto Notification.Domain.Tests possui 16 testes unitários que foram executados com sucesso, garantindo uma boa cobertura dos elementos centrais do domínio de pagamentos. O objetivo desses testes foi validar tanto os Objetos de Valor (VOs) quanto a Entidade Notification, assegurando a consistência e a aderência às regras de negócio. Com isso, o backend se torna mais robusto e confiável, facilitando futuras evoluções e minimizando riscos de erros em produção.
<img width="990" height="323" alt="image" src="https://github.com/user-attachments/assets/a261d178-1165-4a62-b1e9-13299afbba34" />

### User Service Domain Tests

Os testes unitários do **User Service** concentram-se na validação das principais entidades e comportamentos relacionados à autenticação, autorização e gerenciamento de usuários dentro do ecossistema **Comply**.
O projeto segue rigorosamente os princípios da **Clean Architecture** e do **Domain-Driven Design (DDD)**, garantindo isolamento das regras de negócio e fácil manutenção do código.

Foram implementados **65 testes unitários**, todos executados com sucesso, cobrindo os seguintes aspectos fundamentais:

* **Entidade `User`:** Testes que asseguram a integridade da criação de usuários, verificando propriedades obrigatórias como `Name`, `Email`, `PasswordHash` e `Role`.
* **Autenticação e Autorização:** Validação dos fluxos de login e registro, incluindo geração e verificação de tokens JWT, garantindo segurança e conformidade com os padrões modernos de autenticação.
* **Regras de Negócio de Permissão:** Verificação de que apenas usuários com papéis administrativos podem executar operações restritas, protegendo endpoints sensíveis.
* **Objetos de Valor (Value Objects):** Testes de consistência e validação para objetos como `Email`, `Password`, e `DeliveryAddress`, garantindo que apenas dados válidos sejam aceitos no domínio.
* **Resiliência e Consistência:** Garantia de que as operações do domínio não dependam diretamente de infraestrutura, mantendo o isolamento entre camadas.

Essa bateria de testes fortalece a confiabilidade do serviço de usuários, assegurando que a autenticação, o controle de acesso e o gerenciamento de dados pessoais ocorram de maneira segura, escalável e coesa.
<img width="1215" height="600" alt="image" src="https://github.com/user-attachments/assets/6e9da609-7678-4404-bcc0-e8b18eae8e34" />

