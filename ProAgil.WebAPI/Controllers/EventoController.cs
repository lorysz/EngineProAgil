using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProAgil.Domain;
using ProAgil.Repository;
using ProAgil.WebAPI.Dtos;

namespace ProAgil.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventoController : ControllerBase
    {
        private readonly IProAgilRepository _repo;
        private readonly IMapper _mapper;

        public EventoController(IProAgilRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
               var eventos = await _repo.GetAllEventoAsync(true);
               var results = _mapper.Map<EventoDto[]>(eventos);

               return Ok(results); 
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }
            
        }

        [HttpGet("{EventoId}")]
        public async Task<IActionResult> Get(int EventoId)
        {
            try
            {
               var evento = await _repo.GetEventoAsyncById(EventoId, true);
               var results = _mapper.Map<EventoDto>(evento);
               
               return Ok(results); 
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }
            
        }

        [HttpGet("getByTema{tema}")]
        public async Task<IActionResult> Get(string tema)
        {
            try
            {
                var eventos = await _repo.GetAllEventoAsyncByTema(tema, true);
                var results = _mapper.Map<EventoDto[]>(eventos);

               return Ok(results); 
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(EventoDto model)
        {
            try
            {
                var evento = _mapper.Map<Evento>(model);

               _repo.Add(evento);

               if (await _repo.SaveChangesAsync())
               {
                   return Created($"/api/evento/{evento.Id}", _mapper.Map<EventoDto>(model));
               }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }

            return BadRequest();
            
        }

        [HttpPut("{EventoId}")]
        public async Task<IActionResult> Put(int EventoId, EventoDto model)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, false);

                if (evento == null)
                {
                    return NotFound();
                }
               _repo.Update(evento);

               if (await _repo.SaveChangesAsync())
               {
                   return Created($"/api/evento/{evento.Id}", _mapper.Map<Evento>(evento));
               }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }

            return BadRequest();
            
        }

        [HttpDelete("{EventoId}")]
        public async Task<IActionResult> Delete(int EventoId)
        {
            try
            {
                var evento = await _repo.GetEventoAsyncById(EventoId, false);

                if (evento == null)
                {
                    return NotFound();
                }
               _repo.Delete(evento);

               if (await _repo.SaveChangesAsync())
               {
                   return Ok();
               }
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Banco de dados falhou {ex.Message}");
            }

            return BadRequest();
            
        }
        
    }
}