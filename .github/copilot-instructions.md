# Copilot Instructions for Pizzaria Paladar

## Visão Geral
Este projeto é um jogo idle de pizzaria com foco em expansão, upgrades e alta escalabilidade (meta: >1000 clientes simultâneos). O código foi reestruturado para ser leve, robusto e reaproveitável, evitando duplicidade de assets/scripts.

## Arquitetura Principal
- **POO (Programação Orientada a Objetos)**: Use abstração, encapsulamento, herança e polimorfismo para organizar sistemas e evitar duplicidade.
- **ECS (Entity Component System)**: Recomenda-se para gerenciar muitos NPCs/clientes e garantir performance.
- **Singleton GameManager**: Centraliza o estado do jogo, controle de saves, upgrades, reputação e dinheiro.

## Fluxo do Jogo
1. **Main Menu**: Cena inicial com Start Game, Load Game, Options, Quit.
2. **Personalização**: Tela para nome, sexo e cores do personagem.
3. **Game Loop**: Player inicia na pizzaria vazia, compra equipamentos, atende clientes, faz upgrades e contrata funcionários.

## Padrões e Convenções
- **Scripts Base**: Crie classes base para clientes, equipamentos e NPCs. Exemplo:
  - `ClienteBase` → `ClientePedestre`, `ClienteDrive`
  - `Equipamento` → `Forno`, `Freezer`, `Caixa`, `Mesa`
  - `NPCBase` → `AtendenteCaixa`, `Empacotador`
- **Object Pooling**: Implemente para clientes/NPCs para suportar milhares de instâncias sem queda de performance.
- **TaskSystem**: Reaproveite para distribuir funções entre NPCs.
- **Upgrades**: Use herança/polimorfismo para métodos de produção e evolução dos equipamentos.

## Diretórios e Arquivos-Chave
- `Assets/Scenes/`: Cenas principais (Main Menu, GameLoop, etc.)
- `Assets/Scripts/`: Scripts de lógica central (GameManager, PlayerController, ClienteBase, Equipamento, NPCBase)
- `Assets/ImagensAssets/`, `Assets/ObjetosAssets/`: Assets visuais reaproveitáveis
- `ProjectSettings/`: Configurações do projeto Unity

## Estratégia de Implementação
- Comece pelo GameManager Singleton e Main Menu Scene.
- Priorize modularidade e reaproveitamento de scripts.
- Use triggers e eventos para interações entre player, equipamentos e clientes.
- Mantenha o código limpo e documentado para facilitar upgrades e expansões futuras.

## Escalabilidade
- Sempre pense em performance: evite instanciar/destruir objetos em massa, prefira pooling.
- Estruture upgrades e progressão para permitir expansão infinita sem reescrever sistemas.

---

> Para dúvidas sobre padrões, consulte os scripts base e siga a lógica de reaproveitamento e modularidade. Adapte exemplos conforme necessário para manter o projeto leve e escalável.
