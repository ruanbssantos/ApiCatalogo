using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("Primeiro")]
        [HttpGet("/Primeiro")] // "/" IGNORA A ROTA PADRÃO DO CONTROLADOR
        public async Task<ActionResult<Produto>> GetPrimeiro()
        {
            var produtos = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync();

            if (produtos is null) return NotFound("Produtos não encontrados.");

            return produtos;

        }  

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> Get()
        {
            var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
            
            if (produtos is null)  return NotFound("Produtos não encontrados.");
            
            return produtos;

        }

        [HttpGet("{id:int:min(1)}/{param2=Caderno}", Name = "ObterProduto")]
        public async Task<ActionResult<Produto>> Get(int id, string param2)
        {
            //FIRST LANÇA UM ERRO ENQUANTO OR DEFAULT RETORNA NULL QUANDO NÃO ENCONTRA
            var produtos = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.ProdutoId == id);

            if (produtos is null) return NotFound($"Produtos com o código {id} não encontrado.");

            return Ok(new { produtos , param2 =  param2 });
        }

        [HttpPost]
        public async Task<ActionResult> Post(Produto produto)
        {
            if (produto == null) return BadRequest();

            //TRABALHAR COM OS DADOS NA MEMORIA
            await _context.Produtos.AddAsync(produto);

            //SALVA OS DADOS NO BANCO DE DADOS
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId) return BadRequest();

            //INFORMA AO EF CORE QUE O PRODUTOS FOI MODIFICADO E ASSIM PERMITE SALVAR
            _context.Entry(produto).State = EntityState.Modified;

            //SALVA OS DADOS NO BANCO DE DADOS
            await _context.SaveChangesAsync();

            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id) 
        {
            var produtos = await _context.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);

            if (produtos is null) return NotFound($"Produtos com o código {id} não encontrado.");

            _context.Produtos.Remove(produtos);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
