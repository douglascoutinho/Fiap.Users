# Fiap.Users

API de usuários da plataforma. Ela gerencia o cadastro e a autenticação de usuários (tokens **JWT**), persiste os dados em **SQLite** e se integra ao **RabbitMQ** para a comunicação com os demais serviços (por exemplo, o serviço de Notifications consome a fila `user-created-event`).

## Estrutura do projeto

```
.
├── Fiap.Users.Api/              # API ASP.NET Core 8 (controllers, Swagger, autenticação)
│   └── Dockerfile
├── Fiap.Users.Domain/           # Entidades e regras de domínio
├── Fiap.Users.Infra/            # Acesso a dados (SQLite) e users_data_base.db
├── docker-compose.yml           # Ambiente local (RabbitMQ + API)
└── k8s/                         # Manifestos Kubernetes
    ├── 01-secret-user-database.yaml
    ├── 02-configmap-users.yaml
    ├── 02-secret-users-key.yaml
    ├── 03-deployment-users.yaml
    └── 04-svc-users-api.yaml
```

## Pré-requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) (execução local sem container)
- [Docker](https://www.docker.com/) e Docker Compose
- [kubectl](https://kubernetes.io/docs/tasks/tools/) e um cluster local (kind, minikube ou Docker Desktop) para o deploy em Kubernetes

## Configuração

Em variáveis de ambiente, o `:` das configurações do `appsettings` vira `__`.

| Variável | Descrição | Exemplo |
|---|---|---|
| `ASPNETCORE_URLS` | Endereço de escuta da API | `http://+:8081` |
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicação | `Development` |
| `ConnectionStrings__DefaultConnection` | String de conexão do SQLite | `Data Source=/app/database/users_data_base.db` |
| `SQLITE_URL` | Caminho do banco (injetado pelo Secret no Kubernetes) | `Data Source=/app/database/users_data_base.db` |
| `Jwt__Key` | Chave de assinatura dos tokens JWT | definida no Secret `users-key-credentials` |
| `RabbitMQ__HostName` | Host do RabbitMQ | `rabbitmq` (Compose) / `rabbitmq-service` (Kubernetes) |
| `RabbitMQ__Port` | Porta AMQP | `5672` |
| `RabbitMQ__UserName` | Usuário do RabbitMQ | `admin` |
| `RabbitMQ__Password` | Senha do RabbitMQ | `admin1234` |
| `RabbitMQ__VirtualHost` | Virtual host | `/` |

## Executando com Docker Compose

O `docker-compose.yml` sobe o RabbitMQ (com volume e healthcheck) e a API, que só inicia depois que o RabbitMQ estiver saudável.

```bash
docker compose up --build -d
docker compose logs -f api-users
docker compose down            # para os containers (mantém o volume do RabbitMQ)
docker compose down -v         # para e apaga o volume do RabbitMQ
```

| Serviço | URL |
|---|---|
| Swagger da API | http://localhost:8081/swagger/index.html |
| RabbitMQ (painel) | http://localhost:15672 (usuário `admin`, senha `admin1234`) |
| RabbitMQ (AMQP) | `localhost:5672` |

## Docker

O `Dockerfile` usa **multi-stage build** (`base` → `build` → `publish` → `final`): o SDK 8.0 compila e publica, e a imagem final usa apenas o runtime ASP.NET 8.0. O contexto de build é a **raiz da solução**, porque o Dockerfile copia os projetos `Api`, `Domain` e `Infra`.

```bash
docker build -f Fiap.Users.Api/Dockerfile -t <usuario>/src-api-users:latest .
docker push <usuario>/src-api-users:latest
```

Detalhes da imagem:

- O arquivo `Fiap.Users.Infra/users_data_base.db` é **copiado para dentro da imagem**, em `/app/database/users_data_base.db`.
- A pasta `/app/database` recebe permissão para o usuário `app`, para a aplicação conseguir gravar no banco.
- A aplicação roda com o usuário não-root `app` e expõe a porta `8081`.

## Executando no Kubernetes

### Pré-requisito: RabbitMQ no cluster

Os manifestos deste projeto **não criam o RabbitMQ**. O Deployment depende do ConfigMap `configmap-rabbitmq` e do Secret `rabbitmq-credentials`, que vêm do projeto **Fiap.Notifications** (`rabbitmq-service`). Aplique-os antes, senão o pod fica em `CreateContainerConfigError`.

### Recursos

| Arquivo | Recurso | Descrição |
|---|---|---|
| `01-secret-user-database.yaml` | Secret `users-data-base-credentials` | Caminho do banco SQLite (`SQLITE_URL`) |
| `02-configmap-users.yaml` | ConfigMap `users-key-config` | `JWT_KEY` (não referenciado pelo Deployment atual) |
| `02-secret-users-key.yaml` | Secret `users-key-credentials` | Chave JWT (`Jwt__Key`) |
| `03-deployment-users.yaml` | Deployment `users-api-deployment` | API na porta 8081, com readiness probe TCP |
| `04-svc-users-api.yaml` | Service `users-api` | `ClusterIP`, porta **8091** → container 8081 |

### Aplicar

Os Secrets precisam existir antes do Deployment.

```bash
kubectl apply -f k8s/
kubectl get pod,svc,endpoints
```

Para remover tudo: `kubectl delete -f k8s/`.

### Comunicação entre serviços

Dentro do cluster, os serviços se encontram pelo **nome do Service**:

| Serviço | Endereço no cluster | Porta do container |
|---|---|---|
| users-api | `http://users-api:8091` | 8081 |
| catalog-api | `http://catalog-api:8092` | 8082 |
| notifications-api | `http://notifications-api:9093` | 8083 |
| payments-api | `http://payments-api:8094` | 8084 |
| rabbitmq-service | `rabbitmq-service:5672` | 5672 |

### Acessando da sua máquina

Nomes de Service só existem dentro do cluster. Para abrir no navegador:

1. Adicione o nome ao arquivo `hosts` (edite como administrador):

   ```
   # Windows: C:\Windows\System32\drivers\etc\hosts
   127.0.0.1 users-api
   ```

2. Mantenha um port-forward ativo, em um terminal separado:

   ```bash
   kubectl port-forward svc/users-api 8091:8091
   ```

3. Acesse http://users-api:8091/swagger/index.html.

Alternativa sem editar o `hosts`: http://127.0.0.1:8091/swagger/index.html.

### Verificação

```bash
kubectl get pod,svc,endpoints
kubectl logs deploy/users-api-deployment -f
```

## Problemas comuns

| Sintoma | Causa provável | O que fazer |
|---|---|---|
| Pod em `CreateContainerConfigError` | Secret ou ConfigMap não existem (inclusive `configmap-rabbitmq` e `rabbitmq-credentials`) | Aplicar os Secrets e o RabbitMQ antes do Deployment |
| `DNS_PROBE_FINISHED_NXDOMAIN` no navegador | O nome não está no `hosts` | Adicionar a entrada (veja acima) |
| `ERR_CONNECTION_REFUSED` | Port-forward parado ou nome apontando para outro IP | Subir o `port-forward` e conferir com `ping users-api` |
| `Unable to listen on port ...: bind` | Porta já em uso (port-forward antigo) | Encerrar o processo `kubectl` anterior |
| `Endpoints` do Service como `<none>` | Selector não bate ou pod não está `Ready` | Conferir `app: svc-users-api` e `kubectl describe pod` |
| Erro de acesso ao banco SQLite | Pasta `/app/database` sem permissão ou string de conexão incorreta | Conferir `SQLITE_URL` e `ConnectionStrings__DefaultConnection` |

## Pontos de atenção

- **Banco SQLite dentro do container:** o `.db` vai embutido na imagem e não há volume nem PersistentVolumeClaim. Os dados gravados se perdem quando o container ou o pod é recriado. Para persistir, monte um volume em `/app/database`.
- **Chave JWT em texto puro:** a mesma chave está no Secret `users-key-credentials` e no ConfigMap `users-key-config`. Em produção, gere uma chave própria, crie o Secret por `kubectl create secret` e não o versione. O ConfigMap não é usado pelo Deployment e pode ser removido.
- **Senha do RabbitMQ no repositório:** o Secret e o `docker-compose.yml` trazem `admin1234`. Serve para estudo.
- **Chave JWT no Docker Compose:** o `docker-compose.yml` não define `Jwt__Key`, então a aplicação usa o valor do `appsettings`. Para igualar ao Kubernetes, adicione `Jwt__Key` nas variáveis de ambiente.
- **Variáveis do banco:** o Dockerfile define `ConnectionStrings__DefaultConnection` e `SQLITE_URL="url"` (valor de exemplo), e o Kubernetes injeta `SQLITE_URL` pelo Secret. Confirme qual delas a aplicação lê.
- **Ambiente `Development`:** o Dockerfile e o Deployment fixam `ASPNETCORE_ENVIRONMENT=Development`. Em produção, use `Production`.
- **Imagem `:latest`:** com `imagePullPolicy: Always`, dificulta o rollback. Prefira uma tag de versão.
- **Dois arquivos com prefixo `02-`:** o ConfigMap e o Secret da chave. Renumere para deixar a ordem de aplicação clara.
- **Versões do RabbitMQ:** o Compose usa `3.11-management` e o Kubernetes (projeto Notifications) usa `3.13-management`.
- **APIS:** para simular o processo completo é preciso baixar as apis: catalog, payments e notifications.
- **Gerar Token Padrão :** Login: douglas.loc@hotmail.com Senha: Admin@123.
