using Azure;
using Azure.AI.ContentSafety;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.event_.Domains;
using webapi.event_.Interfaces;

namespace webapi.event_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ComentariosEventosController : ControllerBase
    {
        private IComentariosEventosRepository _comentariosEventosRepository;
        private string request;
        private readonly ContentSafetyClient? contentSafetyClient;
        private readonly ContentSafetyClient _contentSafetyClient;

        public ComentariosEventosController(IComentariosEventosRepository comentariosEventosRepository)
        {
            _comentariosEventosRepository = comentariosEventosRepository;
            _contentSafetyClient = contentSafetyClient;
        }

        [HttpPost]

        public async Task<IActionResult> Post(ComentariosEventos comentario)
        {
            try
            {
                if(string.IsNullOrEmpty(comentario.Descricao))
                {
                    return BadRequest("O texto a ser moderado não pode estar vazio!");  
                }
                //criar objeto de análise do content safety
                var requesr = new AnalyzeTextOptions(comentario.Descricao);

                //chamar a API do Content Safety
                Response<AnalyzeTextResult> response = await _contentSafetyClient.AnalyzeTextAsync(request);

                //verificar se o texto analisado tem alguma severidade
                bool temConteudoImproprio = response.Value.CategoriesAnalysis.Any(c => c.Severity > 0);

                comentario.Exibe = !temConteudoImproprio;

                _comentariosEventosRepository.Cadastrar(comentario);

                return Ok();
            }
            catch (Exception e)
            {

             return BadRequest(e.Message);

            }
        }
    }
}
