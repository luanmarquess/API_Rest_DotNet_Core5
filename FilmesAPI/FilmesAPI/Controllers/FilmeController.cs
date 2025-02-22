﻿using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmesAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilmeController : ControllerBase
    {
        private FilmeContext _context; // acessar o contexto do banco de dados
        private IMapper _mapper;

        public FilmeController(FilmeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper; // inicia o automapper no controller 
        }

        [HttpPost]// padrao rest para criar um recurso no sistema
        public IActionResult AdicionaFilme([FromBody] UpdateFilmeDto filmeDto) //[FromBody] Indica que o filme recebido vem do corpo da requisicao
        {
            Filme filme = _mapper.Map<Filme>(filmeDto); // converte um filme Dto para um Filme

            _context.Filmes.Add(filme); // usa o contexto de filme e adiciona no banco
            _context.SaveChanges(); //Salva a alteracao no banco de dados
            return CreatedAtAction(nameof(RecuperaFilmesPorId), new { Id = filme.Id }, filme); // retorna 201(created)
            /* CreatedAtAction() Retorna status da requisicao e local onde esta sendo criado no Headers
             nameof(RecuperaFilmesPorId) == parametro passado informando a action que recupela o filme criado
             new {Id = filme.Id} == valor que se encontra na rota. Id == id filme criado
             filme == object que esta sendo tratado
            */
        }

        [HttpGet]
        public IEnumerable<Filme> RecuperaFilmes() //usa o IEnumerable<Filmes> para nao quebrar no momento que nao for retornar uma lista de filmes
        {
            return _context.Filmes;
        }

        [HttpGet("{id}")] //informar que recebe id por parametro na requisicao
        public IActionResult RecuperaFilmesPorId(int id)
        {
            Filme filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id); // retorna primeiro elemento ou algum padrao onde o primeiro elemento e um filme e o id do filme e igual ao id passado por parametro
            if (filme != null)
            {
                ReadFilmeDto filmeDto = _mapper.Map<ReadFilmeDto>(filme);
                return Ok(filmeDto); // Ok() NotFound() tipo objectResult por tando Tipo do metodo e IActionResult
            }
            return NotFound();
            /*
               foreach(Filme filme in filmes){
                   if(filme.Id == id)
                   {
                       return filme;
                   }
               }
               return null;
            */
        }

        [HttpPut("{id}")]
        public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
        {
            Filme filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id); // codigo duplicado. Isolar em uma classe
            if (filme == null)
            {
                return NotFound();
            }
            _mapper.Map(filmeDto, filme); // Mesmo tipo. Sobrescrece as info do filmeDto com as infos do filme
            _context.SaveChanges();
            return NoContent(); // Boa pratica para retorno de atualizacoes
        }

        [HttpDelete("{id}")]
        public IActionResult DeletaFilme(int id)
        {
            Filme filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
            if(filme == null)
            {
                return NotFound();
            }
            _context.Remove(filme); // caso exista o id, remove o filme
            _context.SaveChanges();
            return NoContent();
        }
    }
}
