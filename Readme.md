# Goiabada Atômica - API do Sistema de Segurança 🔒🚀
## ⚠️ Nota Sobre o Projeto
Este projeto foi desenvolvido para fins de estudo e como parte de um portfólio de desenvolvimento de software. O objetivo é utilizar um estudo de caso para simular um projeto real enquanto aprofundo meus conhecimentos nas tecnologias C#, .Net, Java, Spring, React, Kubernetes entre outras, e melhores práticas de desenvolvimento de software.

Portanto empresa "Goiabada Atômica" e seus clientes, como a "Sabores do Cerrado", são inteiramente fictícios e foram criados para dar um contexto de negócio realista a este desafio técnico.

---
## 1. Sobre o Projeto  
Esta API é o núcleo (core) do Sistema de Segurança da **Goiabada Atômica**.  
Ela foi projetada para ser uma solução **robusta**, **personalizável** e **extensível** para IAM (Identity and Access Management), servindo como o **cérebro 🧠** por trás da autenticação, autorização e gerenciamento de identidades para todas as aplicações cliente que a consumirem.

O projeto foi desenvolvido seguindo uma **arquitetura em camadas**, princípios **SOLID**.
---

## 2. Tecnologias e Padrões ⚙️  
Este projeto foi construído com as seguintes tecnologias e padrões:  

- **Linguagem/Framework:** C# com ASP.NET Core 💻  
- **Banco de Dados:** MySQL 🗄️  
- **ORM e Migrations:** Entity Framework Core (Code-First com Migrations) 🔄  
- **Autenticação/Autorização:** JWT Bearer Authentication 🔑  
- **Hashing de Senhas:** BCrypt.Net-Next 🔒  
- **Containerização:** Docker 🐳  
- **Logging:** Serilog (logging estruturado) 📝  
- **Documentação da API:** Swagger (OpenAPI) 📜  

---

## 3. Pré-requisitos 🛠️  
Antes de começar, garanta que você tem as seguintes ferramentas instaladas:  

- .NET SDK (versão 8 ou superior)  
- Docker Desktop 🐳  
- Um cliente de banco de dados (DBeaver, HeidiSQL, MySQL Workbench)  
- Git 🌐  

---

## 4. Configuração do Ambiente (Getting Started) 🚦  
**Passo 1: Clonar o Repositório**  
```bash
git clone https://github.com/TorugoMarega/SaboresDoCerrado.ApiAutenticacao.Net.git
```

**Passo 2: Subir o Banco de Dados com Docker**  
```bash
docker-compose up -d
```

**Passo 3: Aplicar as Migrações do Banco**  
```bash
dotnet ef database update
```

**Passo 4: Configuração Inicial Essencial (Primeira Execução)**  
⚠️ **Importante:** É necessário ter um perfil **Administrador** no banco de dados para criar o primeiro usuário admin.  
Execute este script SQL:  
```sql
use db_seguranca_dsv;
insert into tbl_roles
values (1,"Administrador",
"Perfil responsável por gerenciar o sistema, realizar cadastros de usuários, perfis, clientes e sistemas",
1);
```

**Passo 5: Rodar a Aplicação**  
```bash
dotnet run
```
A API estará disponível em: **https://localhost:7294**

---

## 5. Estrutura do Projeto 📂  
- **/Controller:** Camada de entrada da API (requisições HTTP).  
- **/Service:** Lógica de negócio principal.  
- **/Repository:** Acesso a dados (interage com o DbContext).  
- **/Model/entity:** Mapeamento das tabelas do banco.  
- **/Model/DTO:** Objetos de Transferência de Dados.  
- **/Middleware:** Middlewares customizados (Handler Global de Exceções).  
- **/Data:** ApplicationContext (DbContext).  

---

## 6. Testando a API 🧪
 ⚠️ **Importante:** É necessário ter um perfil atrelado ao role de **Administrador**, pois somente um Admin poderá cadastrar novos usuários, portanto para a primeira execução é necessário que comente ou remova o atributo de autorização na chamada do método de cadastro de usuários(PostUserAsync) na classe AuthController:

**Antes:**
```
        [HttpPost("register")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostUserAsync([FromBody] RegistrationRequestDTO registrationRequestDTO)
```

**Depois:**
```
        [HttpPost("register")]
        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> PostUserAsync([FromBody] RegistrationRequestDTO registrationRequestDTO)
```
Após a criação do primeiro usuário você pode descomentar e proteger a rota novamente, isso será alterado no futuro.

**Swagger UI**  
1. Inicie a aplicação.  
2. Acesse: **https://localhost:7294/SecApi/swagger/index.html**  
3. Para endpoints protegidos:  
   - Use `POST /auth/register` para criar um admin.  
   - Faça login com `POST /auth/login` para obter o **token JWT**.  
   - Clique em **"Authorize"** no Swagger e cole o token.  
