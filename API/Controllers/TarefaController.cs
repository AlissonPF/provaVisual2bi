using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Models;

namespace API.Controllers;

[Route("api/tarefa")]
[ApiController]
public class TarefaController : ControllerBase
{
    private readonly AppDataContext _context;

    public TarefaController(AppDataContext context) =>
        _context = context;

    // GET:	api/tarefa/concluidas
    [HttpGet]
    [Route("concluidas")]
    public IActionResult Concluidas()
    {
        try
        {
            List<Tarefa> Tarefas = null;
            List<Tarefa> TarefasTemp = _context.Tarefas.Include(x => x.Categoria).ToList();
            foreach (Tarefa tarefa in TarefasTemp)
            {
                if(tarefa.Status == "Concluída"){
                    Tarefas.Add(tarefa);
                }
            }
            return Ok(Tarefas);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    
    // GET:	api/tarefa/naoconcluidas
    [HttpGet]
    [Route("naoconcluidas")]
    public IActionResult NaoConcluidas()
    {
        try
        {
            List<Tarefa> Tarefas = null;
            List<Tarefa> TarefasTemp = _context.Tarefas.Include(x => x.Categoria).ToList();
            foreach (Tarefa tarefa in TarefasTemp)
            {
                if(tarefa.Status == "Não iniciada" || tarefa.Status == "Em andamento"){
                    Tarefas.Add(tarefa);
                }
            }
            return Ok(Tarefas);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    // PATCH: api/tarefa/alterar
    [HttpPatch]
    [Route("alterar")]
    public IActionResult Alterar([FromRoute] int id, [FromBody] Tarefa tarefa)
    {
        try
        {
            Tarefa? tarefaCadastrada =
                _context.Tarefas.FirstOrDefault(x => x.TarefaId == id);
            if (tarefaCadastrada != null)
            {
                if(tarefaCadastrada.Status == "Não iniciada"){
                    tarefaCadastrada.Status = "Em andamento";
                }else if(tarefaCadastrada.Status == "Em andamento"){
                    tarefaCadastrada.Status = "Concluída";
                }
                _context.SaveChanges();
                return Ok(tarefa);
            }
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    // GET: api/tarefa/listar
    [HttpGet]
    [Route("listar")]
    public IActionResult Listar()
    {
        try
        {
            List<Tarefa> tarefas = _context.Tarefas.Include(x => x.Categoria).ToList();
            return Ok(tarefas);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // POST: api/tarefa/cadastrar
    [HttpPost]
    [Route("cadastrar")]
    public IActionResult Cadastrar([FromBody] Tarefa tarefa)
    {
        try
        {
            Categoria? categoria = _context.Categorias.Find(tarefa.CategoriaId);
            if (categoria == null)
            {
                return NotFound();
            }
            tarefa.Categoria = categoria;
            tarefa.Status = "Não iniciada";
            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();
            return Created("", tarefa);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}