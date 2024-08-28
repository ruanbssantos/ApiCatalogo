using ApiCatalogo.Context;
using ApiCatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            var categorias = _context.Categorias.AsNoTracking().Include(p=> p.Produtos).ToList();

            if (categorias is null) return NotFound("Categorias não encontrados.");

            return categorias;

        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> Get()
        {
            //USAR SOMENTE AsNoTracking QUANDO OS OBJETOS DESSA CONSULTA NÃO SERÃO ALTERADOS.
            var categorias = _context.Categorias.AsNoTracking().ToList();

            if (categorias is null) return NotFound("Categorias não encontrados.");

            return categorias;

        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> Get(int id)
        {
            //FIRST LANÇA UM ERRO ENQUANTO OR DEFAULT RETORNA NULL QUANDO NÃO ENCONTRA
            var categorias = _context.Categorias.AsNoTracking().FirstOrDefault(p => p.CategoriaId == id);

            if (categorias is null) return NotFound($"Categoria com o código {id} não encontrado.");

            return categorias;
        }

        [HttpPost]
        public ActionResult Post(Categoria categoria)
        {
            if (categoria == null) return BadRequest();

            //TRABALHAR COM OS DADOS NA MEMORIA
            _context.Categorias.Add(categoria);

            //SALVA OS DADOS NO BANCO DE DADOS
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Categoria categoria)
        {
            if (id != categoria.CategoriaId) return BadRequest();

            //INFORMA AO EF CORE QUE O PRODUTOS FOI MODIFICADO E ASSIM PERMITE SALVAR
            _context.Entry(categoria).State = EntityState.Modified;

            //SALVA OS DADOS NO BANCO DE DADOS
            _context.SaveChanges();

            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var categorias = _context.Categorias.FirstOrDefault(p => p.CategoriaId == id);

            if (categorias is null) return NotFound($"Categoria com o código {id} não encontrado.");

            _context.Categorias.Remove(categorias);
            _context.SaveChanges();

            return Ok();
        }
    }
}
