# Ignorante

Um aplicativo console para facilitar a vida de devs que, assim como eu, não gostam de processos manuais mas precisam manter seus projetos organizados e sem resíduos. Com ele, é possível gerar arquivos .gitignore em questão de um comando, sem exigir que o desenvolvedor tenha conhecimentos sobre como montar um arquivo desse tipo nem aonde encontrar um template pronto para a linguagem que ele está utilizando.

Os comandos para ele são os seguintes:

        - list: listar todos os tipos disponíveis de .gitignore;
        - list {filtro}: listar os tipos de .gitignore que contenham o filtro informado;
        - show {tipo}: consultar o .gitignore disponível  do tipo;
        - gitignore {tipo}: gerar o .gitignore para o tipo informado;

Através destes comandos o app consome a API do site gitignore.io e gera o .gitignore sem maiores interferências manuais, bastando ao utilizador colocar o executável na pasta em que deseja gerar o arquivo.


## História

Desenvolvi o Ignorante porque meus repositórios do Git sempre continham os arquivos temporários que eram gerados pelo Visual Studio, as DLLs compiladas dos meus programas, os pacotes de dependência do Node.js, e por aí vai. Não tinha percebido isso até que (semestre pssado) tive que entregar um trabalho com cerca de estonteantes 100MB ao meu professor de web apenas por causa dos pacotes de dependências que o Node havia baixado. Quando ele pediu para adicionar um .gitignore para que o projeto não ficasse imenso eu não soube como fazer.

Na última sexta estava conversando com uns amigos enquanto fazíamos um trabalho da faculdade, quando um deles (o Éliton), foi criar o .gitignore no nosso projeto. Ele magicamente abriu um site (gitignore.io) que continha centenas de tipos de .gitignore para cada linguagem de programação que se pudesse imaginar, copiou o conteúdo do template para a linguagem que estávamos utilizando, colou no Visual Studio Code, salvou o arquivo como ".gitignore" e colou na pasta do projeto. Notei duas coisas: 1 - durante todo esse tempo em que meus repositórios ficaram poluídos existia uma solução ridiculamente simples para o problema; e 2 - toda vez que eu quisesse criar um arquivo .gitignore, teria que repetir passo a passo todas as etapas que o meu amigo executou, um processo bem chato pra falar a verdade.

Como atualmente estou estudando sobre APIs e elas geralmente seguem um padrão de rota "{endereço do site}/api", resolvi fazer o teste para o gitignore.io. Adicionei o "/api" à URL do site e... tcharam! O site tinha uma API pública. Fiquei muito feliz, porque assim eu sabia como resolver a segunda coisa que eu havia notado sexta-feira: com a API, eu poderia criar um serviço que consome as informações do site e faz todo o processo trabalhoso para mim.
