# Connect-Four
Jogo Connect Four criado como tarefa do grupo de estudos de IA. O código deste exemplo foi inspirado no repositório do usuário [KoalaMoala](https://github.com/KoalaMoala/Connect4).

![](https://gph.is/g/4A6YKdQ)

[Jogue aqui online](https://arthuratlantico.github.io/Connect-Four/)

### Regras
O Coonect Four é parecido com um jogo da velha. As regras são as seguintes:
1. São necessárias 4 peças consecutivas da mesma cor para ganhar
2. Você pode formar linhas horizontais, verticais e diagonais
3. Você escolhe uma coluna e joga a peça naquela coluna
4. Se a coluna estiver ocupada, você não pode colocar nada nela

Neste jogo, o tabuleiro tem tamanho 6x7.

### Inteligência Artificial
Utilizamos o algoritmo de decisão [MCTS](Monte Carlo tree search), que consiste em 4 etapas:
1. **Seleção**: começamos pela raiz e vamos descendo na árvore até chegar numa folha. Cada nó contém uma simulação do tabuleiro
2. **Expansão**: se conseguirmos chegar no fim do jogo numa folha paramos, mas enquanto isso, cada nó vai testar quais jogadas são possíveis na simulação que ele guarda
3. **Simulação**: escolhemos uma jogada aleatória entre as possíveis, simulamos o que aconteceria naquela jogada e realizamos outra expansão
4. **Retro-propagação**: damos uma condição de parada, como tempo ou número de tentativas. Após essa condição ser alcançada, cada folha vai indicar ao nó pai se ela representa uma vitória, que por sua vez indica ao pai quantos filhos indicam vitória, assim por diante, até que a raiz saiba quais são os nós mais promissores

Ao final, temos uma lista com as escolhas e o quão promissoras elas são com base na comparação do número vitórias e o número de tentativas. É interessante que as jogadas ao mesmo tempo maximizem a minha chance de vitória e minimizem as chances do oponente.
