using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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

        [HttpPost("upload")]
        public async Task<IActionResult> Upload()
        {
            try
            {
               var file = Request.Form.Files[0];
               var folderName = Path.Combine("Resources", "Images");
               var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
                    var fullPath = Path.Combine(pathToSave, filename.Replace("\"", " ").Trim());

                    using(var stream = new FileStream(fullPath, FileMode.Create)) {
                        file.CopyTo(stream);
                    }
                }

               return Ok(); 
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
                var idLotes = new List<int>();
                var idRedesSociais = new List<int>();

                foreach (var item in model.Lotes)
                {
                    idLotes.Add(item.Id);
                }

                foreach (var item in model.RedesSociais)
                {
                    idRedesSociais.Add(item.Id);
                }


                var evento = await _repo.GetEventoAsyncById(EventoId, false);

                if (evento == null)
                {
                    return NotFound();
                }

                var lotes = evento.Lotes.Where(lote => !idLotes.Contains(lote.Id)).ToList<Lote>();
                var redesSociais = evento.RedesSociais.Where(rede => !idRedesSociais.Contains(rede.Id)).ToList<RedeSocial>();

                if (lotes.Count > 0)
                {
                   lotes.ForEach(lote => _repo.Delete(lotes));  
                }

                if (redesSociais.Count > 0)
                {
                   redesSociais.ForEach(rede => _repo.Delete(redesSociais));  
                }

                _mapper.Map(model, evento);

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