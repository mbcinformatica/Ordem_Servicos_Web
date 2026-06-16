using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordem_Servicos_Web.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DBClientes
            migrationBuilder.CreateTable(
                name: "DBClientes",
                columns: table => new
                {
                    IDCliente = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPessoa = table.Column<string>(maxLength: 10, nullable: false),
                    Cpf_Cnpj = table.Column<string>(maxLength: 14, nullable: false),
                    Nome_RazaoSocial = table.Column<string>(maxLength: 150, nullable: false),
                    Endereco = table.Column<string>(maxLength: 100, nullable: true),
                    Numero = table.Column<string>(maxLength: 10, nullable: true),
                    Bairro = table.Column<string>(maxLength: 50, nullable: true),
                    Municipio = table.Column<string>(maxLength: 100, nullable: true),
                    UF = table.Column<string>(maxLength: 2, nullable: true),
                    Cep = table.Column<string>(maxLength: 8, nullable: true),
                    Contato = table.Column<string>(maxLength: 50, nullable: true),
                    Fone_1 = table.Column<string>(maxLength: 11, nullable: true),
                    Fone_2 = table.Column<string>(maxLength: 11, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    DataCadastro = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBClientes", x => x.IDCliente);
                });

            // DBFornecedores
            migrationBuilder.CreateTable(
                name: "DBFornecedores",
                columns: table => new
                {
                    IDFornecedor = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoPessoa = table.Column<string>(maxLength: 10, nullable: false),
                    Cpf_Cnpj = table.Column<string>(maxLength: 14, nullable: false),
                    Nome_RazaoSocial = table.Column<string>(maxLength: 150, nullable: false),
                    Endereco = table.Column<string>(maxLength: 100, nullable: true),
                    Numero = table.Column<string>(maxLength: 10, nullable: true),
                    Bairro = table.Column<string>(maxLength: 50, nullable: true),
                    Municipio = table.Column<string>(maxLength: 100, nullable: true),
                    UF = table.Column<string>(maxLength: 2, nullable: true),
                    Cep = table.Column<string>(maxLength: 8, nullable: true),
                    Contato = table.Column<string>(maxLength: 50, nullable: true),
                    Fone_1 = table.Column<string>(maxLength: 11, nullable: true),
                    Fone_2 = table.Column<string>(maxLength: 11, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    DataCadastro = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBFornecedores", x => x.IDFornecedor);
                });

            // DBMarcas
            migrationBuilder.CreateTable(
                name: "DBMarcas",
                columns: table => new
                {
                    IDMarca = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBMarcas", x => x.IDMarca);
                });

            // DBModelos
            migrationBuilder.CreateTable(
                name: "DBModelos",
                columns: table => new
                {
                    IDModelo = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDMarca = table.Column<int>(nullable: false),
                    Descricao = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBModelos", x => x.IDModelo);
                    table.ForeignKey(
                        name: "FK_DBModelos_DBMarcas_IDMarca",
                        column: x => x.IDMarca,
                        principalTable: "DBMarcas",
                        principalColumn: "IDMarca");
                });

            // DBUnidades
            migrationBuilder.CreateTable(
                name: "DBUnidades",
                columns: table => new
                {
                    IDUnidade = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBUnidades", x => x.IDUnidade);
                });

            // DBUsuarios
            migrationBuilder.CreateTable(
                name: "DBUsuarios",
                columns: table => new
                {
                    IDUsuario = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(maxLength: 150, nullable: false),
                    Login = table.Column<string>(maxLength: 20, nullable: false, collation: "Latin1_General_CS_AS"),
                    Senha = table.Column<string>(maxLength: 150, nullable: false),
                    Endereco = table.Column<string>(maxLength: 100, nullable: true),
                    Numero = table.Column<string>(maxLength: 10, nullable: true),
                    Bairro = table.Column<string>(maxLength: 50, nullable: true),
                    Municipio = table.Column<string>(maxLength: 100, nullable: true),
                    UF = table.Column<string>(maxLength: 2, nullable: true),
                    Cep = table.Column<string>(maxLength: 8, nullable: true),
                    Fone_1 = table.Column<string>(maxLength: 11, nullable: true),
                    Fone_2 = table.Column<string>(maxLength: 11, nullable: true),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    DataCadastro = table.Column<DateTime>(nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    Imagem = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBUsuarios", x => x.IDUsuario);
                });

            // DBCategoriaServicos
            migrationBuilder.CreateTable(
                name: "DBCategoriaServicos",
                columns: table => new
                {
                    IDCategoriaServico = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBCategoriaServicos", x => x.IDCategoriaServico);
                });

            // DBServicos
            migrationBuilder.CreateTable(
                name: "DBServicos",
                columns: table => new
                {
                    IDServico = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDCodigoBase = table.Column<string>(maxLength: 20, nullable: false),
                    IDCategoriaServico = table.Column<int>(nullable: false),
                    Descricao = table.Column<string>(maxLength: 100, nullable: false),
                    ValorServico = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBServicos", x => x.IDServico);
                    table.ForeignKey(
                        name: "FK_DBServicos_DBCategoriaServicos_IDCategoriaServico",
                        column: x => x.IDCategoriaServico,
                        principalTable: "DBCategoriaServicos",
                        principalColumn: "IDCategoriaServico");
                });

            // DBProdutos
            migrationBuilder.CreateTable(
                name: "DBProdutos",
                columns: table => new
                {
                    IDProduto = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDProdutoInterno = table.Column<string>(maxLength: 50, nullable: false),
                    IDProdutoFabricante = table.Column<string>(maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(maxLength: 100, nullable: false),
                    IDFornecedor = table.Column<int>(nullable: true),
                    IDMarca = table.Column<int>(nullable: true),
                    IDModelo = table.Column<int>(nullable: true),
                    IDUnidade = table.Column<int>(nullable: true),
                    PrecoCompra = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    PrecoVenda = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    EstoqueAtual = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    EstoqueMinimo = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    DataUltimaCompra = table.Column<DateTime>(nullable: true),
                    Garantia = table.Column<string>(maxLength: 50, nullable: true),
                    Imagem = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBProdutos", x => x.IDProduto);
                    table.ForeignKey("FK_DBProdutos_DBFornecedores", x => x.IDFornecedor, "DBFornecedores", "IDFornecedor");
                    table.ForeignKey("FK_DBProdutos_DBMarcas", x => x.IDMarca, "DBMarcas", "IDMarca");
                    table.ForeignKey("FK_DBProdutos_DBModelos", x => x.IDModelo, "DBModelos", "IDModelo");
                    table.ForeignKey("FK_DBProdutos_DBUnidades", x => x.IDUnidade, "DBUnidades", "IDUnidade");
                });

            // DBLancamentoServicos
            migrationBuilder.CreateTable(
                name: "DBLancamentoServicos",
                columns: table => new
                {
                    IDOrdenServico = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataEmissao = table.Column<DateTime>(nullable: true),
                    DataConclusao = table.Column<DateTime>(nullable: true),
                    IDCliente = table.Column<int>(nullable: true),
                    IDMarca = table.Column<int>(nullable: true),
                    IDProduto = table.Column<int>(nullable: true),
                    NumeroSerie = table.Column<string>(maxLength: 100, nullable: true),
                    DescricaoDefeito = table.Column<string>(nullable: true),
                    GarantiaServico = table.Column<string>(maxLength: 50, nullable: true),
                    GarantiaMaterial = table.Column<string>(maxLength: 50, nullable: true),
                    ValorTotalServico = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    ValorTotalMaterial = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Imagem = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBLancamentoServicos", x => x.IDOrdenServico);
                    table.ForeignKey("FK_DBLancamentoServicos_DBClientes", x => x.IDCliente, "DBClientes", "IDCliente");
                    table.ForeignKey("FK_DBLancamentoServicos_DBMarcas", x => x.IDMarca, "DBMarcas", "IDMarca");
                    table.ForeignKey("FK_DBLancamentoServicos_DBProdutos", x => x.IDProduto, "DBProdutos", "IDProduto");
                });

            // Logs
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(maxLength: 20, nullable: false),
                    Message = table.Column<string>(nullable: false),
                    Exception = table.Column<string>(nullable: true),
                    Properties = table.Column<string>(type: "json", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            // DBMenu
            migrationBuilder.CreateTable(
                name: "DBMenu",
                columns: table => new
                {
                    IDMenu = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBMenu", x => x.IDMenu);
                });

            // DBItensMenu
            migrationBuilder.CreateTable(
                name: "DBItensMenu",
                columns: table => new
                {
                    IDItensMenu = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDMenu = table.Column<int>(nullable: true),
                    Descricao = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBItensMenu", x => x.IDItensMenu);
                    table.ForeignKey("FK_DBItensMenu_DBMenu", x => x.IDMenu, "DBMenu", "IDMenu");
                });

            // DBPermissoes
            migrationBuilder.CreateTable(
                name: "DBPermissoes",
                columns: table => new
                {
                    IDPermissao = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IDMenu = table.Column<int>(nullable: true),
                    IDItensMenu = table.Column<int>(nullable: true),
                    IDUsuario = table.Column<int>(nullable: true),
                    Executar = table.Column<bool>(nullable: false, defaultValue: false),
                    Criar = table.Column<bool>(nullable: false, defaultValue: false),
                    Alterar = table.Column<bool>(nullable: false, defaultValue: false),
                    Excluir = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DBPermissoes", x => x.IDPermissao);
                    table.ForeignKey("FK_DBPermissoes_DBMenu", x => x.IDMenu, "DBMenu", "IDMenu");
                    table.ForeignKey("FK_DBPermissoes_DBItensMenu", x => x.IDItensMenu, "DBItensMenu", "IDItensMenu");
                    table.ForeignKey("FK_DBPermissoes_DBUsuarios", x => x.IDUsuario, "DBUsuarios", "IDUsuario");
                });

            // SEED inicial para DBMenu
            migrationBuilder.InsertData(
                table: "DBMenu",
                columns: new[] { "IDMenu", "Descricao" },
                values: new object[,]
                {
                    { 1, "CADASTROS" },
                    { 2, "LANÇAMENTOS" },
                    { 3, "RELATÓRIOS" },
                    { 4, "CONFIGURAÇÕES" },
                    { 5, "FERRAMENTAS" }
                });

            // SEED inicial para DBItensMenu
            migrationBuilder.InsertData(
                table: "DBItensMenu",
                columns: new[] { "IDItensMenu", "IDMenu", "Descricao" },
                values: new object[,]
                {
                    { 1, 1, "CLIENTES" },
                    { 2, 1, "FORNECEDORES" },
                    { 3, 1, "PRODUTOS" },
                    { 4, 1, "SERVIÇOS" },
                    { 5, 1, "USUÁRIOS" },
                    { 6, 1, "CATEGORIA DE SERVIÇOS" },
                    { 7, 1, "MARCAS" },
                    { 8, 1, "MODELOS" },
                    { 9, 1, "UNIDADES DE MEDIDAS" },
                    { 10, 2, "SERVIÇOS" },
                    { 11, 3, "CLIENTES" },
                    { 12, 3, "FORNECEDORES" },
                    { 13, 3, "USUÁRIOS" },
                    { 14, 4, "FORMULÁRIOS" },
                    { 15, 4, "CONEXÃO BD" },
                    { 16, 4, "BACKUP" },
                    { 17, 4, "RESTAURE" },
                    { 18, 5, "IMPORTAR-CLIENTES" },
                    { 19, 5, "IMPORTAR-FORNECEDORES" },
                    { 20, 5, "IMPORTAR-PRODUTOS" },
                    { 21, 5, "IMPORTAR-SERVIÇOS" },
                    { 22, 5, "IMPORTAR-USUÁRIOS" },
                    { 23, 5, "IMPORTAR-CATEGORIA DE SERVIÇOS" },
                    { 24, 5, "IMPORTAR-MARCAS" },
                    { 25, 5, "IMPORTAR-MODELOS" },
                    { 26, 5, "IMPORTAR-UNIDADES DE MEDIDAS" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove todos os itens de menu
            migrationBuilder.DeleteData(
                table: "DBItensMenu",
                keyColumn: "IDItensMenu",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                          11, 12, 13, 14, 15, 16, 17, 18,
                                          19, 20, 21, 22, 23, 24, 25, 26 });

            // Remove todos os menus
            migrationBuilder.DeleteData(
                table: "DBMenu",
                keyColumn: "IDMenu",
                keyValues: new object[] { 1, 2, 3, 4, 5 });

            migrationBuilder.DropTable(name: "DBPermissoes");
            migrationBuilder.DropTable(name: "DBItensMenu");
            migrationBuilder.DropTable(name: "DBMenu");
            migrationBuilder.DropTable(name: "Logs");
            migrationBuilder.DropTable(name: "DBLancamentoServicos");
            migrationBuilder.DropTable(name: "DBProdutos");
            migrationBuilder.DropTable(name: "DBServicos");
            migrationBuilder.DropTable(name: "DBCategoriaServicos");
            migrationBuilder.DropTable(name: "DBUsuarios");
            migrationBuilder.DropTable(name: "DBUnidades");
            migrationBuilder.DropTable(name: "DBModelos");
            migrationBuilder.DropTable(name: "DBMarcas");
            migrationBuilder.DropTable(name: "DBFornecedores");
            migrationBuilder.DropTable(name: "DBClientes");
        }
    }
}